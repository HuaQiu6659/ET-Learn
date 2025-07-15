using Cysharp.Text;

namespace ET.Server
{
    //Description: 用户信息获取请求处理
    [MessageSessionHandler(SceneType.Realm)]
    [FriendOfAttribute(typeof(ET.UserInfo))]
    public class C2R_UserInfoRequestHandler : MessageSessionHandler<C2R_UserInfoRequest, R2C_UserInfoResponse>
    {
        protected override async ETTask Run(Session session, C2R_UserInfoRequest request, R2C_UserInfoResponse response)
        {
            //请求锁
            var locker = session.Lock(out var hasLocked);
            if (hasLocked)
            {
                RelesaseWithError(EErrorCode_Login.请求过于频繁);
                return;
            }
            using var toRelease = locker;
            
            var scene = session.Root();
            var tokenPassed = scene.GetComponent<TokensComponent>().Verificate(request.Account, request.Token);
            if (!tokenPassed)
            {
                RelesaseWithError(EErrorCode_Login.网关令牌错误);
                return;
            }

            //协程锁
            using var coroutineLocker = await scene.GetComponent<CoroutineLockComponent>().Wait(CoroutineLockType.UserInfo, request.Account);
            var dbCmp = scene.GetComponent<DBManagerComponent>().GetZoneDB(session.Zone());
            var infos = await dbCmp.Query<UserInfo>(c => c.serverId == request.ServerId && c.account == request.Account);
            UserInfo userInfo;
            if (infos.Count == 0)
            {
                //当前直接视为只有一个角色 直接选择该角色, 没有角色时自动创建
                userInfo = session.AddChild<UserInfo>();
                {
                    userInfo.account = request.Account;
                    userInfo.serverId = (int)request.ServerId;
                    userInfo.name = ZString.Concat("游客", TimeInfo.Instance.ServerNow());
                }
                await dbCmp.Save(userInfo);
            }
            else
            {
                userInfo = infos[0];
                session.AddChild(userInfo);
            }

            response.Info = userInfo.ToProto();

            void RelesaseWithError(EErrorCode_Login err)
            {
                response.Error = (int)err;
                response.Message = err.ToString();
                session.DisconnectAsync().NoContext();
            }
        }
    }
}
