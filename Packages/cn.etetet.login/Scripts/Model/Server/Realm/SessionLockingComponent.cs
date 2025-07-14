/*
┌────────────────────────────┐
│　Description: 标识单个Session的特定请求, 避免单个Session短时间重复请求
│　Remark: 
└────────────────────────────┘
*/
namespace ET.Server
{
    /// <summary>
    /// 标识单个Session的特定请求, 避免单个Session短时间重复请求
    /// </summary>
    [ComponentOf(typeof(Session))]
    public class SessionLockingComponent : Entity, IAwake
    {

    }
}
