using System.Text.RegularExpressions;

namespace ET
{
    /// <summary>
    /// 账号验证工具类
    /// </summary>
    public static class AccountHelper
    {
        /// <summary>
        /// 验证账号是否合法（支持邮箱和手机号）
        /// </summary>
        /// <param name="account">账号</param>
        /// <returns>是否合法</returns>
        public static bool IsValidAccount(string account)
        {
            if (string.IsNullOrWhiteSpace(account))
                return false;

            // 检测是否为邮箱格式
            if (IsValidEmail(account))
                return true;

            // 检测是否为手机号格式
            if (IsValidPhoneNumber(account))
                return true;

            return false;
        }
        
        /// <summary>
        /// 验证是否为合法邮箱
        /// </summary>
        /// <param name="email">邮箱地址</param>
        /// <returns>是否为合法邮箱</returns>
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            // 邮箱正则表达式
            Regex emailRegex = new Regex(
                @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
                RegexOptions.IgnoreCase | RegexOptions.Compiled);

            try
            {
                // RFC 5321 规定邮箱地址最大长度为254字符
                if (email.Length > 254)
                    return false;
                
                // 检查基本格式
                if (!emailRegex.IsMatch(email))
                    return false;
                
                // 检查本地部分长度（@符号前的部分，最大64字符）
                var atIndex = email.IndexOf('@');
                if (atIndex > 64)
                    return false;
                
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 验证是否为合法手机号
        /// </summary>
        /// <param name="phoneNumber">手机号</param>
        /// <returns>是否为合法手机号</returns>
        public static bool IsValidPhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return false;

            try
            {
                // 移除所有空格和连字符
                var cleanPhone = phoneNumber.Replace(" ", "").Replace("-", "");
                // 中国大陆手机号正则表达式
                Regex chinaPhoneRegex = new Regex(
                    @"^1[3-9]\d{9}$",
                    RegexOptions.Compiled);

                // 中国大陆手机号验证（11位，1开头）
                if (chinaPhoneRegex.IsMatch(cleanPhone))
                    return true;

                // 国际手机号正则表达式
                Regex internationalPhoneRegex = new Regex(
                    @"^\+?[1-9]\d{7,14}$",
                    RegexOptions.Compiled);

                // 国际手机号验证（支持+号开头，8-15位数字）
                return internationalPhoneRegex.IsMatch(cleanPhone);
            }
            catch
            {
                return false;
            }
        }
        
        /// <summary>
        /// 获取账号类型
        /// </summary>
        /// <param name="account">账号</param>
        /// <returns>账号类型</returns>
        public static AccountType GetAccountType(string account)
        {
            if (string.IsNullOrWhiteSpace(account))
                return AccountType.Invalid;
            
            if (IsValidEmail(account))
                return AccountType.Email;
            
            if (IsValidPhoneNumber(account))
                return AccountType.Phone;
            
            return AccountType.Invalid;
        }
        
        /// <param name="password">密码</param>
        /// <returns>是否符合要求</returns>
        public static bool IsValidPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return false;

            // 检查是否包含非法字符（只允许字母、数字）
            var passwordRegex = new Regex(@"^[a-zA-Z0-9]+$");
            return passwordRegex.IsMatch(password);
        }
    }
    
    /// <summary>
    /// 账号类型枚举
    /// </summary>
    public enum AccountType
    {
        Invalid,    // 无效
        Email,      // 邮箱
        Phone       // 手机号
    }
}