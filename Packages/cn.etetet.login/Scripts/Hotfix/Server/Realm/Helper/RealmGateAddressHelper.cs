using System.Collections.Generic;


namespace ET.Server
{
	public static partial class RealmGateAddressHelper
	{
        public static StartSceneConfig GetGate(int zone, string account) => GetGate(zone, account.GetLongHashCode());

        public static StartSceneConfig GetGate(int zone, long accountHash)
        {
			ulong hash = (ulong)accountHash;
            List<StartSceneConfig> zoneGates = StartSceneConfigCategory.Instance.GetBySceneType(zone, SceneType.Gate);
            return zoneGates[(int)(hash % (ulong)zoneGates.Count)];
        }
    }
}
