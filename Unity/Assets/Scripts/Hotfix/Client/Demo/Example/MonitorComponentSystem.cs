namespace ET.Client
{

    [EntitySystemOf(typeof(MonitorComponent))]
    [FriendOfAttribute(typeof(ET.Client.MonitorComponent))]
    public static partial class MonitorComponentSystem
    {
        [EntitySystem]
        private static void Awake(this ET.Client.MonitorComponent self, int args2)
        {
            Log.Debug("MonitorComponent Awake");

            //要修改entity的属性时需要给System加上 FriendOf 标签
            self.brigthness = args2;
        }

        [EntitySystem]
        private static void Destroy(this ET.Client.MonitorComponent self)
        {

        }

        public static void ChangeBrightness(this MonitorComponent self, int value)
        {
            self.brigthness = value;
        }
    }
}