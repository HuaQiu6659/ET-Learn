using System;
using System.ComponentModel;
using System.Reflection;

namespace ET.Helper
{
    public static class ReflectionHelper
    {
        public static string GetEnumDescription(this Enum self)
        {
            string enumStr = self.ToString();
        
            // 获取枚举类型
            FieldInfo field = self.GetType().GetField(enumStr);

            // 获取 Description 特性
            DescriptionAttribute attribute = (DescriptionAttribute)Attribute.GetCustomAttribute(field, typeof(DescriptionAttribute));

            // 返回描述，如果没有描述则返回枚举值的字符串
            return attribute != null ? attribute.Description : enumStr;
        }
    }
}
