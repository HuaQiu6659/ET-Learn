namespace ET.Server
{
    [EntitySystemOf(typeof(AccountSessionsComponent))]
    [FriendOf(typeof(AccountSessionsComponent))]
    public static partial class AccountSessionsComponentSystem
    {
        [EntitySystem]
        private static void Awake(this ET.Server.AccountSessionsComponent self)
        {

        }
        
        [EntitySystem]
        private static void Destroy(this ET.Server.AccountSessionsComponent self)
        {
            self.accountSessionDic.Clear();
        }

        public static Session Get(this AccountSessionsComponent self, string account)
        {
            self.accountSessionDic.TryGetValue(account, out var result);
            return result;
        }

        public static void Add(this AccountSessionsComponent self, string key, Session value)
        {
            if (self.accountSessionDic.ContainsKey(key))
                self.accountSessionDic[key] = value;
            else
                self.accountSessionDic.Add(key, value);
        }

        public static bool Remove(this AccountSessionsComponent self, string key) => self.accountSessionDic.Remove(key);
    }
}
