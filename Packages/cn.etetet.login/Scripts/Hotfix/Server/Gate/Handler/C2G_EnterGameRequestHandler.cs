using Cysharp.Text;

namespace ET.Server
{
    [MessageSessionHandler(SceneType.Gate)]
    public class C2G_EnterGameRequestHandler : MessageSessionHandler<C2G_EnterGameRequest, G2C_EnterGameResponse>
    {
        protected override async ETTask Run(Session session, C2G_EnterGameRequest request, G2C_EnterGameResponse response)
        {
            var locker = session.Lock(out var hadLocked);
            if (hadLocked)
            {
                RelesaseWithError(EErrorCode.请求过于频繁);
                return;
            }
            using var toRelease = locker;

            var sessionPlayerCmp = session.GetComponent<SessionPlayerComponent>();
            if (sessionPlayerCmp is null)
            {
                RelesaseWithError(EErrorCode.未连接网关);
                return;
            }

            var player = sessionPlayerCmp.Player;
            if (player is null || player.IsDisposed)
            {
                RelesaseWithError(EErrorCode.其他错误);
                return;
            }

            var scene = session.Root();
            var instanceId = session.InstanceId;
            using var coroutineLocker = await scene.GetComponent<CoroutineLockComponent>().Wait(CoroutineLockType.LoginGate, player.Account);

            if (instanceId != session.InstanceId || player.IsDisposed)
            {
                RelesaseWithError(EErrorCode.客户端连接发生变化);
                return;
            }

            //已经在其他客户端登录
            if (player.State == EPlayerState.在线)
            {
                try
                {
                    using var sessionLoginRequest = G2M_SessionLoginRequest.Create();
                    using var sessiongLoginResponse = await scene.GetComponent<MessageLocationSenderComponent>().Get(LocationType.Unit).Call(player.UnitId, sessionLoginRequest) as M2G_SessionLoginResponse;
                    if (sessiongLoginResponse.Error == ErrorCode.ERR_Success)
                    {
                        Log.Console("TODO: 二次登录逻辑, 补全下发切换场景消息");
                        return;
                    }

                    response.Error = sessiongLoginResponse.Error;
                    response.Message = sessiongLoginResponse.Message;
                    await DisconnectHelper.KickWithoutLock(player);
                    session?.DisconnectAsync().NoContext();
                }
                catch (System.Exception ex)
                {
                    var message = ZString.Format("二次登录失败\n{0}", ex);
                    response.Error = (int)EErrorCode.请求过于频繁;
                    response.Message = message;
                    await DisconnectHelper.KickWithoutLock(player);
                    session?.DisconnectAsync().NoContext();
                    throw;
                }
            }

            try
            {
                //在Gate上创建一个临时 MapScene 从而保证登录进图逻辑跟一般的进图逻辑相同
                var gateMapCmp = player.AddComponent<GateMapComponent>();
                gateMapCmp.Scene = await GateMapFactory.Create(gateMapCmp, player.Id, IdGenerater.Instance.GenerateInstanceId(), "GateMap");

                //这里可以从DB中加载Unit
                var unit = UnitFactory.Create(gateMapCmp.Scene, player.Id, UnitType.Player);
                var unitId = unit.Id;
                StartSceneConfig startSceneConfig = StartSceneConfigCategory.Instance.GetBySceneName(session.Zone(), "Map1");

                //等待一帧后再进行传送, 先让response返回, 否则传送消息可能比G2C_EnterMap还早
                TransferHelper.TransferAtFrameFinish(unit, startSceneConfig.ActorId, startSceneConfig.Name).NoContext();
                player.UnitId = unitId;
                response.MyUnitId = unitId;
                player.State = EPlayerState.在线;
            }
            catch (System.Exception ex)
            {
                response.Message = ZString.Format("角色进入游戏逻辑出现问题, 账号: {0}   角色Id: {1}   异常信息: {2}", player.Account, player.Id, ex);
                Log.Error(response.Message);
                response.Error = (int)EErrorCode.进入游戏失败;
                await DisconnectHelper.KickWithoutLock(player);
                session.DisconnectAsync().NoContext();
            }

            void RelesaseWithError(EErrorCode err)
            {
                response.Error = (int)err;
                response.Message = err.ToString();
                session.DisconnectAsync().NoContext();
            }
        }
    }
}