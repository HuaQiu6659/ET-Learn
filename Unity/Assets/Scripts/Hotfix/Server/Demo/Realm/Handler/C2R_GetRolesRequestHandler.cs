using System.Collections.Generic;

namespace ET.Server
{
    [MessageSessionHandler(SceneType.Realm)]
    [FriendOfAttribute(typeof(ET.RoleInfos))]
    public class C2R_GetRolesRequestHandler : MessageSessionHandler<C2R_GetRolesRequest, R2C_GetRolesResponse>
    {
        protected override async ETTask Run(Session session, C2R_GetRolesRequest request, R2C_GetRolesResponse response)
        {
            if (session.GetComponent<SessionLockingComponent>() != null)
            {
                response.Error = (int)EErrorCode.ERR_RequestRepeated;
                session.DisconnectAsync().Coroutine();
                return;
            }

            bool isLegal = session.Root().GetComponent<TokensComponent>().CheckToken(request.account, request.token, session, response);
            if (!isLegal)
                return;

            CoroutineLockComponent coroutineLockComponent = session.Root().GetComponent<CoroutineLockComponent>();
            using (session.AddComponent<SessionLockingComponent>())
            {
                using (await coroutineLockComponent.Wait(CoroutineLockType.Role, request.account.GetLongHashCode()))
                {
                    DBComponent dbComponent = session.Root().GetComponent<DBManagerComponent>().GetZoneDB(session.Zone());
                    List<RoleInfos> roleInfos = await dbComponent.Query<RoleInfos>(r =>
                            r.account == request.account &&
                            request.serverID == r.serverID &&
                            r.state == ERoleState.Normal);
                    
                    response.roles = new List<RoleInfoProto>(roleInfos.Count);
                    for (int i = 0; i < roleInfos.Count; i++)
                    {
                        using var role = roleInfos[i];
                        response.roles.Add(role.ToMessage());
                    }
                    roleInfos.Clear();
                }
            }
            
            await ETTask.CompletedTask;
        }
    }
}
