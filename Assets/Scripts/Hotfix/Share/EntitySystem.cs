/*
┌────────────────────────────┐
│　Description: 
│　Remark: 
└────────────────────────────┘
*/
namespace ET
{
    public static partial class EntitySystem
    {
#pragma warning disable ET0014 // 禁止在Entity类中直接调用Child和Component
        public static K GetOrAddComponent<K>(this Entity self) where K : Entity, IAwake, new() => self.GetComponent<K>() ?? self.AddComponent<K>();

        public static K ReplaceComponent<K>(this Entity self, bool isFromPool = false) where K : Entity, IAwake, new()
        {
            self.RemoveComponent<K>();
            return self.AddComponent<K>(isFromPool);
        }
#pragma warning restore ET0014 // 禁止在Entity类中直接调用Child和Component
    }
}