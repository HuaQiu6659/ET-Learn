/*
┌────────────────────────────┐
│　Description: 暂存账号的连接
│　Remark: 
└────────────────────────────┘
*/
using System.Collections.Generic;

namespace ET.Server
{
    [ComponentOf(typeof(Scene))]
    public class AccountSessionsComponent : Entity, IAwake, IDestroy
    {
        /// <summary>
        /// Key: Account, Value: Session
        /// </summary>
        public Dictionary<string, EntityRef<Session>> accountSessionMap;
    }
}
