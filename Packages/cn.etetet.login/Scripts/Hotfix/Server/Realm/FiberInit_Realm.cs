
namespace ET.Server
{
    [Invoke(SceneType.Realm)]
    public class FiberInit_Realm: AInvokeHandler<FiberInit, ETTask>
    {
        public override async ETTask Handle(FiberInit fiberInit)
        {
            Scene root = fiberInit.Fiber.Root;
            root.AddComponent<MailBoxComponent, int>(MailBoxType.UnOrderedMessage);
            root.AddComponent<TimerComponent>();
            root.AddComponent<CoroutineLockComponent>();
            root.AddComponent<ProcessInnerSender>();
            root.AddComponent<MessageSender>();
            StartSceneConfig startSceneConfig = StartSceneConfigCategory.Instance.Get(root.Fiber.Id);
            root.AddComponent<NetComponent, IKcpTransport>(new UdpTransport(startSceneConfig.InnerIPPort));

            root.AddComponent<DBManagerComponent>();
            root.AddComponent<VerificationCodeComponent>();
            root.AddComponent<EmailSenderComponent>();
            root.AddComponent<AccountSessionsComponent>();
            root.AddComponent<TokensComponent>();
            root.AddComponent<ServerInfosComponent>();

            await ETTask.CompletedTask;
        }
    }
}