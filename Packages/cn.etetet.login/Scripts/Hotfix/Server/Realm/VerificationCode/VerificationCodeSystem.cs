/*
┌────────────────────────────┐
│　Description: 
│　Remark: 
└────────────────────────────┘
*/
namespace ET.Server
{
    [EntitySystemOf(typeof(VerificationCode))]
    public static partial class VerificationCodeSystem
    {
        [EntitySystem]
        private static void Destroy(this ET.Server.VerificationCode self)
        {
            var parent = self.Parent as VerificationCodeComponent;
            parent?.RemoveCodeAsync(self.owner);
        }
        
        [EntitySystem]
        private static void Awake(this ET.Server.VerificationCode self)
        {
            self.AddComponent<VerificationCodeTimeOutComponent>();
        }
    }
}