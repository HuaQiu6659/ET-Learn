/*
┌────────────────────────────┐
│　Description: 
│　Remark: 
└────────────────────────────┘
*/
using System;

namespace ET.Server
{
    [EntitySystemOf(typeof(Cache))]
    public static partial class CacheSystem
    {
        [EntitySystem]
        private static void Awake(this ET.Server.Cache self)
        {

        }

        public static Cache Init<T>(this Cache self, T toCache) where T : Entity
        {
            self.AddChild(toCache);
            self.cache = toCache;
            self.cacheChanged = false;
            var now = TimeInfo.Instance.ServerNow();
            self.createTime = now;
            self.lastVisitTime = now;
            return self;
        }

        public static T GetCache<T>(this Cache self) where T : Entity => self.cache as T;

        public static Cache Update<T>(this Cache self, T toCache) where T : Entity
        {
            self.cache = toCache;
            self.cacheChanged = true;
            self.lastVisitTime = TimeInfo.Instance.ServerNow();
            return self;
        }

        public static async ETTask<Cache> Save(this Cache self, DBComponent db)
        {
            if (!self.cacheChanged)
                return self;

            await db.Save(self.cache.Entity);
            self.cacheChanged = true;
            return self;
        }
    }
}