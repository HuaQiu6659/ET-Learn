/*
┌────────────────────────────┐
│　Description: 
│　Remark: 
└────────────────────────────┘
*/
using System.Threading.Tasks;

namespace ET
{
    public static partial class TaskHelper
    {
        public static async ETTask ETTask(this Task self)
        {
            while (!self.IsCompleted && !self.IsCanceled)
                await ET.ETTask.CompletedTask;
        }

        public static async ETTask<TResult> ETTask<TResult>(this Task<TResult> self)
        {
            while (!self.IsCompleted && !self.IsCanceled)
                await ET.ETTask.CompletedTask;

            return self.Result;
        }
    }
}