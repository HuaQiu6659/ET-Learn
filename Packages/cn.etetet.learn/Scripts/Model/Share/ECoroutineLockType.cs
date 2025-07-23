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
        Login_账号验证 = PackageType.Login * 1000 + 1,
        Login_验证码,
        Login_账号信息,
        Login_登录中心服操作,
        Login_Realm令牌操作,
        Login_网关操作,

        DB = PackageType.DB * 1000 + 1,

        YooAssets = PackageType.YooAssets * 1000 + 1,
        YooAssets_资源加载,
    }
}