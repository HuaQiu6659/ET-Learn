namespace ET.Server
{
    [MessageSessionHandler(SceneType.Realm)]
    public class C2R_RealmKeyRequestHandler : MessageSessionHandler<C2R_RealmKeyRequest, R2C_RealmKeyResponse>
    {
        protected override async ETTask Run(Session session, C2R_RealmKeyRequest request, R2C_RealmKeyResponse response)
        {
            if (session.IsRepeateRequest(response))
                return;

            bool isLegal = session.Root().GetComponent<TokensComponent>().CheckToken(request.account, request.token, session, response);
            if (isLegal)
                return;

            CoroutineLockComponent coroutineLockComponent = session.Root().GetComponent<CoroutineLockComponent>();
            using (session.Lock())
            {
                using (await coroutineLockComponent.Wait(CoroutineLockType.Login, request.account.GetLongHashCode()))
                {
                    //分配Gate
                    StartSceneConfig config = RealmGateAddressHelper.GetGate(request.serverID, request.account);
                    Log.Debug($"Account: {request.account}\tGate: {config}");
                    
                    //向Gate请求key，客户端利用key连接Gate
                    R2G_GetLoginKey getLoginKey = R2G_GetLoginKey.Create();
                    getLoginKey.Account = request.account;
                    G2R_GetLoginKey getLoginKeyResponse = (G2R_GetLoginKey)await session.Root().GetComponent<MessageSender>().Call(config.ActorId, getLoginKey);

                    response.gateAddress = config.InnerIPPort.ToString();
                    response.reamlKey = getLoginKeyResponse.Key;
                    response.gateID = getLoginKeyResponse.GateId;
                    response.Error = getLoginKeyResponse.Error;
                    
                    //客户端会连接网关，因此可断开连接
                    session.DisconnectAsync().Coroutine();
                }
            }
            await ETTask.CompletedTask;
        }
    }
}
