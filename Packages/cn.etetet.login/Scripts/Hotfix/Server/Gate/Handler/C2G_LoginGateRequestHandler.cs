namespace ET.Server
{
    //登录网关时会检测账号是否已经登录
    //若是已经登录  则进行挤下线的操作(替换session)
    [MessageSessionHandler(SceneType.Gate)]
    public class C2G_LoginGateRequestHandler : MessageSessionHandler<C2G_LoginGateRequest, G2C_LoginGateResponse>
    {
        protected override async ETTask Run(Session session, C2G_LoginGateRequest request, G2C_LoginGateResponse response)
        {
            Scene root = session.Root();
            var locker = session.Lock(out var hadLocked);
            if (hadLocked)
            {
                RelesaseWithError(EErrorCode_Login.请求过于频繁);
                return;
            }
            using var toRelease = locker;

            var gateKeysCmp = root.GetComponent<GateSessionKeyComponent>();
            string account = gateKeysCmp.Get(request.Key);
            long accountHash = account.GetLongHashCode();
            if (account == null)
            {
                RelesaseWithError(EErrorCode_Login.网关令牌错误);
                return;
            }
            //验证完成就销毁令牌
            gateKeysCmp.Remove(request.Key);
            session.RemoveComponent<SessionAcceptTimeoutComponent>();

            CoroutineLockComponent coroutineLockComponent = root.GetComponent<CoroutineLockComponent>();
            var instanceId = session.InstanceId;
            using var coroutineLocker = await coroutineLockComponent.Wait(CoroutineLockType.LoginGate, accountHash);
            if (instanceId != session.InstanceId)
            {
                RelesaseWithError(EErrorCode_Login.客户端连接发生变化);
                return;
            }

            //向登录中心服务器进行登记
            G2L_AddLoginRecordRequest addLoginRequest = G2L_AddLoginRecordRequest.Create();
            {
                addLoginRequest.AccountHash = accountHash;
                addLoginRequest.GateId = root.Zone();
            }
            L2G_AddLoginRecordResponse addLoginRecordResponse = await root.GetComponent<MessageSender>().Call(StartSceneConfigCategory.Instance.LoginCenterConfig.ActorId, addLoginRequest) as L2G_AddLoginRecordResponse;
            if (addLoginRecordResponse.Error != ErrorCode.ERR_Success)
            {
                response.Error = addLoginRecordResponse.Error;
                response.Message = addLoginRecordResponse.Message;
                session?.DisconnectAsync().NoContext();
                return;
            }

            //检测该账号是否已经登录
            PlayerComponent playerComponent = root.GetComponent<PlayerComponent>();
            Player player = playerComponent.GetByAccount(account);
            if (player != null)
            {
                //角色在线, 直接替换连接
                player.GetComponent<PlayerSessionComponent>().Session = session;
                player.RemoveComponent<PlayerOfflineOutTimeComponent>();
                session.GetComponent<SessionPlayerComponent>().Player = player;
            }
            else
            {
                player = playerComponent.AddChild<Player, string>(account);
                playerComponent.Add(player);
                PlayerSessionComponent playerSessionComponent = player.AddComponent<PlayerSessionComponent>();
                playerSessionComponent.AddComponent<MailBoxComponent, int>(MailBoxType.GateSession);
                await playerSessionComponent.AddLocation(LocationType.GateSession);

                player.AddComponent<MailBoxComponent, int>(MailBoxType.UnOrderedMessage);
                await player.AddLocation(LocationType.Player);

                playerSessionComponent.Session = session;
                player.State = EPlayerState.登陆中;
            }
            session.AddComponent<SessionPlayerComponent>().Player = player;

            response.PlayerId = player.Id;

            void RelesaseWithError(EErrorCode_Login err)
            {
                response.Error = (int)err;
                response.Message = err.ToString();
                session.DisconnectAsync().NoContext();
            }
        }
    }
}