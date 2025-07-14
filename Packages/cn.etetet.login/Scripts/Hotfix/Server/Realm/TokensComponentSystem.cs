/*
┌────────────────────────────┐
│　Description: 
│　Remark: 
└────────────────────────────┘
*/
using Cysharp.Text;

namespace ET.Server
{
    [EntitySystemOf(typeof(TokensComponent))]
    public static partial class TokensComponentSystem
    {
        [EntitySystem]
        private static void Awake(this ET.Server.TokensComponent self)
        {
            self.tokensMap = new System.Collections.Generic.Dictionary<string, string>();
        }

        [EntitySystem]
        private static void Destroy(this ET.Server.TokensComponent self)
        {
            self.tokensMap.Clear();
            self.tokensMap = null;
        }

        public static TokensComponent AddOrUpdate(this TokensComponent self, string account, string token)
        {
            var map = self.tokensMap;
            if (map.ContainsKey(account))
                map[account] = token;
            else 
                map.Add(account, token);
            return self;
        }

        public static TokensComponent Remove(this  TokensComponent self, string account)
        {
            self.tokensMap.Remove(account);
            return self;
        }

        public static string Get(this TokensComponent self, string account)
        {
            self.tokensMap.TryGetValue(account, out var token);
            return token;
        }

        public static string CreateToken(this TokensComponent self) => ZString.Concat(TimeInfo.Instance.ServerNow(), RandomGenerator.RandomNumber(int.MinValue, int.MaxValue));
    }
}