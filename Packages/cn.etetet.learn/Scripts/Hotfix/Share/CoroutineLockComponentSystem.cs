/*
┌────────────────────────────┐
│　Description: Wait对string作为key的拓展
│　Remark: 
└────────────────────────────┘
*/
namespace ET
{
    public static partial class CoroutineLockComponentSystem
    {
        public static async ETTask<CoroutineLock> Wait(this CoroutineLockComponent self, long coroutineLockType, string key, int time = 60000)
        {
            CoroutineLockQueueType coroutineLockQueueType = self.GetChild<CoroutineLockQueueType>(coroutineLockType) ?? self.AddChildWithId<CoroutineLockQueueType>(coroutineLockType);
            return await coroutineLockQueueType.Wait(key.GetLongHashCode(), time);
        }
    }
}