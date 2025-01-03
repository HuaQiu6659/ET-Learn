using System.ComponentModel;

namespace ET
{
    public static partial class ErrorCode
    {
        public const int ERR_Success = 0;

        // 1-11004 是SocketError请看SocketError定义
        //-----------------------------------
        // 100000-109999是Core层的错误
        
        // 110000以下的错误请看ErrorCore.cs
        
        // 这里配置逻辑层的错误码
        // 110000 - 200000是抛异常的错误
        // 200001以上不抛异常
    }

    public enum EErrorCode
    {
        [Description("短时间内重复请求")]
        ERR_RequestRepeated = 200001,
        
        //*********** 账号操作 ***********
        [Description("账号或密码输入为空")]
        ERR_Account_InputEmpty,
        
        [Description("密码错误")]
        ERR_Account_WrongPassword,
        
        [Description("账号不合规")]
        ERR_Account_InillegalAcount,
        
        [Description("密码不合规")]
        ERR_Account_InillegalPassword,
        
        [Description("账号未注册")]
        ERR_Account_AccountUnregistered,
        
        [Description("账号被封禁")]
        ERR_Account_BlackList,
        
        [Description("账号已存在")]
        ERR_Account_RegisterRepeated,
    }
}