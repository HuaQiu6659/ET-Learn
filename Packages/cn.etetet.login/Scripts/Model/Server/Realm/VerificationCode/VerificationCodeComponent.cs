/*
┌────────────────────────────┐
│　Description: 暂存邮箱获取的验证码
│　Remark: 
└────────────────────────────┘
*/
using System.Collections.Generic;

namespace ET.Server
{
    /// <summary>
    /// 验证码缓存, 验证码有效期为5分钟, 验证码使用后进行销毁
    /// </summary>
    [ComponentOf(typeof(Scene))]
    public class VerificationCodeComponent : Entity, IAwake
    {
        /// <summary> key: 邮箱/电话号码, Value: 验证码 </summary>
        public Dictionary<string, EntityRef<VerificationCode>> codeMap;
    }
}
