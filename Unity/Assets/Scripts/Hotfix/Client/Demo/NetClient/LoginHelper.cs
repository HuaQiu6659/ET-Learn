namespace ET.Client
{
    public static class LoginHelper
    {
        public static async ETTask Login(Scene root, string account, string password)
        {
            root.RemoveComponent<ClientSenderComponent>();
            
            ClientSenderComponent clientSenderComponent = root.AddOrGetComponent<ClientSenderComponent>();

            var response = await clientSenderComponent.LoginAsync(account, password);
            if (response.Error != ErrorCode.ERR_Success)
            {
                Log.Error($"LoginFailed: {response.Error}");
                await EventSystem.Instance.PublishAsync(root, new LoginFailedEvent() { errorCode = response.Error });
                return;
            }

            string token = response.token;
            //root.GetComponent<PlayerComponent>().MyId = response.PlayerId;
            //获取服务器列表
            C2R_GetServerInfosRequest serverInfosRequest = C2R_GetServerInfosRequest.Create();
            serverInfosRequest.account = account;
            serverInfosRequest.token = token;
            R2C_GetServerInfosResponse serverInfosResponse = await clientSenderComponent.Call(serverInfosRequest) as R2C_GetServerInfosResponse;
            if (serverInfosResponse.Error != ErrorCode.ERR_Success)
            {
                Log.Error($"{serverInfosResponse.Error} 获取服务器列表失败");
                return;
            }
            //默认直接取第一个区服，此处是有服务器列表的，可以展示让玩家自选
            ServerInfoProto serverInfoProto = serverInfosResponse.serverInfos[0];
            if (serverInfoProto.state == (int)EServerState.Stop)
            {
                Log.Error($"区服{serverInfoProto.serverName}停运");
                return;
            }
            Log.Debug($"获取区服：{serverInfoProto.serverName}-{serverInfoProto.id}");
            
            //获取区服角色列表
            C2R_GetRolesRequest getRolesRequest = C2R_GetRolesRequest.Create();
            getRolesRequest.token = token;
            getRolesRequest.account = account;
            getRolesRequest.serverID = serverInfoProto.id;
            R2C_GetRolesResponse getRolesResponse = await clientSenderComponent.Call(getRolesRequest) as R2C_GetRolesResponse;
            if (getRolesResponse.Error != ErrorCode.ERR_Success)
            {
                Log.Error($"{getRolesResponse.Error} 获取角色列表失败");
                return;
            }

            //无角色，发送创建请求
            RoleInfoProto roleInfoProto;
            if (getRolesResponse.roles.Count < 1)
            {
                C2R_CreateRoleRequest createRoleRequest = C2R_CreateRoleRequest.Create();
                createRoleRequest.token = token;
                createRoleRequest.account = account;
                createRoleRequest.roleName = account;
                createRoleRequest.serverID = serverInfoProto.id;
                
                R2C_CreateRoleResponse createRoleResponse = await clientSenderComponent.Call(createRoleRequest) as R2C_CreateRoleResponse;
                if (createRoleResponse.Error != ErrorCode.ERR_Success)
                {
                    Log.Error($"{createRoleResponse.Error} 创建角色失败");
                    return;
                }

                roleInfoProto = createRoleResponse.role;
            }
            else
            {
                roleInfoProto = getRolesResponse.roles[0];
            }
            
            //请求获取ReamlKey
            C2R_RealmKeyRequest realmKeyRequest = C2R_RealmKeyRequest.Create();
            realmKeyRequest.account = account;
            realmKeyRequest.token = token;
            realmKeyRequest.serverID = serverInfoProto.id;
            var realmKeyResponse = await Call<R2C_RealmKeyResponse>(realmKeyRequest);
            if (realmKeyResponse.Error != ErrorCode.ERR_Success)
            {
                Log.Error("获取RealmKey失败");
                return;
            } 
            
            //请求角色进入地图
            await clientSenderComponent.LoginGameAsync(account, realmKeyResponse.reamlKey, roleInfoProto.id, realmKeyResponse.gateAddress);
            
            
            await EventSystem.Instance.PublishAsync(root, new LoginFinish());

            async ETTask<TResponse> Call<TResponse>(IRequest request) where TResponse : class, ISessionResponse => await clientSenderComponent.Call(request) as TResponse;
        }
    }
}