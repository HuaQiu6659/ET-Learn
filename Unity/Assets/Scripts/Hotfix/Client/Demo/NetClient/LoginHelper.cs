namespace ET.Client
{
    public static class LoginHelper
    {
        public static async ETTask Login(Scene root, string account, string password)
        {
            root.RemoveComponent<ClientSenderComponent>();
            
            ClientSenderComponent clientSenderComponent = root.AddComponent<ClientSenderComponent>();

            var response = await clientSenderComponent.LoginAsync(account, password);
            if (response.Error != ErrorCode.ERR_Success)
            {
                Log.Error($"LoginFailed: {response.Error}");
                await EventSystem.Instance.PublishAsync(root, new LoginFailedEvent() { errorCode = response.Error });
                return;
            }

            string token = response.token;
            //root.GetComponent<PlayerComponent>().MyId = response.PlayerId;
            //TODO:获取服务器列表
            //TODO:获取区服角色列表
            
            await EventSystem.Instance.PublishAsync(root, new LoginFinish());
        }
    }
}