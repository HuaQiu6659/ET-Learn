using System.Linq;

namespace ET.Server
{
    [MessageSessionHandler(SceneType.Realm)]
    [FriendOf(typeof(ServerInfosComponent))]
    public class C2R_ServerListRequestHandler : MessageSessionHandler<C2R_ServerListRequest, R2C_ServerListResponse>
    {
        protected override async ETTask Run(Session session, C2R_ServerListRequest request, R2C_ServerListResponse response)
        {
            var scene = session.Root();
            var tokenPassed = scene.GetComponent<TokensComponent>().Verificate(request.Account, request.Token);
            if (!tokenPassed)
            {
                RelesaseWithError(EErrorCode.令牌验证失败);
                return;
            }

            response.List.AddRange(scene.GetComponent<ServerInfosComponent>().servers.Select(c => c.Entity.ToProto()));
            await ETTask.CompletedTask;

            void RelesaseWithError(EErrorCode err)
            {
                response.Error = (int)err;
                response.Message = err.ToString();
                session.DisconnectAsync().NoContext();
            }
        }
    }
}
