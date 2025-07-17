/*
┌────────────────────────────┐
│　Description: 
│　Remark: 
└────────────────────────────┘
*/
using System.Linq.Expressions;
using System;
using ZLinq;
using System.Collections.Generic;

namespace ET.Server
{
    [FriendOf(typeof(Cache))]
    [EntitySystemOf(typeof(CacheComponent))]
    public static partial class CacheComponentSystem
    {
        [EntitySystem]
        private static void Awake(this ET.Server.CacheComponent self)
        {
            self.cacheMap = new Dictionary<Type, HashSet<EntityRef<Cache>>>();
        }

        [EntitySystem]
        private static void Destroy(this ET.Server.CacheComponent self)
        {
            self.cacheMap.Clear();
            self.cacheMap = null;
        }

        private static HashSet<EntityRef<Cache>> GetCaches<T>(this CacheComponent self) where T : class
        {
            var key = typeof(T);
            if (!self.cacheMap.TryGetValue(key, out var result))
            {
                result = new HashSet<EntityRef<Cache>>();
                self.cacheMap.Add(key, result);
            }
            return result;
        }

        public static async ETTask<List<T>> Query<T>(this CacheComponent self, Expression<Func<T, bool>> filter) where T : Entity
        {
            var func = filter.Compile();
            var caches = self.GetCaches<T>();
            var refOfCache = caches.AsValueEnumerable().Where(c => func(c.Entity.cache.Entity as T));
            if (refOfCache.Count() > 0)
                return refOfCache.Select(c => c.Entity.cache.Entity as T).ToList();

            var db = self.Root().GetComponent<DBManagerComponent>().GetZoneDB(self.Zone());
            var list = await db.Query(filter);
            if (list.Count > 0)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    var unuseCache = self.AddChild<Cache>();
                    {
                        unuseCache.Init(list[i]);
                    }
                    caches.Add(unuseCache);
                }
            }
            return list;
        }

        public static CacheComponent Add<T>(this CacheComponent self, T entity) where T : Entity
        {
            var caches = self.GetCaches<T>();
            var newCache = self.AddChild<Cache>();
            {
                newCache.Init(entity);
            }
            caches.Add(newCache);
            return self;
        }
    }
}