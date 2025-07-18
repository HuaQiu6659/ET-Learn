/*
┌────────────────────────────┐
│　Description: 
│　Remark: 
└────────────────────────────┘
*/
namespace ET.Server
{
    [ChildOf(typeof(VerificationCodeComponent))]
    public class VerificationCode : Entity, IAwake, IDestroy
    {
        /// <summary> email or phone </summary>
        public string owner;
        
        /// <summary> 验证码 </summary>
        public string code;

        public long CodeHash => code.GetLongHashCode();

        /// <summary> 避免短时间内重复发送 </summary>
        public long lastSendTime;
    }
}
