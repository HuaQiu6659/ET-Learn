namespace ET.Server
{
    [MessageSessionHandler(SceneType.Realm)]
    [FriendOfAttribute(typeof(ET.RoleInfos))]
    public class C2R_CreateRoleRequestHandler : MessageSessionHandler<C2R_CreateRoleRequest, R2C_CreateRoleResponse>
    {
        protected override async ETTask Run(Session session, C2R_CreateRoleRequest request, R2C_CreateRoleResponse response)
        {
            var realm = session.Root();
            //session锁
            if (session.GetComponent<SessionLockingComponent>() is not null)
            {
                response.Error = (int)EErrorCode.ERR_RequestRepeated;
                session.DisconnectAsync().Coroutine();
                return;
            }
            //Token
            bool isLegal = realm.GetComponent<TokensComponent>().CheckToken(request.account, request.token, session, response);
            if (!isLegal)
                return;
            //RoleName
            if (string.IsNullOrEmpty(request.roleName))
            {
                response.Error = (int)EErrorCode.ERR_RoleNameEmpty;
                return;
            }
            //协程锁
            CoroutineLockComponent coroutineLockComponent = realm.GetComponent<CoroutineLockComponent>();
            using (session.AddComponent<SessionLockingComponent>())
            {
                using (await coroutineLockComponent.Wait(CoroutineLockType.Role, request.account.GetLongHashCode()))
                {
                    var dbComponent = realm.GetComponent<DBManagerComponent>().GetZoneDB(session.Zone());
                    //检查重复
                    var roleInfos = await dbComponent.Query<RoleInfos>(r => r.name == request.roleName && r.serverID == request.serverID);
                    if (roleInfos?.Count > 0)
                    {
                        response.Error = (int)EErrorCode.ERR_RoleExisted;
                        return;
                    }
                    //创建角色
                    var newRole = session.AddChild<RoleInfos>();
                    newRole.account = request.account;
                    newRole.state = ERoleState.Normal;
                    newRole.name = request.roleName;
                    newRole.serverID = request.serverID;
                    newRole.createTime = TimeInfo.Instance.ServerNow();
                    await dbComponent.Save(newRole);
                    response.role = newRole.ToMessage();
                    newRole?.Dispose();
                }
            }

            await ETTask.CompletedTask;
        }
    }
}
