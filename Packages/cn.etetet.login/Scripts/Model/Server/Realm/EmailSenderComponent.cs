/*
┌────────────────────────────┐
│　Description: 邮箱发送
│　Remark: 
└────────────────────────────┘
*/
namespace ET.Server
{
    /// <summary>
    /// 向外部邮箱发送信息
    /// </summary>
    [ComponentOf(typeof(Scene))]
    public class EmailSenderComponent : Entity, IAwake
    {
        /// <summary> 发件人名字 </summary>
        public string name;

        /// <summary> 发件人邮箱(自己的邮箱) </summary>
        public string eamil;

        //SMTP
        public string smtpServer;
        public int smtpPort;
        public string smtpUserName;
        public string smtpPassword;
        public bool smtpEnableSSL;
        public const bool SMTP_ENABLE_SSL = true;
    }
}
