/*
┌────────────────────────────┐
│　Description: 账号超时???
│　Remark: 
└────────────────────────────┘
*/

namespace ET.Server
{
    [ComponentOf(typeof(Session))]
    public class AccountCheckOutTimeComponent : Entity, IAwake<string>
    {
        public string account;
    }
}
