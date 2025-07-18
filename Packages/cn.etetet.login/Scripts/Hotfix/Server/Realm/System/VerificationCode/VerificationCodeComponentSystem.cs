/*
┌────────────────────────────┐
│　Description: 验证码的 获取, 验证, 注销
│　Remark: 
└────────────────────────────┘
*/
using System.Collections.Generic;

namespace ET.Server
{
    [EntitySystemOf(typeof(VerificationCodeComponent))]
    [FriendOf(typeof(VerificationCode))]
    public static partial class VerificationCodeComponentSystem
    {
        [EntitySystem]
        private static void Awake(this VerificationCodeComponent self)
        {
            self.codeMap = new Dictionary<string, EntityRef<VerificationCode>>();
        }

        /// <param name="self"></param>
        /// <param name="owner">邮箱/电话号码</param>
        /// <returns></returns>
        public static VerificationCode GetVerificationCode(this VerificationCodeComponent self, string owner)
        {
            if (self.codeMap.TryGetValue(owner, out EntityRef<VerificationCode> code))
                return code.Entity;

            var codeEntity = self.AddChild<VerificationCode>();
            {
                self.codeMap.Add(owner, codeEntity);
                codeEntity.code = self.RandomCode();
                codeEntity.owner = owner;
            }
            return codeEntity;
        }

        /// <param name="owner">邮箱/电话号码</param>
        public static async ETTask<VerificationCodeComponent> RemoveCodeAsync(this VerificationCodeComponent self, string owner)
        {
            using (await self.Root().GetComponent<CoroutineLockComponent>().Wait(CoroutineLockType.VerificationCode, owner.GetLongHashCode()))
                self.codeMap.Remove(owner);

            return self;
        }

        /// <param name="owner">邮箱/电话号码</param>
        /// <param name="code">验证码</param>
        public static async ETTask<bool> Verificate(this VerificationCodeComponent self, string owner, long code)
        {
            using (await self.Root().GetComponent<CoroutineLockComponent>().Wait(CoroutineLockType.VerificationCode, owner.GetLongHashCode()))
            {
                if (self.codeMap.TryGetValue(owner, out var codeInMap))
                {
                    var result = codeInMap.Entity.CodeHash == code;

                    //验证成功则直接销毁验证码
                    if (result)
                        codeInMap.Entity.Dispose();

                    return true;
                }

                return false;
            }
        }

        private static string RandomCode(this VerificationCodeComponent self)
        {
            var code = RandomGenerator.RandomNumber(0,999999);
            return code.ToString().PadLeft(6, '0'); // 不足6位时用0补足
        }
    }
}