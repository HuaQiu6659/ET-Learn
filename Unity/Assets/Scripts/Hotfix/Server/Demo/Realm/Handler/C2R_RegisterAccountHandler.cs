using System.Text.RegularExpressions;

namespace ET.Server
{
    [MessageSessionHandler(SceneType.Realm)]
    [FriendOf(typeof(AccountInfo))]
    public class C2R_RegisterAccountHandler: MessageSessionHandler<C2R_RegisterAccountRequest, R2C_RegisterAccountResponse>
    {
        protected override async ETTask Run(Session session, C2R_RegisterAccountRequest request, R2C_RegisterAccountResponse response)
        {
            //记得去除TimeoutComponent 不然5秒后连接就自动断开了
            session.RemoveComponent<SessionAcceptTimeoutComponent>();
            
            //短时间内重复请求
            if (session.GetComponent<SessionLockingComponent>() is not null)
            {
                response.Error = (int)EErrorCode.ERR_RequestRepeated;
                session.DisconnectAsync().Coroutine();
                return;
            }
            
            if (string.IsNullOrEmpty(request.Account) || string.IsNullOrEmpty(request.Password))
            {
                response.Error = (int)EErrorCode.ERR_Account_InputEmpty;
                session.DisconnectAsync().Coroutine();
                return;
            }

            //限制6~15位长度的数字字母组合
            if (Regex.IsMatch(request.Account.Trim(), @"^[a-zA-Z0-9]{6,15}$"))
            {
                response.Error = (int)EErrorCode.ERR_Account_InillegalAcount;
                session.DisconnectAsync().Coroutine();
                return;
            }

            //限制8~30位长度的数字字母组合
            if (Regex.IsMatch(request.Password.Trim(), @"^[a-zA-Z0-9]{8,30}$"))
            {
                response.Error = (int)EErrorCode.ERR_Account_InillegalPassword;
                session.DisconnectAsync().Coroutine();
                return;
            }

            //协程锁保证同一账号登录的先后性
            var coroutineLocker = session.Root().GetComponent<CoroutineLockComponent>();
            //避免短时间内重复请求
            using (session.AddComponent<SessionLockingComponent>())
            {
                using (await coroutineLocker.Wait(CoroutineLockType.Login, request.Account.GetLongHashCode()))
                {
                    //获取数据库
                    var dbComponent = session.Root().GetComponent<DBManagerComponent>().GetZoneDB(session.Zone());
                    var accountInfos = await dbComponent.Query<AccountInfo>(a => a.account == request.Account);
                    //已注册
                    if (accountInfos is not null && accountInfos.Count > 0)
                    {
                        response.Error = (int)EErrorCode.ERR_Account_RegisterRepeated;
                        session.DisconnectAsync().Coroutine();
                        return;
                    }

                    //账号注册
                    var infosCmp = session.AddOrGetComponent<AccountInfosComponent>();
                    var newAccount = infosCmp.AddChild<AccountInfo>();
                    {
                        newAccount.account = request.Account.Trim();
                        newAccount.password = request.Account.Trim();
                        newAccount.createTime = TimeInfo.Instance.ServerNow();
                        newAccount.accountType = EAccountType.General;
                        await dbComponent.Save(newAccount);
                    }
                }
            }
            
            await ETTask.CompletedTask;
        }
    }
}
