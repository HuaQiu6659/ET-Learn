namespace ET
{
    [EntitySystemOf(typeof(ServerInfo))]
    [FriendOfAttribute(typeof(ET.ServerInfo))]
    public static partial class ServerInfoSystem
    {
        [EntitySystem]
        private static void Awake(this ET.ServerInfo self)
        {

        }

        public static void FromMessage(this ServerInfo self, ServerInfoProto proto)
        {
            self.serverName = proto.serverName;
            self.state = (EServerState)proto.state;
        }

        public static ServerInfoProto ToMessage(this ServerInfo self)
        {
            ServerInfoProto result = ServerInfoProto.Create();
            result.state = (int)self.state;
            result.serverName = self.serverName;
            return result;
        }
    }
}
