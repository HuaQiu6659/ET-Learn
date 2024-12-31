using System;
using System.Security.Cryptography;
using System.Text;

namespace ET.Helper
{
    public static class HashHelper
    {
        /// <summary>
        /// 获取字符串的Hash值
        /// </summary>
        public static string StringSHA1(this string self)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(self);
            return BytesSHA1(buffer);
        }

        /// <summary>
        /// 获取字节数组的Hash值
        /// </summary>
        public static string BytesSHA1(this byte[] self)
        {
            // 说明：创建的是SHA1类的实例，生成的是160位的散列码
            HashAlgorithm hash = HashAlgorithm.Create();
            byte[] hashBytes = hash.ComputeHash(self);
            return ToString(hashBytes);
        }
    
        private static string ToString(byte[] hashBytes)
        {
            string result = BitConverter.ToString(hashBytes);
            result = result.Replace("-", "");
            return result.ToLower();
        }
    }
}
