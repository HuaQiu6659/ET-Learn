using System.Collections.Generic;

namespace ET.Server
{
    [ComponentOf(typeof(Scene))]
    public class AccountSessionsComponent : Entity, IAwake, IDestroy
    {
        public Dictionary<string, EntityRef<Session>> accountSessionDic = new Dictionary<string, EntityRef<Session>>();
    }
}
