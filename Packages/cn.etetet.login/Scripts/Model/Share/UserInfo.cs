namespace ET
{
    [ChildOf]
    public class UserInfo : Entity, IAwake
	{
        public string account;
        public int serverId;
        public string name;
        public string iconUrl;
	}
}