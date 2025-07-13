/*
┌────────────────────────────┐
│　Description: 
│　Remark: 
└────────────────────────────┘
*/

using System;
using System.Collections.Generic;
using ET.Server;

namespace ET.Server
{
    [EntitySystemOf(typeof(VerificationCodeComponent))]
    [FriendOfAttribute(typeof(ET.Server.VerificationCode))]
    public static partial class VerificationCodeComponentSystem
    {
        [EntitySystem]
        private static void Awake(this ET.Server.VerificationCodeComponent self)
        {
            self.emailCodes = new Dictionary<string, EntityRef<VerificationCode>>();
            self.phoneCodes = new Dictionary<string, EntityRef<VerificationCode>>();

            //TODO:1.读取配置表中的短信SDK鉴权码, 邮箱鉴权码(配置表还没弄, 这条先不管)
            //2.
        }

        //TODO:1.检测缓存中是否存在邮箱
        //2.
        public static string GetVerificationCodeByEmail(this VerificationCodeComponent self, string email)
        {
            if (self.emailCodes.TryGetValue(email, out EntityRef<VerificationCode> code))
            {
                return code.Entity.code;
            }

            var codeEntity = self.AddChild<VerificationCode>();
            {
                self.emailCodes.Add(email, codeEntity);
                codeEntity.code = self.RandomCode();
                codeEntity.isEamil = true;
                codeEntity.owner = email;
            }
            return codeEntity.code;
        }

        public static string GetVerificationCodeByPhone(this VerificationCodeComponent self, string phone)
        {
            throw new Exception();
        }

        public static VerificationCodeComponent RemoveCodeByEmail(this VerificationCodeComponent self, string email)
        {
            self.emailCodes.Remove(email);
            return self;
        }

        public static VerificationCodeComponent removeCodeByPhone(this VerificationCodeComponent self, string phone)
        {
            self.phoneCodes.Remove(phone);
            return self;
        }

        private static string RandomCode(this VerificationCodeComponent self)
        {
            var code = RandomGenerator.RandomNumber(0,999999);
            return code.ToString(); //TODO:不足6位补足6位
            
        }
    }
}