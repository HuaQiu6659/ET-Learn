/*
┌────────────────────────────┐
│　Description: 处理 Realm 的登录消息, 若是账号已经在服务器中登录了, 则向 Gate 发送断连
│　Remark: 
└────────────────────────────┘
*/
namespace ET.Server
{
    [MessageSessionHandler(SceneType.LoginCenter)]
    public partial class G2L_AddLoginRecordRequestHandler : MessageHandler<Scene, G2L_AddLoginRecordRequest, L2G_AddLoginRecordResponse>
    {
        protected override async ETTask Run(Scene scene, G2L_AddLoginRecordRequest request, L2G_AddLoginRecordResponse response)
        {
            var recorder = scene.GetComponent<LoginInfoRecorderComponent>();
            recorder.AddOrUpdate(request.AccountHash, (int)request.GateId);
            await ETTask.CompletedTask;
        }
    }
}