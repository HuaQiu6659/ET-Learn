/*
┌────────────────────────────┐
│　Description: 
│　Remark: 
└────────────────────────────┘
*/

using System;
using ET.Server;

namespace ET.Client
{
    [EntitySystemOf(typeof(VerificationCodeTimeOutComponent))]
    public static partial class VerificationCodeTimeOutComponentSystem
    {
        [Invoke(TimerCoreInvokeType.VerificationCodeTimeOut)]
        public class VerificationCodeTimeOut: ATimer<VerificationCodeTimeOutComponent>
        {
            protected override void Run(VerificationCodeTimeOutComponent self)
            {
                try
                {
                    self.Parent.Dispose();
                }
                catch (Exception e)
                {
                    Log.Error($"move timer error: {self.Id}\n{e}");
                }
            }
        }
        
        [EntitySystem]
        private static void Awake(this ET.Server.VerificationCodeTimeOutComponent self)
        { 
            self.timer = self.Root().GetComponent<TimerComponent>().NewOnceTimer(TimeInfo.Instance.ServerNow() + 5 * 60 * 1000, TimerCoreInvokeType.VerificationCodeTimeOut, self);
        }
        
        [EntitySystem]
        private static void Destroy(this ET.Server.VerificationCodeTimeOutComponent self)
        {
            self.Root().GetComponent<TimerComponent>()?.Remove(ref self.timer);
        }
    }
}