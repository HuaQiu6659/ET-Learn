/*
┌────────────────────────────┐
│　Description: 暂存服务器上的连接   账号作为索引
│　Remark: 
└────────────────────────────┘
*/
using System.Collections.Generic;

namespace ET.Server
{
    [EntitySystemOf(typeof(AccountSessionsComponent))]
    public static partial class AccountSessionsComponentSystem
    {
        [EntitySystem]
        private static void Awake(this AccountSessionsComponent self)
        {
            self.accountSessionMap = new Dictionary<string, EntityRef<Session>>();
        }

        [EntitySystem]
        private static void Destroy(this AccountSessionsComponent self)
        {
            self.accountSessionMap.Clear();
            self.accountSessionMap = null;
        }

        public static Session Get(this AccountSessionsComponent self, string account) 
        {
            self.accountSessionMap.TryGetValue(account, out var result);
            return result;
        }

        public static AccountSessionsComponent AddOrUpdate(this AccountSessionsComponent self, string account, Session session)
        {
            var map = self.accountSessionMap;
            if (map.ContainsKey(account))
                map[account] = session;
            else
                map.Add(account, session);

            return self;
        }

        public static AccountSessionsComponent Remove(this AccountSessionsComponent self, string account) 
        {
            self.accountSessionMap.Remove(account);
            return self;
        }
    }
}