using System.Collections.Generic;

namespace ET.Server
{
    [ComponentOf(typeof(Scene))]
    public class ServerInfosManagerComponent : Entity, IAwake, IDestroy
    {
        public List<EntityRef<ServerInfo>> serverInfos = new List<EntityRef<ServerInfo>>();
    }
}