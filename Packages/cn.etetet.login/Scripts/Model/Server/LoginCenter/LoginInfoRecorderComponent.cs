/*
┌────────────────────────────┐
│　Description: 
│　Remark: 
└────────────────────────────┘
*/
using System.Collections.Generic;

namespace ET.Server
{
    [ComponentOf(typeof(Scene))]
    public class LoginInfoRecorderComponent : Entity, IAwake, IDestroy
    {
        /// <summary>
        /// Key: accountHash, Value:区号
        /// </summary>
        public Dictionary<long, int> accountInfosMap;
    }
}
