namespace ET.Server
{
    public static partial class DisconnectHelper
    {
        public static async ETTask KickAsync(Player player)
        {
            if (player is null || player.IsDisposed)
                return;

            long instanceId = player.InstanceId;
            using var locker = await player.Root().GetComponent<CoroutineLockComponent>().Wait(CoroutineLockType.LoginGate, player.Account.GetLongHashCode());
            //说明归属改变 or 已经被释放
            if (player.IsDisposed || player.InstanceId != instanceId)
                return;

            await KickWithoutLock(player);
        }

        public static async ETTask KickWithoutLock(Player player)
        {
            if (player is null || player.IsDisposed)
                return;

            using var toRelease = player;
            var scene = player.Root();
            var accountHash = player.Account.GetLongHashCode();
            if (player.State == EPlayerState.在线)
            {
                //通知游戏逻辑服下线Unit角色, 把数据缓存进数据库
                using var m2gExitGameReqest = await scene.GetComponent<MessageLocationSenderComponent>().Get(LocationType.Unit).Call(player.UnitId, G2M_ExitGameRequest.Create()) as M2G_ExitGameResponse;

                //通知移除账号角色登录信息
                using var g2lRemoveLoginRecordRequest = G2L_RemoveLoginRecordRequest.Create();
                {
                    g2lRemoveLoginRecordRequest.AccountHash = accountHash;
                    g2lRemoveLoginRecordRequest.GateId = player.Zone();
                }
                using var l2gRemoveLoginRecordResponse = await scene.GetComponent<MessageSender>().Call(StartSceneConfigCategory.Instance.LoginCenterConfig.ActorId, g2lRemoveLoginRecordRequest) as L2G_RemoveLoginRecordResponse;
            }

            player.State = EPlayerState.离线;

            await player.GetComponent<PlayerSessionComponent>().RemoveLocation(LocationType.GateSession);
            await player.RemoveLocation(LocationType.Player);   //Location是什么???
            scene.GetComponent<PlayerComponent>().Remove(player);

            var timerCmp = scene.GetComponent<TimerComponent>();
            await timerCmp.WaitAsync(300);
        }
    }
}
