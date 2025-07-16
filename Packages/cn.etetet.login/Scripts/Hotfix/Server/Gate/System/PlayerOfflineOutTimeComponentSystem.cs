/*
┌────────────────────────────┐
│　Description: 
│　Remark: 
└────────────────────────────┘
*/
namespace ET.Server
{
    [EntitySystemOf(typeof(PlayerOfflineOutTimeComponent))]
    public static partial class PlayerOfflineOutTimeComponentSystem
    {
        [EntitySystem]
        private static void Awake(this ET.Server.PlayerOfflineOutTimeComponent self)
        {
            self.Root().GetComponent<TimerComponent>().NewOnceTimer(TimeInfo.Instance.ServerNow()+ 10_000, TimerInvokeType.PlayerOfflineTimeOut, self);
        }

        [EntitySystem]
        private static void Destroy(this ET.Server.PlayerOfflineOutTimeComponent self)
        {
            self.Root().GetComponent<TimerComponent>().Remove(ref self.timer);
        }

        //下线后这个组件也会被释放掉
        public static void Kick(this PlayerOfflineOutTimeComponent self)
        {
            DisconnectHelper.KickAsync(self.GetParent<Player>()).NoContext();
        }
    }

    [Invoke(TimerInvokeType.PlayerOfflineTimeOut)]
    public class PlayerOfflineOutTimer : ATimer<PlayerOfflineOutTimeComponent>
    {
        protected override void Run(PlayerOfflineOutTimeComponent t)
        {
            t?.Kick();
        }
    }
}