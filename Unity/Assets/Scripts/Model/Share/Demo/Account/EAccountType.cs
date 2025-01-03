using System.ComponentModel;

namespace ET
{
    public enum EAccountType
    {
        [Description("正常")]
        General,
        
        [Description("被封禁")]
        BlackList
    }
}