/*
┌────────────────────────────┐
│　Description: 
│　Remark: 
└────────────────────────────┘
*/
namespace ET
{
    [EntitySystemOf(typeof(ServerInfo))]
    public static partial class ServerInfoSystem
    {
        [EntitySystem]
        private static void Awake(this ET.ServerInfo self)
        {

        }  

        public static ServerInfoProto ToProto(this ET.ServerInfo self)
        {
            var proto = ServerInfoProto.Create();
            {
                proto.Id = self.Id;
                proto.Name = self.name;
                proto.State = (int)self.state;
            }
            return proto;
        }

        public static ServerInfo FromProto(this ServerInfo self, ServerInfoProto proto)
        {
            self.name = proto.Name;
            self.state = (EServerState)proto.State;
            return self;
        }
    }
}