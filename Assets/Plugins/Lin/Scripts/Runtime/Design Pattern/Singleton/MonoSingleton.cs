/*
┌────────────────────────────┐
│　Description: Mono单例
│　Author: 花球i
│　Remark: 
└────────────────────────────┘
┌──────────────┐                                   
│　ClassName: MonoSingleton
└──────────────┘
*/

using UnityEngine;

namespace Lin.Runtime.DesinPattern.Singleton
{
    public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
#pragma warning disable ET0015 // Static字段声明需要标记标签
        private static T instance;

        public static T GetInstance()
        {
            if (!instance)
            {
                instance = FindAnyObjectByType<T>() ?? new GameObject($"[{typeof(T).Name}]").AddComponent<T>();
#if UNITY_EDITOR
                if (!Application.isPlaying)
                    return instance;
#endif
                DontDestroyOnLoad(instance.gameObject);
            }
            return instance;
        }

        protected virtual void Init() { }

        private void Awake()
        {
            var ins = GetInstance();
            if (ins != this)
            {
                Destroy(gameObject);
                return;
            }

            Init();
        }

        protected virtual void OnDestroy()
        {
            if (instance == this)
                instance = null;
        }

        public void Destroy()
        {
            if (instance == this)
                Destroy(gameObject);
        }

        public static bool HasInstance() => instance;
    }
}