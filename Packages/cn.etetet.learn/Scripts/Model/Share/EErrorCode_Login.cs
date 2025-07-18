/*
┌────────────────────────────┐
│　Description: 
│　Remark: 
└────────────────────────────┘
*/
namespace ET
{
    public enum EErrorCode_Login
    {
        成功 = 0,

        网关令牌错误 = ErrorCode.ERR_ConnectGateKeyError,
        其他错误,

        未连接网关,

        客户端连接发生变化,

        账号不合法,

        账号不存在,
        密码错误,
        未设置密码,

        账号已被封禁,

        请求过于频繁,
        
        验证码发送失败,
        验证码错误,

        令牌验证失败,

        账号在其他客户端登录, 

        进入游戏失败
    }
}