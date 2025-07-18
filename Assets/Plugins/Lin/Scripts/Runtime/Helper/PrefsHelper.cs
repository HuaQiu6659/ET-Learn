/*
┌────────────────────────────┐
│　Description: 信息存储辅助
│　Remark: 会将所有信息都序列化成字符串
└────────────────────────────┘
┌──────────────┐                                   
│　ClassName: PrefsHelper
└──────────────┘
*/
using Newtonsoft.Json;
using System;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Scripting;
using ET;

namespace Lin.Runtime.Helper
{
    [Preserve]
    public static class PrefsHelper
    {
        [StaticField]
        private static PrefsUtility utility;

        static PrefsHelper()
        {
            utility =
#if UNITY_EDITOR
                new EditorPrefsUtility();
#else
                new PlayerPrefsUtility();
#endif
        }

        public static void DeleteKey<T>(string key) => utility.Delete<T>(key);

        public static T Get<T>(string key)
        {
            CheckType<T>();
            return utility.Get<T>(key);
        }

        public static T Get<T>(string key, T defaultValue)
        {
            CheckType<T>();
            return utility.Get(key, defaultValue);
        }

        public static T Get<T>(string key, Func<T> createFunc)
        {
            CheckType<T>();
            return utility.Get(key, createFunc);
        }

        public static void Set<T>(string key, T value)
        {
            CheckType<T>();
            utility.Set(key, value);
        }

        private static void CheckType<T>()
        {
            var type = typeof(T);
            // 如果是值类型但不是struct（即内置值类型如int、enum等），直接通过
            if (type.IsValueType && !type.IsLayoutSequential)
                return;
            
            // 其他类型需要检查是否可序列化
            if (!type.Attributes.HasFlag(System.Reflection.TypeAttributes.Serializable))
                throw new Exception($"{type.FullName} has not 'Serializable'.");
        }

        public static string TranslateKey<T>(string key)
        {
            var projectName = new DirectoryInfo(Application.dataPath).Parent.Name;
            return $"{projectName}_{typeof(T).FullName}_{key}".GetLongHashCode().ToString("X");
        }
    }

    abstract class PrefsUtility
    {
        public abstract T Get<T>(string key);

        public abstract T Get<T>(string key, T defaultValue);

        public abstract T Get<T>(string key, Func<T> createFunc);

        public abstract void Set<T>(string key, T value);

        public abstract void Delete<T>(string key);
    }

#if UNITY_EDITOR

    class EditorPrefsUtility : PrefsUtility
    {
        public override T Get<T>(string key) => Get(key, default(T));

        public override T Get<T>(string key, T defaultValue)
        {
            key = TranslateKey<T>(key);
            string path = GetPath(key);
            var json = File.Exists(path) ? File.ReadAllText(path) : null;
            if (string.IsNullOrEmpty(json))
                return defaultValue;

            try
            {
                var result = JsonConvert.DeserializeObject<T>(json);
                return result;
            }
            catch (Exception)
            {
                return defaultValue;
            }
        }

        public override T Get<T>(string key, Func<T> createFunc)
        {
            key = TranslateKey<T>(key);
            string path = GetPath(key);
            var json = File.Exists(path) ? File.ReadAllText(path) : null;
            if (string.IsNullOrEmpty(json))
                return createFunc();

            try
            {
                var result = JsonConvert.DeserializeObject<T>(json);
                return result;
            }
            catch (Exception)
            {
                return createFunc();
            }
        }

        public override void Set<T>(string key, T value)
        {
            key = TranslateKey<T>(key);
            string path = GetPath(key);
            string json = JsonConvert.SerializeObject(value);
            File.WriteAllText(path, json);
        }

        private string TranslateKey<T>(string key) => PrefsHelper.TranslateKey<T>(key);

        private string GetPath(string key)
        {
            var dir = $"{new DirectoryInfo(Application.dataPath).Parent.FullName}/EditorPrefs";
            IOHelper.InsureExist(dir, false);
            return $"{dir}/{key}";
        }

