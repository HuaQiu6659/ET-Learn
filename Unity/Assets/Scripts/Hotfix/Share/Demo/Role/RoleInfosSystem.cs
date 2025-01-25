namespace ET
{
    [EntitySystemOf(typeof(RoleInfos))]
    [FriendOfAttribute(typeof(ET.RoleInfos))]
    public static partial class RoleInfosSystem
    {
        [EntitySystem]
        private static void Awake(this ET.RoleInfos self)
        {

        }

        [EntitySystem]
        private static void Destroy(this ET.RoleInfos self)
        {

        }
        
        public static void FromMessage(this RoleInfos self, RoleInfoProto message)
        {
            self.id = message.id;
            self.account = message.account;
            self.state = (ERoleState)message.state;
            self.name = message.name;
            self.lastLoginTime = message.lastLoginTime;
            self.createTime = message.createTime;
        }

        public static RoleInfoProto ToMessage(this RoleInfos self)
        {
            RoleInfoProto result = RoleInfoProto.Create();
            result.id = self.id;
            result.state = (int)self.state;
            result.account = self.account;
            result.createTime = self.createTime;
            result.lastLoginTime = self.lastLoginTime;
            result.name = self.name;
            return result;
        }
    }
}
