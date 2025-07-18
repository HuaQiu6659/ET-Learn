namespace ET.Client
{
    [MessageHandler(SceneType.NetClient)]
    public class Main2NetClient_EnterGameRequestHandler : MessageHandler<Scene, Main2NetClient_EnterGameRequest, NetClient2Main_EnterGameResponse>
    {
        protected override async ETTask Run(Scene scene, Main2NetClient_EnterGameRequest request, NetClient2Main_EnterGameResponse response)
        {
            string account = request.Account;
            var netComponent = scene.GetComponent<NetComponent>();
            var gateSession = await netComponent.CreateRouterSession(NetworkHelper.ToIPEndPoint(request.GateAddress), account, account.GetLongHashCode());
            scene.GetComponent<SessionComponent>().Session = gateSession;
            gateSession.AddComponent<ClientSessionErrorComponent>();

            var loginGateRequest = C2G_LoginGateRequest.Create();
            {
                loginGateRequest.Key = request.GateKey;
                loginGateRequest.Account = account;
            }
            var loginGateResponse = await gateSession.Call(loginGateRequest) as  G2C_LoginGateResponse;
            if (loginGateResponse.Error != ErrorCode.ERR_Success)
            {
                response.Error = loginGateResponse.Error;
                response.Message = loginGateResponse.Message;
                return;
            }

            G2C_EnterGameResponse enterGameResponse = await gateSession.Call(C2G_EnterGameRequest.Create()) as G2C_EnterGameResponse;
            if (enterGameResponse.Error != ErrorCode.ERR_Success)
            {
                response.Error = enterGameResponse.Error;
                response.Message = enterGameResponse.Message;
                return;
            }

            Log.Debug("进入游戏成功");
        }
    }
}