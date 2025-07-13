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
        public bool isEamil;
        
        /// <summary> 验证码 </summary>
        public string code;
    }
}
