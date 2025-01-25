namespace ET.Server
{
    [MessageHandler(SceneType.LoginCenter)]
    public class G2L_RecordLoginInfosRequestHandler : MessageHandler<Scene, G2L_RecordLoginInfosRequest, L2G_RecordLoginInfosResponse>
    {
        protected override async ETTask Run(Scene scene, G2L_RecordLoginInfosRequest request, L2G_RecordLoginInfosResponse response)
        {
            long accountID = request.account.GetLongHashCode();
            CoroutineLockComponent locker = scene.GetComponent<CoroutineLockComponent>();
            LoginInfosRecordComponent recoder = scene.GetComponent<LoginInfosRecordComponent>();
            using (await locker.Wait(CoroutineLockType.LoginCenterLogin, accountID))
            {
                if (recoder.Contains(accountID))
                {
                    response.Error = (int)EErrorCode.ERR_AccountLogined;
                    return;
                }
                
                recoder.Add(accountID, request.serverID);
            }
            
            await ETTask.CompletedTask;
        }
    }
}
