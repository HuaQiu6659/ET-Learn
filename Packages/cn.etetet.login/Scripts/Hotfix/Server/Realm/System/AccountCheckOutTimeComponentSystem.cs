/*
┌────────────────────────────┐
│　Description: 这里只写了长时间无操作时直接断连, 但没有写用户操作后对组件的移出or刷新计时器
│　Remark: 
└────────────────────────────┘
*/
namespace ET.Server
{
    [Invoke(TimerInvokeType.AccountSessionCheckTimeOut)]
    public class AccountSessionCheckOutTimer : ATimer<AccountCheckOutTimeComponent>
    {
        //超时自释放
        protected override void Run(AccountCheckOutTimeComponent t)
        {
            t?.DeleteSession();
        }
    }

    [EntitySystemOf(typeof(AccountCheckOutTimeComponent))]
    public static partial class AccountCheckOutTimeComponentSystem
    {
        [EntitySystem]
        private static void Destroy(this ET.Server.AccountCheckOutTimeComponent self)
        {

        }

        [EntitySystem]
        private static void Awake(this ET.Server.AccountCheckOutTimeComponent self, string account)
        {
            self.account = account;

            var timerCmp = self.Root().GetComponent<TimerComponent>();
            timerCmp.Remove(ref self.timer);
            self.timer = timerCmp.NewOnceTimer(TimeInfo.Instance.ServerNow() + 60_000, TimerInvokeType.AccountSessionCheckTimeOut, self);
        }

        public static void DeleteSession(this AccountCheckOutTimeComponent self)
        {
            var scene = self.Root();
            var accountSessionsCmp = scene.GetComponent<AccountSessionsComponent>();
            var session = self.GetParent<Session>();
            var originSession = accountSessionsCmp.Get(self.account);

            //可能在这期间出现连接替换? 所以才需要判断连接是否还相同
            if (originSession != null && originSession.InstanceId == session.InstanceId)
                accountSessionsCmp.Remove(self.account);

            var disconnect = A2C_Disconnect.Create();
            {
                disconnect.Error = (int)EDisconnectType.长时间无操作;
            }
            session?.Send(disconnect);
            session?.DisconnectAsync().NoContext();
        }
    }
}