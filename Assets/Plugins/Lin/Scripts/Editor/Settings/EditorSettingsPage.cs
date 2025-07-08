using Lin.Editor.Asset;
using Lin.Runtime;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Lin.Editor
{
    static class EditorSettingsPage
    {
        private static EditorSettings settings;
        private static SerializedObject settingsObject;

        //Asset Summary
        private static SerializedProperty descriptionFilters;
        private static SerializedProperty assetSummaryTitleSize;
        private static SerializedProperty scriptDscColor;
        private static SerializedProperty scriptDscBold;
        private static SerializedProperty scriptDscItalic;

        [SettingsProvider]
        private static SettingsProvider InProjectSettings()
        {
            return new SettingsProvider("Project/Lin Settings", SettingsScope.Project)
            {
                activateHandler = OnSettingsActive,
                guiHandler = OnSettingsGUI,
            };
        }

        [MenuItem("Lin/Settings", priority = 100)]
        private static void OpenSettingsPage() => SettingsService.OpenProjectSettings("Project/Lin Settings");

        private static void OnSettingsActive(string searchContext, VisualElement rootElement)
        {
            if (settings is null)
            {
                settings = EditorSettings.GetInstance();
                settingsObject = new SerializedObject(settings);
                descriptionFilters = settingsObject.FindProperty(nameof(settings.descriptionFilters));
                assetSummaryTitleSize = settingsObject.FindProperty(nameof(settings.assetSummaryTitleSize));
                scriptDscColor = settingsObject.FindProperty(nameof(settings.scriptDscColor));
                scriptDscBold = settingsObject.FindProperty(nameof(settings.scriptDscBold));
                scriptDscItalic = settingsObject.FindProperty(nameof(settings.scriptDscItalic));
            }
        }

        private static void OnSettingsGUI(string searchContext)
        {
            GUIStyle title = new GUIStyle(GUI.skin.label)
            {
                fontSize = 12,
                fontStyle = FontStyle.Bold
            };

            Titile("AssetSummary");
            EditorGUILayout.PropertyField(assetSummaryTitleSize, new GUIContent("注释字体大小"));

            Titile("Script Descriptions");
            EditorGUILayout.PropertyField(scriptDscColor, new GUIContent("颜色"));
            EditorGUILayout.PropertyField(scriptDscBold, new GUIContent("粗体"));
            EditorGUILayout.PropertyField(scriptDscItalic, new GUIContent("斜体"));
            EditorGUILayout.PropertyField(descriptionFilters, new GUIContent("描述标识"));

            if (settingsObject.ApplyModifiedProperties())
                AssetSummaryDrawer.Refresh();

            void Titile(string message, float space = 10)
            {
                GUILayout.Space(space);
                GUILayout.Label(message, title);
            }
        }
    }
}