        public override void Delete<T>(string key)
        {
            key = TranslateKey<T>(key);
            string path = GetPath(key);
            if (File.Exists(path))
                File.Delete(path);
        }
    }

#endif

    class PlayerPrefsUtility : PrefsUtility
    {
        private Dictionary<Type, object> archivesMap = new Dictionary<Type, object>();

        private PrefsArchive<T> GetArchive<T>()
        {
            var type = typeof(T);
            PrefsArchive<T> archive;

            if (!archivesMap.ContainsKey(type))
            {
                archive = PrefsArchive<T>.Load();
                archivesMap.Add(type, archive);
            }
            else
                archive = archivesMap[type] as PrefsArchive<T>;

            return archive;
        }

        public override T Get<T>(string key) => Get(key, default(T));

        public override T Get<T>(string key, T defaultValue)
        {
            var archive = GetArchive<T>();
            return archive.Get(key, defaultValue);
        }

        public override T Get<T>(string key, Func<T> createFunc)
        {
            var archive = GetArchive<T>();
            if (!archive.ContainsKey(key))
                Set(key, createFunc());

            return archive.Get(key);
        }

        public override void Set<T>(string key, T value)
        {
            var archive = GetArchive<T>();
            archive.Set(key, value);
        }

        public override void Delete<T>(string key)
        {
            var archive = GetArchive<T>();
            archive.Remove(key);
        }

        [Serializable]
        class PrefsArchive<T> : Dictionary<string, T>
        {
            private string filePath;
            private object locker;
            private const byte OFFSET = 7;

            public static PrefsArchive<T> Load()
            {
                string filePath = GetPath();
                PrefsArchive<T> result;
#if UNITY_WEBGL
                string key = typeof(T).FullName;
                if (PlayerPrefs.HasKey(key))
                {
                    string json = PlayerPrefs.GetString(key);
                    result = JsonConvert.DeserializeObject<PrefsArchive<T>>(json);
                }
                else
                    result = new PrefsArchive<T>();
#else
                if (File.Exists(filePath))
                {
                    var bytes = File.ReadAllBytes(filePath);
                    Translate(bytes, filePath);
                    string json = Encoding.Default.GetString(bytes);
                    result = JsonConvert.DeserializeObject<PrefsArchive<T>>(json);
                }
                else
                    result = new PrefsArchive<T>();
#endif
                result.filePath = filePath;
                result.locker = new object();

                return result;
            }

#if !UNITY_WEBGL
            //简易加密，解密
            private static void Translate(byte[] bytes, string path)
            {
                Type type = typeof(T);
                byte flags = (byte)(type.FullName.Length % byte.MaxValue);
                byte offset = (byte)((flags + path.Length) % byte.MaxValue);
                offset = offset == 0 ? OFFSET : offset;
                for (int i = 0; i < bytes.Length; i++)
                    bytes[i] ^= offset;
            }
#endif
            private static void Save(PrefsArchive<T> archive)
            {
#if UNITY_WEBGL
                string json = JsonConvert.SerializeObject(archive);
                PlayerPrefs.SetString(typeof(T).FullName, json);
#else
                string json = JsonConvert.SerializeObject(archive);
                string dir = Path.GetDirectoryName(archive.filePath);
                IOHelper.InsureExist(dir, false);
                var bytes = Encoding.Default.GetBytes(json);
                Translate(bytes, archive.filePath);
                File.WriteAllBytes(archive.filePath, bytes);
#endif
            }

            private static string GetPath()
            {
                StringBuilder stringBuilder = new StringBuilder(Application.persistentDataPath);
                stringBuilder.Append("/Temps/");
                stringBuilder.Append(typeof(T).FullName.GetLongHashCode().ToString("X"));
                return stringBuilder.ToString();
            }

            public void Set(string key, T value)
            {
                lock (locker)
                {
                    if (!ContainsKey(key))
                        Add(key, default);

                    this[key] = value;
                    Save(this);
                }
            }

            public T Get(string key, T defaultValue = default)
            {
                lock (locker)
                {
                    if (TryGetValue(key, out T result))
                        return result;

                    return defaultValue;
                }
            }
        }
    }
}