using UnityEngine.Pool;

namespace Lin.Runtime
{
    //加锁
    public static class Factory<T> where T : class, new()
    {
#pragma warning disable ET0015 // Static字段声明需要标记标签
        private static ObjectPool<T> pool;
        private static readonly object lockObject = new object();

        public static T Get()
        {
            lock (lockObject)
            {
            if (pool is null)
                pool = new ObjectPool<T>(() => new T(), actionOnRelease: item => { });
            }
            return pool.Get();
        }

        public static void Release(T target)
        {
            if (pool != null)
            {
                pool.Release(target);
            }
        }
    }
}
