namespace ET.Server
{
    /// <summary>
    /// 邮件服务配置
    /// </summary>
    public static class EmailConfig
    {
        // 邮件服务API配置
        // 实际使用时需要替换为真实的邮件服务提供商配置
        // 推荐的邮件服务提供商：
        // 1. SendGrid: https://sendgrid.com/
        // 2. Mailgun: https://www.mailgun.com/
        // 3. 阿里云邮件推送: https://www.aliyun.com/product/directmail
        // 4. 腾讯云邮件推送: https://cloud.tencent.com/product/ses
        
        /// <summary>
        /// 邮件服务API地址
        /// </summary>
        public const string EMAIL_API_URL = "https://api.emailservice.com/send";
        
        /// <summary>
        /// API密钥 - 实际使用时需要配置真实的API密钥
        /// 建议从环境变量或配置文件中读取，不要硬编码在代码中
        /// </summary>
        public const string API_KEY = "your-api-key-here";
        
        /// <summary>
        /// 发件人邮箱
        /// </summary>
        public const string SENDER_EMAIL = "noreply@yourdomain.com";
        
        /// <summary>
        /// 发件人名称
        /// </summary>
        public const string SENDER_NAME = "验证码服务";
        
        /// <summary>
        /// 验证码邮件主题
        /// </summary>
        public const string VERIFICATION_SUBJECT = "验证码";
        
        /// <summary>
        /// 验证码有效期（分钟）
        /// </summary>
        public const int VERIFICATION_CODE_EXPIRE_MINUTES = 5;
        
        // SMTP配置（备用方案）
        public const string SMTP_SERVER = "smtp.example.com";
        public const int SMTP_PORT = 587;
        public const string SMTP_USERNAME = "your-smtp-username";
        public const string SMTP_PASSWORD = "your-smtp-password";
        public const bool SMTP_ENABLE_SSL = true;
    }
}