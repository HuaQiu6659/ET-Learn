namespace ET.Server
{
    [MessageHandler(SceneType.Gate)]
    public class L2G_DisconnectGateUnitRequestHandler : MessageHandler<Scene, L2G_DisconnectGateUnitRequest, G2LDisconnectGateUnitResponse>
    {
        protected override async ETTask Run(Scene unit, L2G_DisconnectGateUnitRequest request, G2LDisconnectGateUnitResponse response)
        {
            long accountID = request.account.GetLongHashCode();
            
            await ETTask.CompletedTask;
        }
    }
}