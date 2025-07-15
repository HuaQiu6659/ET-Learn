namespace ET.Server
{
    [ChildOf(typeof(PlayerComponent))]
    public sealed class Player : Entity, IAwake<string>
    {
        public string Account { get; set; }

        public EPlayerState State { get; set; }

        public long UnitId { get; set; }
    }

    public enum EPlayerState
    {
        离线, 
        登陆中,
        在线
    }
}