using System;
using System.Net;

namespace ET.Server
{
    [MessageSessionHandler(SceneType.Realm)]
    [FriendOf(typeof(AccountInfo))]
    public class C2R_LoginHandler : MessageSessionHandler<C2R_Login, R2C_Login>
    {
        protected override async ETTask Run(Session session, C2R_Login request, R2C_Login response)
        {
            // TODO:判断账号密码是否合法(长度, 非法字符)
            if (string.IsNullOrEmpty(request.Account))
            {
                RelesaseWithError(EErrCode_Login.账号不合法);
                return;
            }
            if (string.IsNullOrEmpty(request.Password))
            {
                RelesaseWithError(EErrCode_Login.密码不合法);
                return;
            }

            //数据库操作应该使用协程锁
            using (await session.Root().GetComponent<CoroutineLockComponent>().Wait(CoroutineLockType.LoginAccount, request.Account.GetLongHashCode()))
            {
                // 根据配置表 zone 获取对应的数据库
                var dbMgr = session.Root().GetComponent<DBManagerComponent>().GetZoneDB(session.Zone());
                var infos = await dbMgr.Query<AccountInfo>(info => info.account == request.Account);
                // 数据库中没有对应账号, 自动注册
                if (infos.Count == 0)
                {
                    //TODO:应该返回账号未注册
                    /*
                    RelesaseWithError(EErrCode_Login.账号不存在);
                    return;
                     */
                    var infosMgr = session.GetOrAddComponent<AccountInfosComponent>();

                    var newAccount = infosMgr.AddChild<AccountInfo>();
                    {
                        newAccount.account = request.Account;
                        newAccount.password = request.Password;
                        newAccount.state = EAccountState.用户;
                        newAccount.createTime = TimeInfo.Instance.ServerNow();
                    }

                    await dbMgr.Save(newAccount);
                }
                else
                {
                    var infoInDb = infos[0];
                    if (infoInDb.password != request.Password)
                    {
                        RelesaseWithError(EErrCode_Login.密码错误);
                        return;
                    }

                    if (infoInDb.state == EAccountState.封禁)
                    {
                        RelesaseWithError(EErrCode_Login.账号已被封禁);
                        return;
                    }
                }
            }

            const int UserZone = 3; // 这里一般会有创角，选择区服，demo就不做这个操作了，直接放在3区

            // 随机分配一个Gate
            StartSceneConfig config = RealmGateAddressHelper.GetGate(UserZone, request.Account);
            Log.Debug($"gate address: {config}");

            // 向gate请求一个key, 客户端可以拿着这个key连接gate
            R2G_GetLoginKey r2GGetLoginKey = R2G_GetLoginKey.Create();
            r2GGetLoginKey.Account = request.Account;
            G2R_GetLoginKey g2RGetLoginKey = (G2R_GetLoginKey)await session.Fiber().Root.GetComponent<MessageSender>().Call(config.ActorId, r2GGetLoginKey);

            response.Address = config.InnerIPPort.ToString();
            response.Key = g2RGetLoginKey.Key;
            response.GateId = g2RGetLoginKey.GateId;

            //保证通信完成, 1秒后彻底释放
            session.Release(1000).NoContext();

            void RelesaseWithError(EErrCode_Login err)
            {
                response.Error = (int)err;
                response.Message = err.ToString();
                session.Release(1000).NoContext();
            }
        }
    }
}
