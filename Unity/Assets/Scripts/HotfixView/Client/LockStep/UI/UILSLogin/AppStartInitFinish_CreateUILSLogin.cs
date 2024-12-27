namespace ET.Client
{
	[Event(SceneType.LockStep)]
	public class AppStartInitFinish_CreateUILSLogin: AEvent<Scene, AppStartInitFinish>
	{
		protected override async ETTask Run(Scene root, AppStartInitFinish args)
		{
			Log.Debug("LockStep Mode");
			
			await UIHelper.Create(root, UIType.UILSLogin, UILayer.Mid);
		}
	}
}
