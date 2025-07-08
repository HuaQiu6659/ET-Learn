/*
┌────────────────────────────┐
│　Description: Show comments in project window.
└────────────────────────────┘
*/
using Lin.Editor.Helper;
using Lin.Runtime.Helper;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Lin.Editor.Asset
{
    [InitializeOnLoad]
    public static class AssetSummaryDrawer
    {
        private const float DISPLAY_OFFSET = 25;

        private static Dictionary<string, (string description, string tooltip)> descriptionMap;
        private static HashSet<string> readedList;

        static AssetSummaryDrawer()
        {
            readedList = new HashSet<string>();
            descriptionMap = new Dictionary<string, (string description, string tooltip)>();
            EditorApplication.projectWindowItemOnGUI += OnProjectWindowItemOnGUI;
        }

        [MenuItem("Assets/修改注释")]
        private static void SetAssetDescription()
        {
            var assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
            AssetSummaryWindow.Show(assetPath);
        }

        [MenuItem("Assets/修改注释", validate = true)]
        private static bool SetAssetDescriptionValidate()
        {
            return Selection.activeObject != null;
        }

        private static void OnProjectWindowItemOnGUI(string guid, Rect selectionRect)
        {
            if (string.IsNullOrEmpty(guid))
                return;

            if (descriptionMap.ContainsKey(guid))
            {
                var labelRect = new Rect(selectionRect);
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                string fileName = Path.GetFileNameWithoutExtension(assetPath);
                
                // 创建样式并启用富文本
                var style = new GUIStyle(EditorStyles.miniLabel);
                style.normal.textColor = new Color(0.6f, 0.6f, 0.6f, 1f);
                style.richText = true;
                style.alignment = TextAnchor.MiddleLeft;
                
                // 计算文件名宽度
                string description = descriptionMap[guid].description;
                float fileNameWidth = EditorStyles.label.CalcSize(new GUIContent(fileName)).x;
                
                // 计算注释的显示位置
                labelRect.x = selectionRect.x + fileNameWidth + DISPLAY_OFFSET;
                labelRect.width = selectionRect.width - labelRect.x;
                
                // 对象Tooltip
                var content = new GUIContent(description);
                content.tooltip = descriptionMap[guid].tooltip;
                
                // 绘制注释
                GUI.Label(labelRect, content, style);
            }
            else if (!readedList.Contains(guid))
            {
                readedList.Add(guid);
                var settings = EditorSettings.GetInstance();

                var importer = ImporterHelper.FromGUID(guid);
                var summary = importer.GetDescription();
                string title = summary.title;
                if (!string.IsNullOrEmpty(title))
                    title = $"<b><size={settings.assetSummaryTitleSize}><color=#{summary.titleColor}>{title}</color></size></b>";
                if (importer.assetPath.EndsWith(".cs"))
                {
                    // 读取.cs文件内容
                    string fileContent = File.ReadAllText(importer.assetPath);

                    // 匹配Description注释
                    foreach (var filter in settings.descriptionFilters)
                        FindDescriptions(filter);

                    void FindDescriptions(string filter)
                    {
                        if (fileContent.Contains(filter))
                        {
                            var descMatch = System.Text.RegularExpressions.Regex.Match(
                                fileContent,
                                @$"{filter}([^\n]+)");

                            if (descMatch.Success)
                            {
                                string descValue = descMatch.Groups[1].Value.Trim();
                                if (!string.IsNullOrWhiteSpace(descValue))
                                {
                                    descValue = $"<color=#{settings.scriptDscColor.ToHtmlStringRGB()}>{descValue}</color>";
                                    if (settings.scriptDscBold)
                                        descValue = $"<b>{descValue}</b>";

                                    if (settings.scriptDscItalic)
                                        descValue = $"<i>{descValue}</i>";

                                    title = $"{descValue} {title}";
                                }
                            }
                        }
                    }
                }

                if (!string.IsNullOrEmpty(title))
                    descriptionMap.Add(guid, (title, summary.description));
            }
        }

        public static void Refresh(string guid)
        {
            descriptionMap.Remove(guid);
            readedList.Remove(guid);
        }

        public static void Refresh()
        {
            descriptionMap.Clear();
            readedList.Clear();
        }
    }
}