/*
┌────────────────────────────┐
│　Description: 
│　Remark: 
└────────────────────────────┘
*/
namespace ET.Server
{
    public static partial class DisconnectHelper
    {
        public static async ETTask KickPlayer(Player player)
        {
            if (player is null || player.IsDisposed)
                return;

            var instanceId = player.InstanceId;
            using var locker = await player.Root().GetComponent<CoroutineLockComponent>().Wait(CoroutineLockType.LoginGate, player.Account.GetLongHashCode());

            //此时player所属客户端已经切换
            if (player.IsDisposed || instanceId != player.InstanceId)
                return;

            await KickPlayerWithoutLock(player);
        }

        public static async ETTask KickPlayerWithoutLock(Player player)
        {
            if (player is null || player.IsDisposed)
                return;

            using var toRelease = player;
            var scene = player.Root();
            if (player.State == EPlayerState.在线)
            {
                //通知在线的客户端下线, 并把数据存入数据库
                var m2gExitGameResponse = await scene.GetComponent<MessageLocationSenderComponent>().Get(LocationType.Unit).Call(player.UnitId, G2M_ExitGameRequest.Create()) as M2G_ExitGameResponse;
                G2L_RemoveLoginRecordRequest removeLoginRecordRequest = G2L_RemoveLoginRecordRequest.Create();
                {
                    removeLoginRecordRequest.AccountHash = player.Account.GetLongHashCode();
                    removeLoginRecordRequest.GateId = player.Zone();
                }
                var removeLoginRecordResponse = await scene.GetComponent<MessageSender>().Call(StartSceneConfigCategory.Instance.LoginCenterConfig.ActorId, removeLoginRecordRequest) as L2G_AddLoginRecordResponse;

            }

            var timer = scene.GetComponent<TimerComponent>();
            player.State = EPlayerState.离线;
            await player.GetComponent<PlayerSessionComponent>().RemoveLocation(LocationType.GateSession);
            await player.RemoveLocation(LocationType.Player);
            scene.GetComponent<PlayerComponent>().Remove(player);
        }
    }
}