namespace ET.Server
{
    [MessageHandler(SceneType.LoginCenter)]
    public class R2L_LoginAccountRequestHandler : MessageHandler<Scene, R2L_LoginAccountRequest, L2R_LoginAccountResponse>
    {
        protected override async ETTask Run(Scene scene, R2L_LoginAccountRequest request, L2R_LoginAccountResponse response)
        {
            long accountID = request.account.GetLongHashCode();
            CoroutineLockComponent coroutineLockComponent = scene.GetComponent<CoroutineLockComponent>();
            using (await coroutineLockComponent.Wait(CoroutineLockType.LoginCenterLogin, accountID))
            {
                var recorder = scene.GetComponent<LoginInfosRecordComponent>();
                //账号不在线
                if (!recorder.Contains(accountID))
                    return;

                //账号已经在线则强制下线
                int zone = recorder.Get(accountID);
                StartSceneConfig gateConfig = RealmGateAddressHelper.GetGate(zone, request.account);
                
                L2G_DisconnectGateUnitRequest disconnectGateUnitRequest = L2G_DisconnectGateUnitRequest.Create();
                disconnectGateUnitRequest.account = request.account;
                var disconnectGateUnitResponse =
                        await scene.GetComponent<MessageSender>().Call(gateConfig.ActorId, disconnectGateUnitRequest) as G2LDisconnectGateUnitResponse;
                
                response.Error = disconnectGateUnitResponse.Error;
            }
        }
    }
}