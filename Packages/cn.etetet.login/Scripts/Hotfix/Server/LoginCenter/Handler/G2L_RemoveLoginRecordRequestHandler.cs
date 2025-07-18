/*
┌────────────────────────────┐
│　Description: 
│　Remark: 
└────────────────────────────┘
*/
namespace ET.Server
{
    [MessageHandler(SceneType.LoginCenter)]
    public partial class G2L_RemoveLoginRecordRequestHandler : MessageHandler<Scene, G2L_RemoveLoginRecordRequest, L2G_RemoveLoginRecordResponse>
    {
        protected override async ETTask Run(Scene scene, G2L_RemoveLoginRecordRequest request, L2G_RemoveLoginRecordResponse response)
        {
            var loginRecorder = scene.GetComponent<LoginInfoRecorderComponent>();
            var gateId = loginRecorder.Get(request.AccountHash);
            if (gateId == request.GateId)
                loginRecorder.Remove(request.AccountHash);
            await ETTask.CompletedTask;
        }
    }
}