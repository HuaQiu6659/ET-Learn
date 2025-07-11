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
        public static async ETTask Release(this Session self, int millisecond = 0)
        {
            if (millisecond > 0)
                await self.Root().GetComponent<TimerComponent>().WaitAsync(millisecond);
            self.Dispose();
        }
    }
}