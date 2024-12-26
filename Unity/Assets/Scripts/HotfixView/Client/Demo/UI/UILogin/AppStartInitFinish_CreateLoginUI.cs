namespace ET.Client
{
	[Event(SceneType.Demo)]
	public class AppStartInitFinish_CreateLoginUI: AEvent<Scene, AppStartInitFinish>
	{
		protected override async ETTask Run(Scene root, AppStartInitFinish args)
		{
			//表现层可调用逻辑层代码
			//逻辑层不能直接调用表现层代码

			//AdChild 可添加多个相同类型实体
			var computer = root.GetComponent<ComputersComponent>().AddChild<Computer>();
			
			//AddComponent 则每个类型组件只能添加一个
			computer.AddComponent<PCCaseComponent>();
			computer.AddComponent<MonitorComponent, int>(10);
			
			await UIHelper.Create(root, UIType.UILogin, UILayer.Mid);
		}
	}
}
