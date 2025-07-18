/*
┌────────────────────────────┐
│　Description: 
│　Remark: 
└────────────────────────────┘
*/
namespace ET
{
    public enum ECoroutineLockType
    {
        账号验证 = PackageType.Login * 1000 + 1,
        验证码,
        账号信息,

        登录中心服操作,
        Realm令牌操作,
        网关操作,

        数据缓存操作,
    }
}