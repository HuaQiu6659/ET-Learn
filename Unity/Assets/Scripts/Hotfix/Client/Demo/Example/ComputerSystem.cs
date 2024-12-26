namespace ET.Client
{
    //Hotfix里面不能定义实体类，只能定义静态类
    [EntitySystemOf(typeof(Computer))]
    public static partial class ComputerSystem
    {
        //这些拓展方法是根据Entity继承的接口生成，并对应生命周期
        [EntitySystem]
        private static void Awake(this ET.Client.Computer self)
        {
            Log.Debug("Computer Awake.");
        }
        
        [EntitySystem]
        private static void Update(this ET.Client.Computer self)
        {
            Log.Debug("Computer Update.");
        }
        
        [EntitySystem]
        private static void Destroy(this ET.Client.Computer self)
        {
            Log.Debug("Computer Destroy.");
        }

        public static void Open(this Computer self)
        {
            Log.Debug("Computer Open!");
        }
    }
}
