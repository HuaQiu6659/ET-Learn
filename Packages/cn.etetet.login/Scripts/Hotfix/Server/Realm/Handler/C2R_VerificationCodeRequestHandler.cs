using System.Net.Http;
using System.Text;

namespace ET.Server
{

    //Description: 验证码获取请求处理
    [MessageSessionHandler(SceneType.Realm)]
    [FriendOf(typeof(VerificationCode))]
    public class C2R_VerificationCodeRequestHandler : MessageSessionHandler<C2R_VerificationCodeRequest, R2C_VerificationCodeResponse>
    {
        protected override async ETTask Run(Session session, C2R_VerificationCodeRequest request, R2C_VerificationCodeResponse response)
        {
            using (await session.Root().GetComponent<CoroutineLockComponent>().Wait(CoroutineLockType.VerificationCode, request.Receiver))
            {
                var code = session.Root().GetComponent<VerificationCodeComponent>().GetVerificationCode(request.Receiver);
                var now = TimeInfo.Instance.ServerNow();
                if (now - code.lastSendTime <= 1 * 1000)    //短时间内重复请求
                {
                    RelesaseWithError(EErrorCode_Login.请求过于频繁);
                    return;
                }

                if (request.IsEamil)
                {
                    // 通过邮箱发送验证码
                    bool emailSent = await session.Root().GetComponent<EmailSenderComponent>().SendVerificationCodeAsync(request.Receiver, code.code);
                    if (!emailSent)
                    {
                        RelesaseWithError(EErrorCode_Login.验证码发送失败);
                        return;
                    }
                    Log.Info($"Verification code sent to email: {request.Receiver}");
                }
                else
                {
                    //TODO:通过SDK发送短信验证码(暂时未选用SDK, 先不做)
                }

                code.lastSendTime = now;
            }

            void RelesaseWithError(EErrorCode_Login err)
            {
                response.Error = (int)err;
                response.Message = err.ToString();
                session.DisconnectAsync().NoContext();
            }
        }

        public static async ETTask<string> PostAsync(string url, string content, string contentType = "application/json")
        {
            try
            {
                var httpContent = new StringContent(content, Encoding.UTF8, contentType);
                var response = await new HttpClient().PostAsync(url, httpContent);
                return await response.Content.ReadAsStringAsync();
            }
            catch (System.Exception e)
            {
                Log.Error($"HTTP POST request failed: {e.Message}");
                return null;
            }
        }
    }
}
