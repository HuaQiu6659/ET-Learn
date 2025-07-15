/*
┌────────────────────────────┐
│　Description: 
│　Remark: 
└────────────────────────────┘
*/
namespace ET
{
    public static partial class CoroutineLockType
    {
        public const int LoginAccount = PackageType.Login * 1000 + 1;   //登录
        public const int VerificationCode = LoginAccount + 1;           //验证码
        public const int UserInfo = LoginAccount + 2;                   //角色信息
        public const int LoginCenter = LoginAccount + 3;                //登录中心服验证
        public const int RealmKey = LoginAccount + 4;
        public const int LoginGate = LoginAccount + 5;
    }
}