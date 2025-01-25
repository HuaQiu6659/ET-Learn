namespace ET.Server
{
    [EntitySystemOf(typeof(ServerInfosManagerComponent))]
    [FriendOfAttribute(typeof(ET.Server.ServerInfosManagerComponent))]
    [FriendOfAttribute(typeof(ET.ServerInfo))]
    public static partial class ServerInfosManagerComponentSystem
    {
        [EntitySystem]
        private static void Awake(this ET.Server.ServerInfosManagerComponent self)
        {
            self.Load();
        }

        [EntitySystem]
        private static void Destroy(this ET.Server.ServerInfosManagerComponent self)
        {
            foreach (ServerInfo info in self.serverInfos)
                info?.Dispose();

            self.serverInfos.Clear();
        }

        public static void Load(this ServerInfosManagerComponent self)
        {
            self.Destroy();

            var serverInfoConfigs = StartZoneConfigCategory.Instance.GetAll();
            foreach (var info in serverInfoConfigs.Values)
            {
                if (info.ZoneType != 1)
                    continue;

                ServerInfo toAdd = self.AddChildWithId<ServerInfo>(info.Id);
                toAdd.serverName = info.DBName;
                toAdd.state = EServerState.Normal;
                self.serverInfos.Add(toAdd);
            }
        }
    }
}