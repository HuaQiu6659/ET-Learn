/*
┌────────────────────────────┐
│　Description: 
│　Remark: 
└────────────────────────────┘
*/

namespace ET.Server
{
    [ChildOf(typeof(CacheComponent))]
    public class Cache : Entity, IAwake
    {
        public EntityRef<Entity> cache;
        public bool cacheChanged;
        public long lastVisitTime;
        public long createTime;
    }
}
