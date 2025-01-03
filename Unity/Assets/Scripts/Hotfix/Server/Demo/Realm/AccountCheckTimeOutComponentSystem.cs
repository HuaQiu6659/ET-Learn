namespace ET.Server
{
    [Invoke(TimerInvokeType.AccountSessionCheckOutTime)]
    public class AccountSessionCheckOutTimer : ATimer<AccountCheckTimeOutComponent>
    {
        protected override void Run(AccountCheckTimeOutComponent t)
        {
            t?.DeleteSession();
        }
    }
    
    [EntitySystemOf(typeof(AccountCheckTimeOutComponent))]
    [FriendOf(typeof(ET.Server.AccountCheckTimeOutComponent))]
    public static partial class AccountCheckTimeOutComponentSystem
    {
        [EntitySystem]
        private static void Awake(this ET.Server.AccountCheckTimeOutComponent self, string account)
        {
            self.account = account;
            
            var timerCmp = self.Root().GetComponent<TimerComponent>();
            {
                timerCmp.Remove(ref self.timerID);
                //10分钟后执行，一次性的延时任务
                self.timerID = timerCmp.NewOnceTimer(TimeInfo.Instance.ServerNow() + 600_000, TimerInvokeType.AccountSessionCheckOutTime, self);
            }
        }
        
        [EntitySystem]
        private static void Destroy(this ET.Server.AccountCheckTimeOutComponent self)
        {
            self.Root().GetComponent<TimerComponent>().Remove(ref self.timerID);
        }

        public static void DeleteSession(this AccountCheckTimeOutComponent self)
        {
            var session = self.GetParent<Session>();
            var originSession = session.Root().GetComponent<AccountSessionsComponent>().Get(self.account);
            if (originSession is not null && session.InstanceId == originSession.InstanceId)
            {
                session.Root().GetComponent<AccountSessionsComponent>().Remove(self.account);
            }
            
            A2C_Disconnect disconnect = A2C_Disconnect.Create();
            disconnect.Error = 1;
            session?.Send(disconnect);
            session?.DisconnectAsync().Coroutine();
        }
    }
}
