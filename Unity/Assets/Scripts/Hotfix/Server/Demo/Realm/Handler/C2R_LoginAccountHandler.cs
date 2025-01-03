using System.Text.RegularExpressions;

namespace ET.Server
{
    [MessageSessionHandler(SceneType.Realm)]
    [FriendOf(typeof(AccountInfo))]
    public class C2R_LoginAccountHandler : MessageSessionHandler<C2R_LoginAccountRequest, R2C_LoginAccountResponse>
    {
        protected override async ETTask Run(Session session, C2R_LoginAccountRequest request, R2C_LoginAccountResponse response)
        {
            //记得去除TimeoutComponent 不然5秒后连接就自动断开了
            session.RemoveComponent<SessionAcceptTimeoutComponent>();

            //短时间内重复请求
            if (session.GetComponent<SessionLockingComponent>() is not null)
            {
                response.Error = (int)EErrorCode.ERR_RequestRepeated;
                session.DisconnectAsync().Coroutine();
                return;
            }

            if (string.IsNullOrEmpty(request.Account) || string.IsNullOrEmpty(request.Password))
            {
                response.Error = (int)EErrorCode.ERR_Account_InputEmpty;
                session.DisconnectAsync().Coroutine();
                return;
            }

            //限制6~15位长度的数字字母组合
            if (Regex.IsMatch(request.Account.Trim(), @"^[a-zA-Z0-9]{6,15}$"))
            {
                response.Error = (int)EErrorCode.ERR_Account_InillegalAcount;
                session.DisconnectAsync().Coroutine();
                return;
            }

            //限制8~30位长度的数字字母组合
            if (Regex.IsMatch(request.Password.Trim(), @"^[a-zA-Z0-9]{8,30}$"))
            {
                response.Error = (int)EErrorCode.ERR_Account_InillegalPassword;
                session.DisconnectAsync().Coroutine();
                return;
            }

            //协程锁保证同一账号登录的先后性
            var coroutineLocker = session.Root().GetComponent<CoroutineLockComponent>();
            //避免短时间内重复请求
            using (session.AddComponent<SessionLockingComponent>())
            {
                using (await coroutineLocker.Wait(CoroutineLockType.Login, request.Account.GetLongHashCode()))
                {
                    //获取数据库
                    var dbComponent = session.Root().GetComponent<DBManagerComponent>().GetZoneDB(session.Zone());
                    var accountInfos = await dbComponent.Query<AccountInfo>(a => a.account == request.Account);
                    //未注册
                    if (accountInfos?.Count == 0)
                    {
                        response.Error = (int)EErrorCode.ERR_Account_AccountUnregistered;
                        session.DisconnectAsync().Coroutine();
                        return;
                    }

                    var accountInfo = accountInfos[0];
                    var infosCmp = session.AddOrGetComponent<AccountInfosComponent>();
                    infosCmp.AddChild(accountInfo);
                    //密码错误
                    if (accountInfo.password != request.Password)
                    {
                        response.Error = (int)EErrorCode.ERR_Account_WrongPassword;
                        session.DisconnectAsync().Coroutine();
                        return;
                    }

                    //账号被封禁
                    if (accountInfo.accountType == EAccountType.BlackList)
                    {
                        response.Error = (int)EErrorCode.ERR_Account_BlackList;
                        session.DisconnectAsync().Coroutine();
                        return;
                    }
                    
                    //正常登录
                    R2L_LoginAccountRequest r2lReq = R2L_LoginAccountRequest.Create();
                    r2lReq.account = request.Account;
                    //服务器信息配置直接通过 StartSceneConfigCategory.Instance 获取
                    StartSceneConfig loginCenterConfig = StartSceneConfigCategory.Instance.LoginCenterConfig;
                    var l2rRsp = (L2R_LoginAccountResponse)await session.Fiber().Root.GetComponent<MessageSender>().Call(loginCenterConfig.ActorId, r2lReq);
                    //登陆失败
                    if (l2rRsp.Error != ErrorCode.ERR_Success)
                    {
                        response.Error = l2rRsp.Error;
                        session.DisconnectAsync().Coroutine();
                        return;
                    }

                    var sessionsCmp = session.Root().GetComponent<AccountSessionsComponent>();
                    //检查相同账号在其他客户端是否已经登录
                    Session otherSession = sessionsCmp.Get(request.Account);
                    {
                        otherSession?.Send(A2C_Disconnect.Create());
                        otherSession.DisconnectAsync().Coroutine();
                    }
                    sessionsCmp.Add(request.Account, session);
                    session.AddComponent<AccountCheckTimeOutComponent, string>(request.Account);
                    //替换token
                    string token = $"{TimeInfo.Instance.ServerNow()}{RandomGenerator.RandomNumber(int.MinValue, int.MinValue)}";
                    var tokenCmp = session.Root().GetComponent<TokensComponent>();
                    {
                        tokenCmp.Add(request.Account, token);
                    }

                    response.token = token;
                }
            }
            
            await ETTask.CompletedTask;
        }
    }
}
