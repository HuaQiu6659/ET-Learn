/*
┌────────────────────────────┐
│　Description: 记录账号信息
│　Remark: 
└────────────────────────────┘
*/
namespace ET.Server
{
    [ChildOf]
    public class AccountInfo : Entity, IAwake
    {
        public EAccountState state;

        public string account;
        public string password;

        public long createTime;
        public long lastLoginTime;
    }
}
