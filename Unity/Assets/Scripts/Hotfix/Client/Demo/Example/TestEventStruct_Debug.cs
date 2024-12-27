namespace ET.Client
{
    [Event(SceneType.Demo)]
    public class TestEventStruct_Debug :AEvent<Scene, TestEventStruct>
    {
        protected override async ETTask Run(Scene root, TestEventStruct a)
        {
            Log.Debug($"TestEvent: {a.intArg}");
			
            //表现层可调用逻辑层代码
            //逻辑层不能直接调用表现层代码

            //AddChild 可添加多个相同类型实体
            var computer = root.GetComponent<ComputersComponent>().AddChild<Computer>();
			
            //AddComponent 则每个类型组件只能添加一个
            computer.AddComponent<PCCaseComponent>();
            var monitorCmp = computer.AddComponent<MonitorComponent, int>(10);
            computer.Open();
            monitorCmp.ChangeBrightness(5);

            //entity.Scene()是获取上级最接近的Scene
            //Log.Debug($"ComputerRoot:{computer.Root().InstanceId}   Root:{root.InstanceId}   ComputerScene:{computer.Scene()}");

            await root.GetComponent<TimerComponent>().WaitAsync(3000);
            computer.Dispose();
            await ETTask.CompletedTask;
        }
    }
}
