/*
┌────────────────────────────┐
│　Description: 
│　Remark: 
└────────────────────────────┘
*/
namespace ET
{
    public static partial class SessionSystem
    {
        /// <summary>
        /// 在 millisecond 后断开Session
        /// </summary>
        public static async ETTask Disconnect(this Session self, int millisecond = 1000)
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
    }
}