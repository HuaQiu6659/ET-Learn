/*
┌────────────────────────────┐
│　Description: 
│　Remark: 
└────────────────────────────┘
*/
namespace ET.Server
{
    [EntitySystemOf(typeof(ServerInfosComponent))]
    [FriendOfAttribute(typeof(ET.ServerInfo))]
    public static partial class ServerInfosComponentSystem
    {
        [EntitySystem]
        private static void Destroy(this ServerInfosComponent self)
        {
            self.servers.Clear();
            self.servers = null;

        }

        [EntitySystem]
        private static void Awake(this ServerInfosComponent self)
        {
            self.servers = new System.Collections.Generic.List<EntityRef<ServerInfo>>();
            self.Load();
        }

        public static ServerInfosComponent Load(this ServerInfosComponent self)
        {
            foreach (var item in self.servers)
                item.Entity?.Dispose();

            self.servers.Clear();
            var serverInfoConfigs = StartZoneConfigCategory.Instance.GetAll();
            foreach (var config in serverInfoConfigs)
            {

                var info = config.Value;
                if (info.ZoneType != 1)
                    continue;

                ServerInfo server = self.AddChildWithId<ServerInfo>(config.Value.Id);
                {
                    server.name = info.DBName;
                    server.state = EServerState.空闲;
                }
                self.servers.Add(server);
            }
            return self;
        }
    }
}