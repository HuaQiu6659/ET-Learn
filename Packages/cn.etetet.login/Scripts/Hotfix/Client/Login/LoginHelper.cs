namespace ET.Client
{
    public static class LoginHelper
    {
        public static async ETTask Login(Scene root, string address, string account, string password)
        {
            //保证登录与上一次的链接无关联
            root.RemoveComponent<ClientSenderComponent>();
            ClientSenderComponent clientSenderComponent = root.AddComponent<ClientSenderComponent>();

            long playerId = await clientSenderComponent.LoginAsync(address, account, password);
            root.GetComponent<PlayerComponent>().MyId = playerId;
            await EventSystem.Instance.PublishAsync(root, new LoginFinish());
        }
    }
}