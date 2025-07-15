/*
┌────────────────────────────┐
│　Description: 
│　Remark: 
└────────────────────────────┘
*/
namespace ET.Server
{
    [EntitySystemOf(typeof(LoginInfoRecorderComponent))]
    public static partial class LoginInfoRecorderComponentSystem
    {
        [EntitySystem]
        private static void Awake(this ET.Server.LoginInfoRecorderComponent self)
        {
            self.accountInfosMap = new System.Collections.Generic.Dictionary<long, int>();
        }

        [EntitySystem]
        private static void Destroy(this ET.Server.LoginInfoRecorderComponent self)
        {
            self.accountInfosMap.Clear();
            self.accountInfosMap = null;
        }

        public static LoginInfoRecorderComponent AddOrUpdate(this LoginInfoRecorderComponent self, long accountHash, int channelId)
        {
            var map = self.accountInfosMap;
            var key = accountHash;
            if (map.ContainsKey(key))
                map[key] = channelId;
            else
                map.Add(key, channelId);
            return self;
        }

        public static bool IsLogined(this LoginInfoRecorderComponent self, long accountHash)
        {
            return self.accountInfosMap.ContainsKey(accountHash);
        }

        public static LoginInfoRecorderComponent Remove(this LoginInfoRecorderComponent self, long accountHash)
        {
            self.accountInfosMap.Remove(accountHash);
            return self;
        }

        public static int Get(this LoginInfoRecorderComponent self, long accountHash)
        {
            if (self.accountInfosMap.TryGetValue(accountHash, out var result))
                return result;

            return -1;
        }
    }
}