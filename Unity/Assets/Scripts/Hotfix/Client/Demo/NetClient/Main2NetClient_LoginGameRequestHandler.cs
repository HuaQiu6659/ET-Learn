namespace ET.Client
{
    [MessageHandler(SceneType.NetClient)]
    public class Main2NetClient_LoginGameRequestHandler : MessageHandler<Scene, Main2NetClient_LoginGameRequest, NetClient2Main_LoginGameResponse>
    {
        protected override async ETTask Run(Scene root, Main2NetClient_LoginGameRequest request, NetClient2Main_LoginGameResponse response)
        {
            NetComponent netComponent = root.GetComponent<NetComponent>();
            Session gateSession =
                    await netComponent.CreateRouterSession(NetworkHelper.ToIPEndPoint(request.gateAddress), request.account, request.account);
            gateSession.AddComponent<ClientSessionErrorComponent>();
            root.GetComponent<SessionComponent>().Session = gateSession;
            
            C2G_LoginGameGateRequest loginGameGateRequest = C2G_LoginGameGateRequest.Create();
            {
                loginGameGateRequest.account = request.account;
                loginGameGateRequest.key = request.realmKey;
                loginGameGateRequest.roleID = request.roleID;
            }
            
            G2C_LoginGameGateResponse loginGameGateResponse = await gateSession.Call<G2C_LoginGameGateResponse>(loginGameGateRequest);
            if (loginGameGateResponse.Error != ErrorCode.ERR_Success)
            {
                response.Error = loginGameGateResponse.Error;
                Log.Error($"登录Gate失败 {(EErrorCode)loginGameGateResponse.Error}");
                return;
            }
                
            Log.Debug("登录Gate成功");
            
            
            
            await ETTask.CompletedTask;
        }
    }
}

