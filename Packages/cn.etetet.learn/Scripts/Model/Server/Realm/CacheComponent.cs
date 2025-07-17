/*
┌────────────────────────────┐
│　Description: 
│　Remark: 
└────────────────────────────┘
*/
using System;
using System.Collections.Generic;

namespace ET.Server
{
    [ComponentOf(typeof(Scene))]
    public class CacheComponent : Entity, IAwake, IDestroy
    {
        public Dictionary<Type, HashSet<EntityRef<Cache>>> cacheMap;
    }
}
