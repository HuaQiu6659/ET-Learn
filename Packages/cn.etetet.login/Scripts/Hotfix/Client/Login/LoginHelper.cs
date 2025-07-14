using Cysharp.Text;

namespace ET.Client
{
    public static class LoginHelper
    {
        public static async ETTask LoginByPassword(Scene root, string address, string account, string password)
        {
            //保证登录与上一次的链接无关联
            ClientSenderComponent clientSenderComponent = root.ReplaceComponent<ClientSenderComponent>();

            var response = await clientSenderComponent.LoginAsync(address, account, password);
            if (response.Error != ErrorCode.ERR_Success)
            {
                Log.Error(ZString.Format("登录失败, {0}:{1}", response.Error, response.Message));
                return;
            }
            root.GetComponent<PlayerComponent>().MyId = response.PlayerId;
            await EventSystem.Instance.PublishAsync(root, new LoginFinish());
        }
    }
}