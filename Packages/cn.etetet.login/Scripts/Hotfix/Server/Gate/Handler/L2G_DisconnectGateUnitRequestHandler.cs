namespace ET.Server
{
    [MessageHandler(SceneType.Gate)]
    public class L2G_DisconnectGateUnitRequestHandler : MessageHandler<Scene, L2G_DisconnectGateUnitRequest, G2L_DisconnectGateUnitResponse>
    {
        protected override async ETTask Run(Scene scene, L2G_DisconnectGateUnitRequest request, G2L_DisconnectGateUnitResponse response)
        {
            var accountHash = request.Account.GetLongHashCode();
            using var coroutineLocker = await scene.GetComponent<CoroutineLockComponent>().Wait(CoroutineLockType.LoginGate, accountHash);
            var playerComponent = scene.GetComponent<PlayerComponent>();
            var player = playerComponent.GetByAccount(request.Account);
            if (player is null)
                return;

            scene.GetComponent<GateSessionKeyComponent>().Remove(accountHash);
            PlayerSessionComponent sessionCmp = player.GetComponent<PlayerSessionComponent>();
            A2C_Disconnect disconnect = A2C_Disconnect.Create();
            {
                disconnect.Error = (int)EErrorCode_Login.账号在其他客户端登录;
            }
            sessionCmp.Session.Send(disconnect);
            sessionCmp.Session.DisconnectAsync().NoContext();
            sessionCmp.Session = null;
            player.AddComponent<PlayerOfflineOutTimeComponent>();
        }
    }
}