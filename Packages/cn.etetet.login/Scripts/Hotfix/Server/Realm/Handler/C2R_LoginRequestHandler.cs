
namespace ET.Server
{

    [MessageSessionHandler(SceneType.Realm)]
    [FriendOf(typeof(AccountInfo))]
    [FriendOfAttribute(typeof(ET.Server.AccountSessionsComponent))]
    public class C2R_LoginRequestHandler : MessageSessionHandler<C2R_LoginRequest, R2C_LoginRespose>
    {
        protected override async ETTask Run(Session session, C2R_LoginRequest request, R2C_LoginRespose response)
        {
            // 视为正常连接
            session.RemoveComponent<SessionAcceptTimeoutComponent>();

            if (session.GetComponent<SessionLockingComponent>() != null)
            {
                ReleaseWithError(EErrorCode_Login.请求过于频繁);
                return;
            }
            // 登录结束就释放锁
            using var locking = session.AddComponent<SessionLockingComponent>();

            // 判断账号密码是否合法(长度, 非法字符)
            if (!AccountHelper.IsValidAccount(request.Account))
            {
                ReleaseWithError(EErrorCode_Login.账号不合法);
                return;
            }
            if (!AccountHelper.IsValidPassword(request.Password))
            {
                ReleaseWithError(EErrorCode_Login.密码不合法);
                return;
            }

            var scene = session.Root();

            // 数据库操作应该使用协程锁
            AccountInfo account = null;
            using (await scene.GetComponent<CoroutineLockComponent>().Wait(CoroutineLockType.LoginAccount, request.Account.GetLongHashCode()))
            {
                // 根据配置表 zone 获取对应的数据库
                var dbMgr = scene.GetComponent<DBManagerComponent>().GetZoneDB(session.Zone());
                var infos = await dbMgr.Query<AccountInfo>(info => info.account == request.Account);

                // 数据库中没有对应账号
                if (infos.Count == 0)
                {
                    if (!request.ByVerificationCode)
                    {
                        ReleaseWithError(EErrorCode_Login.账号不存在);
                        return;
                    }

                    if (!await scene.GetComponent<VerificationCodeComponent>().Verificate(request.Account, request.Password))
                    {
                        ReleaseWithError(EErrorCode_Login.验证码错误);
                        return;
                    }

                    // 验证码登录的情况下自动注册账号
                    var now = TimeInfo.Instance.ServerNow();
                    account = session.AddChild<AccountInfo>();
                    {
                        account.account = request.Account;
                        account.password = string.Empty;
                        account.state = EAccountState.用户;
                        account.createTime = now;
                        account.lastLoginTime = now;
                    }
                }
                else
                {
                    account = infos[0];
                    if (string.IsNullOrEmpty(account.password))
                    {
                        ReleaseWithError(EErrorCode_Login.未设置密码);
                        return;
                    }

                    if (account.password != request.Password)
                    {
                        ReleaseWithError(EErrorCode_Login.密码错误);
                        return;
                    }

                    if (account.state == EAccountState.封禁)
                    {
                        ReleaseWithError(EErrorCode_Login.账号已被封禁);
                        return;
                    }

                    account.lastLoginTime = TimeInfo.Instance.ServerNow();
                    session.AddChild(account);
                }

                await dbMgr.Save(account);
            }

            // 向登录中心进行登录请求
            R2L_LoginRequest loginRst = R2L_LoginRequest.Create();
            {
                loginRst.Account = request.Account;
            }
            var loginCenterConfig = StartSceneConfigCategory.Instance.LoginCenterConfig;
            var loginRsp = await scene.GetComponent<MessageSender>().Call(loginCenterConfig.ActorId, loginRst) as L2R_LoginResponse;
            if (loginRsp.Error != ErrorCode.ERR_Success)
            {
                response.Error = loginRsp.Error;
                session.Disconnect().NoContext();
                return;
            }

            // 将Session替换成当前连接的Session
            var sessionsComponent = scene.GetComponent<AccountSessionsComponent>();
            var otherSession = sessionsComponent.Get(request.Account);

            // 当前账号已经登录, 强制挤下线
            if (otherSession != null)
            {
                //otherSession.Send();
                otherSession.Disconnect().NoContext();
            }
            scene.GetComponent<AccountSessionsComponent>().AddOrUpdate(account.account, session);
            session.AddComponent<AccountCheckOutTimeComponent, string>(account.account);

            // 更换成当前账号的Token
            var tokensCmp = scene.GetComponent<TokensComponent>();
            string token = tokensCmp.CreateToken();
            tokensCmp.AddOrUpdate(request.Account, token);
            response.Token = token;

            void ReleaseWithError(EErrorCode_Login err)
            {
                response.Error = (int)err;
                response.Message = err.ToString();
                session.Disconnect().NoContext();
            }
        }


    }
}
