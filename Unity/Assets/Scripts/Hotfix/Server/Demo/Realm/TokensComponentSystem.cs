namespace ET.Server
{
    [EntitySystemOf(typeof(TokensComponent))]
    [FriendOfAttribute(typeof(ET.Server.TokensComponent))]
    public static partial class TokensComponentSystem
    {
        [EntitySystem]
        private static void Awake(this ET.Server.TokensComponent self)
        {

        }

        [EntitySystem]
        private static void Destroy(this ET.Server.TokensComponent self) => self.accountTokens.Clear();

        public static void Add(this TokensComponent self, string account, string token)
        {
            if (self.accountTokens.ContainsKey(account))
                self.accountTokens[account] = token;
            else
                self.accountTokens.Add(account, token);
            
            self.TimeOutRemoveKey(account, token).Coroutine();
        }

        public static bool Remove(this TokensComponent self, string account) => self.accountTokens.Remove(account);

        public static string Get(this TokensComponent self, string account)
        {
            self.accountTokens.TryGetValue(account, out var token);
            return token;
        }

        private static async ETTask TimeOutRemoveKey(this TokensComponent self, string key, string token)
        {
            await self.Root().GetComponent<TimerComponent>().WaitAsync(60 * 1000);
            string onlineToken = self.Get(key);
            if (token == onlineToken)
                self.Remove(key);
        }
    }
}