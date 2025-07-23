/*
┌────────────────────────────┐
│　Description: 
│　Remark: 
└────────────────────────────┘
*/
namespace ET.Server
{
    public static partial class SessionSystem
    {
        /// <summary>
        /// 在 millisecond 后断开Session
        /// </summary>
        public static async ETTask DisconnectAsync(this Session self, int millisecond = 1000)
        {
            if (millisecond > 0)
            {
                long instanceId = self.InstanceId;
                await self.Root().GetComponent<TimerComponent>().WaitAsync(millisecond);

                //说明此时session的用途已经发生改变, 也可能已经被释放
                if (self.InstanceId != instanceId)
                    return;
            }
            self.Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="self"></param>
        /// <param name="hasLocked">在调用之前是否已经有锁 True:已经有锁</param>
        /// <returns></returns>
        public static SessionLockingComponent Lock(this Session self, out bool hasLocked)
        {
            if (self.GetComponent<SessionLockingComponent>() != null)
            {
                hasLocked = true;
                return null;
            }
            hasLocked = false;
            return self.AddComponent<SessionLockingComponent>();
        }
    }
}