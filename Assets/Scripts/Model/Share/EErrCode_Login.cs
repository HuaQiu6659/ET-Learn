/*
┌────────────────────────────┐
│　Description: 
│　Remark: 
└────────────────────────────┘
*/
namespace ET
{
    public enum EErrCode_Login
    {
        成功 = 0,

        网关令牌错误 = ErrorCode.ERR_ConnectGateKeyError,

        账号不合法,
        密码不合法,

        账号不存在,
        密码错误,

        账号已被封禁
    }
}