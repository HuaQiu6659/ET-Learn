namespace ET.Server
{
    public static class SessionHelper
    {
        /// <summary>
        /// delay 秒后将session断联
        /// </summary>
        /// <param name="self"></param>
        /// <param name="delay">秒</param>
        public static async ETTask DisconnectAsync(this Session self, float delay = 1)
        {
            if (self is null || self.IsDisposed)
                return;

            long instanceID = self.InstanceId;

            await self.Root().GetComponent<TimerComponent>().WaitAsync((int)(delay * 1000));
            //可能中途被释放或者用于别的连接
            if (self.InstanceId != instanceID)
                return;
            
            self.Dispose();
        }

        public static SessionLockingComponent Lock(this Session self)
        {
            return self.AddComponent<SessionLockingComponent>();
        }

        /// <summary>
        /// 检测是否重复请求，重复请求时对重复的请求发送错误并断开连接
        /// </summary>
        /// <param name="self"></param>
        /// <param name="response"></param>
        /// <returns>True：重复请求，False：正常请求</returns>
        public static bool IsRepeateRequest(this Session self, IResponse response)
        {
            var isLocking = self.GetComponent<SessionLockingComponent>() != null;
            if (isLocking)
            {
                response.Error = (int)EErrorCode.ERR_RequestRepeated;
                self.DisconnectAsync().Coroutine();
            }
            return isLocking;
        }
    }
}