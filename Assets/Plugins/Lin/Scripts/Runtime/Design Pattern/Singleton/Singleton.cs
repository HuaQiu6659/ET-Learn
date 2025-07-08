/*
┌────────────────────────────┐
│　Description: 一般类单例
│　Author: 花球i
│　Remark: 
└────────────────────────────┘
┌──────────────┐                                   
│　ClassName: Singleton
└──────────────┘
*/
using UnityEngine;

namespace Lin.Runtime.DesinPattern.Singleton
{
    public abstract class Singleton<T> where T : class, new()
    {
#pragma warning disable ET0015 // Static字段声明需要标记标签
        private static T instance;

        public static T GetInstance()
        {
            if (instance == null)
            {
                instance = new T();
                Application.quitting += Application_quitting;
            }
            return instance;
        }

        private static void Application_quitting()
        {
            instance = null;
            Application.quitting -= Application_quitting;
        }
    }
}
