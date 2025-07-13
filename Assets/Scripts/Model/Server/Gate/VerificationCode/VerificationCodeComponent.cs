/*
┌────────────────────────────┐
│　Description: 暂存邮箱获取的验证码
│　Remark: 
└────────────────────────────┘
*/
using System.Collections.Generic;

namespace ET.Server
{
    [ComponentOf(typeof(Scene))]
    public class VerificationCodeComponent : Entity, IAwake
    {
        /// <summary> key: 邮箱, Value: 验证码 </summary>
        public Dictionary<string, EntityRef<VerificationCode>> emailCodes;
        
        /// <summary> key: 手机号, Value: 验证码 </summary>
        public Dictionary<string, EntityRef<VerificationCode>> phoneCodes;
    }
}
