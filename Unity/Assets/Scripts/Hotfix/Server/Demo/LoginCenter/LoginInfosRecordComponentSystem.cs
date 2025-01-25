namespace ET.Server
{
    [EntitySystemOf(typeof(LoginInfosRecordComponent))]
    [FriendOfAttribute(typeof(ET.Server.LoginInfosRecordComponent))]
    public static partial class LoginInfosRecordComponentSystem
    {
        [EntitySystem]
        private static void Awake(this ET.Server.LoginInfosRecordComponent self)
        {

        }
        
        [EntitySystem]
        private static void Destroy(this ET.Server.LoginInfosRecordComponent self)
        {
            self.accountLoginInfosMap.Clear();
        }

        public static void Add(this LoginInfosRecordComponent self, long accountID, int serverID)
        {
            var map = self.accountLoginInfosMap;
            if (map.ContainsKey(accountID))
            {
                map[accountID] = serverID;
                return;
            }

            map.Add(accountID, serverID);
        }

        public static int Get(this LoginInfosRecordComponent self, long accountID)
        {
            if (self.accountLoginInfosMap.TryGetValue(accountID, out var result))
                return result;

            return -1;
        }

        public static bool Contains(this LoginInfosRecordComponent self, long accountID) => self.accountLoginInfosMap.ContainsKey(accountID);

        public static bool Remove(this LoginInfosRecordComponent self, long accountID) => self.accountLoginInfosMap.Remove(accountID);
    }
}
