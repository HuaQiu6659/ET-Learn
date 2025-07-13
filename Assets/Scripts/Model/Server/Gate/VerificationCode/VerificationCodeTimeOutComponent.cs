/*
┌────────────────────────────┐
│　Description: 验证码有效期是5分钟, 五分钟后当前账号的验证码失效
│　Remark: 
└────────────────────────────┘
*/
namespace ET.Server
{
    /// <summary>
    /// 验证码有效期是5分钟, 五分钟后当前账号的验证码失效
    /// </summary>
    [ComponentOf(typeof(VerificationCode))]
    public class VerificationCodeTimeOutComponent : Entity, IAwake, IDestroy
    {
        public long timer;
    }   
}
