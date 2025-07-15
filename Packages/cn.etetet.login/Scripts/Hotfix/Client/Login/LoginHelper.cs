using Cysharp.Text;

namespace ET.Client
{
    public static class LoginHelper
    {
        public static async ETTask LoginByPassword(Scene root, string address, string account, string password)
        {
            //保证登录与上一次的链接无关联
            ClientSenderComponent clientSenderComponent = root.ReplaceComponent<ClientSenderComponent>();

            using var response = await clientSenderComponent.LoginAsync(address, account, password);
            if (response.Error != ErrorCode.ERR_Success)
            {
                Log.Error(ZString.Format("登录失败, {0}:{1}", response.Error, response.Message));
                return;
            }
            var token = response.Token;

            //获取服务器列表
            using C2R_ServerListRequest serverListRequest = C2R_ServerListRequest.Create();
            {
                serverListRequest.Account = account;
                serverListRequest.Token = token;
            }
            using var serverListResponse = await clientSenderComponent.Call(serverListRequest) as R2C_ServerListResponse;
            if (serverListResponse.Error != ErrorCode.ERR_Success)
            {
                Log.Error(ZString.Format("获取服务器列表失败, {0}:{1}", serverListResponse.Error, serverListResponse.Message));
                return;
            }
            for (int i = 0; i < serverListResponse.List.Count; i++)
            {
                var info = serverListResponse.List[i];
                Log.Debug(ZString.Format("{0}.{1}   {2}", info.Id, info.Name, (EServerState)info.State));
            }

            //获取用户信息
            var serverIndex = account.GetLongHashCode() % serverListResponse.List.Count;
            var server = serverListResponse.List[(int)serverIndex];
            using C2R_UserInfoRequest playerInfoRequest = C2R_UserInfoRequest.Create();
            {
                playerInfoRequest.ServerId = server.Id;
                playerInfoRequest.Account = account;
                playerInfoRequest.Token = token;
            }
            var playerInfoResponse = await clientSenderComponent.Call(playerInfoRequest) as R2C_UserInfoResponse;
            if (playerInfoResponse.Error != ErrorCode.ERR_Success)
            {
                Log.Error(ZString.Format("获取用户信息失败, {0}:{1}", serverListResponse.Error, serverListResponse.Message));
                return;
            }
            root.AddChild<UserInfo>().FromProto(playerInfoResponse.Info);

            //获取RealmKey
            C2R_RealmKeyRequest realmKeyRequest = C2R_RealmKeyRequest.Create();
            {
                realmKeyRequest.Account = account;
                realmKeyRequest.Token = token;
                realmKeyRequest.ServerId = server.Id;
            }
            var realmKeyResponse = await clientSenderComponent.Call(realmKeyRequest) as R2C_RealmKeyResponse;
            if (realmKeyResponse.Error != ErrorCode.ERR_Success)
            {
                Log.Error(ZString.Format("获取RealmKey失败, {0}:{1}", realmKeyResponse.Error, realmKeyResponse.Message));
                return;
            }

            //请求进入地图
            var enterGameResponse = await clientSenderComponent.EnterGameAsync(account, realmKeyResponse.Key, realmKeyResponse.GateAddress);
            if (enterGameResponse.Error != ErrorCode.ERR_Success)
            {
                Log.Error(ZString.Format("进入游戏失败, {0}:{1}", enterGameResponse.Error, enterGameResponse.Message));
                return;
            }

            Log.Debug("登录逻辑完成");

            await EventSystem.Instance.PublishAsync(root, new LoginFinish());
        }
    }
}