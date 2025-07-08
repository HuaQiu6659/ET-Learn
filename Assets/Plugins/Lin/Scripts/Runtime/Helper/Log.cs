/*
┌────────────────────────────┐
│　Description: 打印辅助
│　Remark: 
└────────────────────────────┘
*/

using UnityEngine;
using Cysharp.Text;

namespace Lin.Runtime.Helper
{
    public static class Log
    {

        [HideInCallstack]
        public static void Debug(this Object target, object message) => UnityEngine.Debug.Log(ZString.Format("<b>[{0} {1}]</b> {2}", target.name, target.GetType().Name, message), target);

        [HideInCallstack]
        public static void Debug(this object target, object message, Object context = null) => UnityEngine.Debug.Log(ZString.Format("<b>[{0}]</b> {1}", target, message), context);

        [HideInCallstack]
        public static void Debug(string title, object message, Object context = null) => UnityEngine.Debug.Log(ZString.Format("<b>[{0}]</b> {1}", title, message), context);


        [HideInCallstack]
        public static void Warning(this Object target, object message) => UnityEngine.Debug.LogWarning(ZString.Format("<b>[{0} {1}]</b> {2}", target.name, target.GetType().Name, message), target);

        [HideInCallstack]
        public static void Warning(this object target, object message, Object context = null) => UnityEngine.Debug.LogWarning(ZString.Format("<b>[{0}]</b> {1}", target, message), context);

        [HideInCallstack]
        public static void Warning(string title, object message, Object context = null) => UnityEngine.Debug.LogWarning(ZString.Format("<b>[{0}]</b> {1}", title, message), context);


        [HideInCallstack]
        public static void Error(this Object target, object message) => UnityEngine.Debug.LogError(ZString.Format("<b>[{0} {1}]</b> {2}", target.name, target.GetType().Name, message), target);

        [HideInCallstack]
        public static void Error(this object target, object message, Object context = null) => UnityEngine.Debug.LogError(ZString.Format("<b>[{0}]</b> {1}", target, message), context);

        [HideInCallstack]
        public static void Error(string title, object message, Object context = null) => UnityEngine.Debug.LogError(ZString.Format("<b>[{0}]</b> {1}", title, message), context);
    }
}