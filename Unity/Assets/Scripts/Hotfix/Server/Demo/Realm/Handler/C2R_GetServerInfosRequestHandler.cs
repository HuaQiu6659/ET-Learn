namespace ET.Server
{
    [MessageSessionHandler(SceneType.Realm)]
    [FriendOfAttribute(typeof(ET.Server.ServerInfosManagerComponent))]
    public class C2R_GetServerInfosRequestHandler : MessageSessionHandler<C2R_GetServerInfosRequest, R2C_GetServerInfosResponse>
    {
        protected override async ETTask Run(Session session, C2R_GetServerInfosRequest request, R2C_GetServerInfosResponse response)
        {
            //检验token
            string token = session.Root().GetComponent<TokensComponent>().Get(request.account);
            if (token is null || token != request.token)
            {
                response.Error = (int)EErrorCode.ERR_TokenError;
                session.DisconnectAsync().Coroutine();
                return;
            }

            //传入区服信息
            foreach (var serverInfoRef in session.Root().GetComponent<ServerInfosManagerComponent>().serverInfos)
            {
                ServerInfo info = serverInfoRef;
                response.serverInfos.Add(info.ToMessage());
            }

            await ETTask.CompletedTask;
        }
    }
}