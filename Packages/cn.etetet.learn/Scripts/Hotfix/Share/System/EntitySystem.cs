/*
┌────────────────────────────┐
│　Description: 
│　Remark: 
└────────────────────────────┘
*/
using System;

namespace ET
{
    public static partial class EntitySystem
    {
        [EnableAccessEntiyChild]
        public static K GetOrAddComponent<K>(this Entity self) where K : Entity, IAwake, new() => self.GetComponent<K>() ?? self.AddComponent<K>();

        [EnableAccessEntiyChild]
        public static K ReplaceComponent<K>(this Entity self, bool isFromPool = false) where K : Entity, IAwake, new()
        {
            self.RemoveComponent<K>();
            return self.AddComponent<K>(isFromPool);
        }

        [EnableAccessEntiyChild]
        public static bool HasComponent<K>(this Entity self) where K : Entity => self.GetComponent<K>() is not null;


        [EnableAccessEntiyChild]
        public static ETTask<CoroutineLock> CoroutineLock(this Entity self, ECoroutineLockType lockType, long key, int time = 60_000)
        {
            var locker = self.Root().GetComponent<CoroutineLockComponent>();
            if (locker is null)
                throw new NullReferenceException("Root has not CoroutineLockComponent.");

            return locker.Wait((int)lockType, key, time);
        }

        [EnableAccessEntiyChild]
        public static ETTask<CoroutineLock> CoroutineLock(this Entity self, ECoroutineLockType lockType, string key, int time = 60_000) => self.CoroutineLock(lockType, key.GetLongHashCode(), time);
    }
}