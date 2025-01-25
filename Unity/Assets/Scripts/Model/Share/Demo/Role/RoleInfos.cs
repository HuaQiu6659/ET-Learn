namespace ET
{
    [ChildOf]
    public class RoleInfos:Entity, IAwake, IDestroy
    {
        public int id;
        public string name;
        public int serverID;
        public ERoleState state;
        public string account;
        public long lastLoginTime;
        public long createTime;
    }

    public enum ERoleState
    {
        Normal,
        Freeze
    }
}
