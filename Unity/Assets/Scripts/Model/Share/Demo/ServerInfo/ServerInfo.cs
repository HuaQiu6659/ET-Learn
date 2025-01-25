namespace ET
{
    [ChildOf]
    public class ServerInfo : Entity, IAwake
    {
        public EServerState state;
        public string serverName;
    }

    public enum EServerState
    {
        Normal,
        Stop
    }
}
