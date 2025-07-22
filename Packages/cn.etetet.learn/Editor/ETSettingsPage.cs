using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ET
{
    public static class ETSettingsPage
    {
        private static SerializedObject globalConfig;
        private static SerializedProperty codeMode;
        private static SerializedProperty sceneName;
        private static SerializedProperty address;

        private static SerializedObject yooConfig;
        private static SerializedProperty editorMode;
        private static SerializedProperty runtimeMode;
        private static SerializedProperty cdnUri;

        [SettingsProvider]
        private static SettingsProvider InProjectSettings()
        {
            return new SettingsProvider("Project/ET Settings", SettingsScope.Project)
            {
                activateHandler = OnSettingsActive,
                guiHandler = OnSettingsGUI,
            };
        }

        [MenuItem("ET/Settings", priority = 100)]
        private static void OpenSettingsPage() => SettingsService.OpenProjectSettings("Project/ET Settings");

        private static void OnSettingsGUI(string obj)
        {

            GUIStyle title = new GUIStyle(GUI.skin.label)
            {
                fontSize = 12,
                fontStyle = FontStyle.Bold
            };

            Titile("ET", 0);
            EditorGUILayout.PropertyField(codeMode);
            EditorGUILayout.PropertyField(sceneName);
            EditorGUILayout.PropertyField(address, new GUIContent("Server Address"));

            Titile("YooAsset", 10);
            EditorGUILayout.PropertyField(editorMode, new GUIContent("编辑器模式"));
            EditorGUILayout.PropertyField(runtimeMode, new GUIContent("实机运行"));
            EditorGUILayout.PropertyField(cdnUri, new GUIContent("CDN"));

            if (globalConfig.ApplyModifiedProperties() || yooConfig.ApplyModifiedProperties())
                AssetDatabase.Refresh();

            void Titile(string message, float space = 10)
            {
                GUILayout.Space(space);
                GUILayout.Label(message, title);
            }
        }

        private static void OnSettingsActive(string arg1, VisualElement element)
        {
            globalConfig = new SerializedObject(Resources.Load("GlobalConfig"));
            codeMode = globalConfig.FindProperty("CodeMode");
            sceneName = globalConfig.FindProperty("SceneName");
            address = globalConfig.FindProperty("Address");

            yooConfig = new SerializedObject(Resources.Load("YooConfig"));
            runtimeMode = yooConfig.FindProperty(nameof(runtimeMode));
            editorMode = yooConfig.FindProperty(nameof(editorMode));
            cdnUri = yooConfig.FindProperty("url");
        }
    }

}
