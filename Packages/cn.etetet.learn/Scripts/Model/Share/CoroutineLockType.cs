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
    }
}