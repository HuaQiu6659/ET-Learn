/*
┌────────────────────────────┐
│　Description: 
│　Remark: 
└────────────────────────────┘
*/
using System.Net.Http;
using System.Text;

namespace ET.Server
{
    [EntitySystemOf(typeof(EmailSenderComponent))]
    public static partial class EmailSenderComponentSystem
    {
        [EntitySystem]
        private static void Awake(this EmailSenderComponent self)
        {
            //TODO:读取配置表
        }

        // 邮件服务配置从EmailConfig类中获取

        /// <summary>
        /// 发送验证码邮件
        /// </summary>
        /// <param name="toEmail">收件人邮箱</param>
        /// <param name="verificationCode">验证码</param>
        /// <returns>是否发送成功</returns>
        public static async ETTask<bool> SendVerificationCodeAsync(this EmailSenderComponent self, string toEmail, string verificationCode)
        {
            const string subject = "验证码";

            try
            {
                // 构建邮件内容
                var emailData = new
                {
                    to = toEmail,
                    from = new { email = self.eamil, name = self.name },
                    subject = subject,
                    html = $@"
                        <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto;'>
                            <h2 style='color: #333;'>验证码</h2>
                            <p>您的验证码是：</p>
                            <div style='background-color: #f5f5f5; padding: 20px; text-align: center; font-size: 24px; font-weight: bold; color: #007bff; letter-spacing: 5px; margin: 20px 0;'>
                                {verificationCode}
                            </div>
                            <p style='color: #666;'>验证码有效期为{EmailConfig.VERIFICATION_CODE_EXPIRE_MINUTES}分钟，请及时使用。</p>
                            <p style='color: #999; font-size: 12px;'>如果您没有请求此验证码，请忽略此邮件。</p>
                        </div>",
                    apiKey = EmailConfig.API_KEY
                };

                string jsonContent = MongoHelper.ToJson(emailData);

                // 发送HTTP请求
                string response = await self.PostAsync(EmailConfig.EMAIL_API_URL, jsonContent);

                if (!string.IsNullOrEmpty(response))
                {
                    Log.Info($"Email sent successfully to {toEmail}, response: {response}");
                    return true;
                }
                else
                {
                    Log.Error($"Failed to send email to {toEmail}");
                    return false;
                }
            }
            catch (System.Exception e)
            {
                Log.Error($"Error sending email to {toEmail}: {e.Message}");
                return false;
            }
        }

        private static async ETTask<string> PostAsync(this EmailSenderComponent self, string url, string content, string contentType = "application/json")
        {
            try
            {
                var httpClient = new HttpClient();
                var httpContent = new StringContent(content, Encoding.UTF8, contentType);
                var task = new HttpClient().PostAsync(url, httpContent);
                while (!task.IsCompleted)
                    await ETTask.CompletedTask;

                var response = task.Result;
                var readTask = response.Content.ReadAsStringAsync();
                while (!readTask.IsCompleted)
                    await ETTask.CompletedTask;

                return readTask.Result;
            }
            catch (System.Exception e)
            {
                Log.Error($"HTTP POST request failed: {e.Message}");
                return null;
            }
        }
    }
}