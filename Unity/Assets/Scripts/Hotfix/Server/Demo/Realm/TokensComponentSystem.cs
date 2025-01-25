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

        /// <summary>
        /// 检测账号和token是否相符
        /// </summary>
        /// <param name="self"></param>
        /// <param name="account">要检测的账号</param>
        /// <param name="token">客户端传来的token</param>
        /// <param name="session">客户端链接</param>
        /// <param name="response">响应，不相符时response.Error会赋值EErrorCode.ERR_TokenError</param>
        /// <param name="disconnectWithError">True：不相符时断开客户端链接</param>
        /// <returns>true：相符 False：不相符</returns>
        public static bool CheckToken(this TokensComponent self, string account, string token, Session session, IResponse response, bool disconnectWithError = true)
        {
            string tokenInServer = self.Get(account);
            bool isLegal = tokenInServer != null && tokenInServer == token;
            if (!isLegal)
            {
                response.Error = (int)EErrorCode.ERR_TokenError;
                if (disconnectWithError)
                    session.DisconnectAsync().Coroutine();
            }
            return isLegal;
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