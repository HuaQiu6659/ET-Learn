namespace ET
{
    [ChildOf]
	public class ServerInfo : Entity, IAwake
	{
		public string name;
		public EServerState state;
	}
}