namespace ET.Server
{
    [MessageSessionHandler(SceneType.Realm)]
    [FriendOfAttribute(typeof(ET.Server.AccountInfo))]
    public class C2R_LoginHandler : MessageSessionHandler<C2R_Login, R2C_Login>
    {
        protected override async ETTask Run(Session session, C2R_Login request, R2C_Login response)
        {
            //账号or密码为空
            if (string.IsNullOrEmpty(request.Account) || string.IsNullOrEmpty(request.Password))
            {
                response.Error = ErrorCode.ERR_LoginInfoEmpty;
                CloseSession(session).Coroutine();
                return;
            }

            //利用账号进行协程锁，避免多客户端同时对同一账号进行登录、注册
            using (await session.Root().GetComponent<CoroutineLockComponent>().Wait(CoroutineLockType.Login, request.Account.GetLongHashCode()))
            {
                //从数据库中检测账号密码正确性
                //通过DBMgr获取对应区服的服务器		zone为区服信息
                DBComponent dbCpm = session.Root().AddOrGetComponent<DBManagerComponent>().GetZoneDB(session.Zone());
                var accounts = await dbCpm.Query<AccountInfo>(a => a.account == request.Account);
                //账号未注册
                if (accounts.Count <= 0)
                {
                    //注册流程
                    var accountInfosCmp = session.AddOrGetComponent<AccountInfosComponent>();
                    var accountInfo = accountInfosCmp.AddChild<AccountInfo>();
                    accountInfo.account = request.Account;
                    accountInfo.password = request.Password;
                    Log.Debug($"创建账号：{request.Account}");
                    await dbCpm.Save(accountInfo);
                }
                else
                {
                    AccountInfo InDb = accounts[0];
                    //密码错误
                    if (InDb.password != request.Password)
                    {
                        response.Error = ErrorCode.ERR_LoginWrongPassword;
                        CloseSession(session).Coroutine();
                        return;
                    }
                }
            }
            
            //网络消息通讯时先要获取StartSceneConfig
            //再利用config.ActorID告诉MessageSender对特定实体进行通信

            // 随机分配一个Gate
            StartSceneConfig config = RealmGateAddressHelper.GetGate(session.Zone(), request.Account);
            Log.Debug($"gate address: {config}");

            // 向gate请求一个key,客户端可以拿着这个key连接gate
            R2G_GetLoginKey r2GGetLoginKey = R2G_GetLoginKey.Create();
            r2GGetLoginKey.Account = request.Account;
            G2R_GetLoginKey g2RGetLoginKey = (G2R_GetLoginKey)await session.Fiber().Root.GetComponent<MessageSender>().Call(
                config.ActorId, r2GGetLoginKey);

            response.Address = config.InnerIPPort.ToString();
            response.Key = g2RGetLoginKey.Key;
            response.GateId = g2RGetLoginKey.GateId;

            //不在这个方法里直接写Dispose是为了保证Response能正常发送出去
            CloseSession(session).Coroutine();
        }

        private async ETTask CloseSession(Session session)
        {
            await session.Root().GetComponent<TimerComponent>().WaitAsync(1000);
            session.Dispose();
        }
    }
}
