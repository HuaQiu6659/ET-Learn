namespace ET.Server
{
    /// <summary>
    /// 长时间无操作导致的连接断开
    /// </summary>
    [ComponentOf(typeof(Session))]
    public class AccountCheckTimeOutComponent : Entity, IAwake<string>, IDestroy
    {
        public long timerID = 0;
        public string account;
    }
}