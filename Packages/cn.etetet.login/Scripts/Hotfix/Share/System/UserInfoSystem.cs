/*
┌────────────────────────────┐
│　Description: 
│　Remark: 
└────────────────────────────┘
*/
namespace ET
{
    [EntitySystemOf(typeof(UserInfo))]
    public static partial class UserInfoSystem
    {
        [EntitySystem]
        private static void Awake(this ET.UserInfo self)
        {

        }

        public static UserInfo FromProto(this ET.UserInfo self, UserInfoProto proto)
        {
            self.account = proto.Account;
            self.name = proto.Name;
            self.serverId = (int)proto.ServerId;
            self.iconUrl = proto.IconUrl;
            return self;
        }

        public static UserInfoProto ToProto(this ET.UserInfo self)
        {
            var proto = UserInfoProto.Create();
            {
                proto.Account = self.account;
                proto.ServerId = self.serverId;
                proto.Name = self.name;
                proto.IconUrl = self.iconUrl;
            }
            return proto;
        }
    }
}