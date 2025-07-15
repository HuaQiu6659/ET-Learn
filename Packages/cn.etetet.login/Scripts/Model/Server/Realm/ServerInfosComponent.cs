/*
┌────────────────────────────┐
│　Description: 记录其他服务器信息
│　Remark: 
└────────────────────────────┘
*/
using System.Collections.Generic;

namespace ET.Server
{
    [ComponentOf(typeof(Scene))]
    public class ServerInfosComponent : Entity, IAwake, IDestroy
    {
        public List<EntityRef<ServerInfo>> servers;
    }
}
