using System.Collections.Generic;

namespace ET.Server
{
    /// <summary>
    /// key:accountID   value:zoneID
    /// </summary>
    [ComponentOf(typeof(Scene))]
    public class LoginInfosRecordComponent : Entity, IAwake, IDestroy
    {
        public Dictionary<long, int> accountLoginInfosMap = new Dictionary<long, int>();
    }
}
