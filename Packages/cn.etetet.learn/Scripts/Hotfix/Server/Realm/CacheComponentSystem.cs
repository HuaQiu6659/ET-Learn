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

    [Invoke(TimerInvokeType.CacheRefresh)]
    public class CacheComponentTimer : ATimer<CacheComponent>
    {
        protected override void Run(CacheComponent t)
        {
            t?.WriteIntoDataBase();
        }
    }

    [FriendOf(typeof(Cache))]
    [EntitySystemOf(typeof(CacheComponent))]
    public static partial class CacheComponentSystem
    {
        [EntitySystem]
        private static void Awake(this ET.Server.CacheComponent self)
        {
            self.cacheMap = new Dictionary<Type, Dictionary<long, EntityRef<Cache>>>();
            self.timer = self.Root().GetComponent<TimerComponent>().NewRepeatedTimer(10 * 60 * 1000, TimerInvokeType.CacheRefresh, self);
        }

        [EntitySystem]
        private static void Destroy(this ET.Server.CacheComponent self)
        {
            self.Root().GetComponent<TimerComponent>().Remove(ref self.timer);
            self.cacheMap.Clear();
            self.cacheMap = null;

        }

        private static long GetLockerKey<T>() => GetLockerKey(typeof(T));

        private static long GetLockerKey(Type type) => type.FullName.GetLongHashCode();

        private static Dictionary<long, EntityRef<Cache>> GetCaches<T>(this CacheComponent self) where T : class
        {
            var key = typeof(T);
            if (!self.cacheMap.TryGetValue(key, out var result))
            {
                result = new Dictionary<long, EntityRef<Cache>>();
                self.cacheMap.Add(key, result);
            }
            return result;
        }

        public static async ETTask<List<T>> Query<T>(this CacheComponent self, Expression<Func<T, bool>> filter) where T : Entity
        {
            using var locker = await self.CoroutineLock(ECoroutineLockType.数据缓存操作, GetLockerKey<T>());

            var func = filter.Compile();
            var caches = self.GetCaches<T>();
            var refOfCache = caches.AsValueEnumerable().Where(c => func(c.Value.Entity.cache.Entity as T));
            if (refOfCache.Count() > 0)
                return refOfCache.Select(c => c.Value.Entity.cache.Entity as T).ToList();

            var db = self.Root().GetComponent<DBManagerComponent>().GetZoneDB(self.Zone());
            var list = await db.Query(filter);
            if (list.Count > 0)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    var entity = list[i];
                    var unuseCache = self.AddChild<Cache>();
                    {
                        unuseCache.Init(entity);
                    }
                    caches.Add(entity.Id, unuseCache);
                }
            }
            return list;
        }

        public static async ETTask<CacheComponent> AddOrUpdate<T>(this CacheComponent self, T entity) where T : Entity
        {
            using var locker = await self.CoroutineLock(ECoroutineLockType.数据缓存操作, GetLockerKey<T>());

            var caches = self.GetCaches<T>();
            if (!caches.TryGetValue(entity.Id, out var cache))
            {
                cache = self.AddChild<Cache>();
                {
                    cache.Entity.Init(entity);
                }
                caches.Add(entity.Id, cache);
            }
            else
            {
                cache.Entity.cache = entity;
                cache.Entity.cacheChanged = true;
            }
            return self;
        }

        public static async ETTask<CacheComponent> WriteIntoDataBase(this CacheComponent self)
        {
            var dbMgr = self.Root().GetComponent<DBManagerComponent>();
            foreach (var item in self.cacheMap.AsValueEnumerable())
            {
                using var locker = await self.CoroutineLock(ECoroutineLockType.数据缓存操作, GetLockerKey(item.Key));
                foreach (var cache in item.Value.AsValueEnumerable())
                {
                    if (!cache.Value.Entity.cacheChanged)
                        continue;

                    var db = dbMgr.GetZoneDB(cache.Value.Entity.cache.Entity.Zone());
                    await db.Save(cache.Value.Entity.cache.Entity);
                }
            }

            await ETTask.CompletedTask;
            return self;
        }
    }
}