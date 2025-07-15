/*
┌────────────────────────────┐
│　Description: 登录中心服初始化
│　Remark: 
└────────────────────────────┘
*/
namespace ET.Server
{
    [Invoke(SceneType.LoginCenter)]
    public class FiberInit_LoginCenter : AInvokeHandler<FiberInit, ETTask>
    {
        public override async ETTask Handle(FiberInit init)
        {
            var root= init.Fiber.Root;
            root.AddComponent<MailBoxComponent, int>(MailBoxType.UnOrderedMessage);
            root.AddComponent<TimerComponent>();
            root.AddComponent<CoroutineLockComponent>();
            root.AddComponent<ProcessInnerSender>();
            root.AddComponent<MessageSender>();

            root.AddComponent<LoginInfoRecorderComponent>();

            await ETTask.CompletedTask;
        }
    }
}