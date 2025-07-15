/*
┌────────────────────────────┐
│　Description: 处理 Realm 的登录消息, 若是账号已经在服务器中登录了, 则向 Gate 发送断连
│　Remark: 
└────────────────────────────┘
*/
namespace ET.Server
{
    [MessageSessionHandler(SceneType.LoginCenter)]
    public partial class R2L_LoginRequestHandler : MessageHandler<Scene, R2L_LoginRequest, L2R_LoginResponse>
    {
        protected override async ETTask Run(Scene scene, R2L_LoginRequest request, L2R_LoginResponse response)
        {
            var accountHash = request.Account.GetLongHashCode();
            var coroutineLockerCmp = scene.GetComponent<CoroutineLockComponent>();
            using var coroutineLocker = await coroutineLockerCmp.Wait(CoroutineLockType.LoginCenter, accountHash);

            var loginRecoreder = scene.GetComponent<LoginInfoRecorderComponent>();
            if (!loginRecoreder.IsLogined(accountHash))
                return;

            int zone = loginRecoreder.Get(accountHash);
            StartSceneConfig gateConfig = RealmGateAddressHelper.GetGate(zone, accountHash);

            var l2gRequest = L2G_DisconnectGateUnitRequest.Create();
            {
                l2gRequest.Account = request.Account;
            }
            var g2lResponse = await scene.GetComponent<MessageSender>().Call(gateConfig.ActorId, l2gRequest) as G2L_DisconnectGateUnitResponse;
            response.Error = g2lResponse.Error;
            response.Message = g2lResponse.Message;
        }
    }
}