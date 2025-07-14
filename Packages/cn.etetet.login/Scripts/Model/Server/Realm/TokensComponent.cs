/*
┌────────────────────────────┐
│　Description: Token缓存
│　Remark: 
└────────────────────────────┘
*/
using System.Collections.Generic;

namespace ET.Server
{
    [ComponentOf(typeof(Scene))]
    public class TokensComponent : Entity, IAwake, IDestroy
    {
        public Dictionary<string, string> tokensMap;
    }
}
