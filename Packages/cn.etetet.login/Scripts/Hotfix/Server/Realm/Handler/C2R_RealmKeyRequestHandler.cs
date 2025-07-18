namespace ET.Server
{
    [MessageSessionHandler(SceneType.Realm)]
    public class C2R_RealmKeyRequestHandler : MessageSessionHandler<C2R_RealmKeyRequest, R2C_RealmKeyResponse>
    {
        protected override async ETTask Run(Session session, C2R_RealmKeyRequest request, R2C_RealmKeyResponse response)
        {
            session.Lock(out var hadLocked);
            if (hadLocked)
            {
                RelesaseWithError(EErrorCode_Login.请求过于频繁);
                return;
            }
            var scene = session.Root();

            var verificated = scene.GetComponent<TokensComponent>().Verificate(request.Account, request.Token);
            if (!verificated)
            {
                RelesaseWithError(EErrorCode_Login.令牌验证失败);
                return;
            }

            var accountHash = request.Account.GetLongHashCode();

            var coroutineLockerCmp = scene.GetComponent<CoroutineLockComponent>();
            using var coroutineLocker = await coroutineLockerCmp.Wait(CoroutineLockType.RealmKey, accountHash);

            //此时Session是在Realm 上, Realm是在公共服务器 1000, 那么就不能直接使用 session.Zone() 进行区服配置获取
            //获取gate信息
            var gateConfig = RealmGateAddressHelper.GetGate((int)request.ServerId, accountHash);
            //向gate获取令牌, 用户拿着令牌登录gate
            R2G_GetLoginKey r2gGetLoginKey = R2G_GetLoginKey.Create();
            {
                r2gGetLoginKey.Account = request.Account;
            }
            var g2rGetLoginKey = await scene.GetComponent<MessageSender>().Call(gateConfig.ActorId, r2gGetLoginKey) as G2R_GetLoginKey;

            response.GateAddress = gateConfig.InnerIPPort.ToString();
            response.Key = g2rGetLoginKey.Key;
            response.GateId = g2rGetLoginKey.GateId;

            session?.DisconnectAsync().NoContext();

            void RelesaseWithError(EErrorCode_Login err)
            {
                response.Error = (int)err;
                response.Message = err.ToString();
                session.DisconnectAsync().NoContext();
            }
        }
    }
}
