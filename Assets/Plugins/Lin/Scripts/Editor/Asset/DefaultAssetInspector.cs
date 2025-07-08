using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using Lin.Editor.Helper;
using Object = UnityEngine.Object;
using Lin.Runtime;

namespace Lin.Editor.Asset
{
    [CustomEditor(typeof(DefaultAsset), true)]
    public class DefaultAssetInspector : UnityEditor.Editor
    {
        private Dictionary<string, string> customUris;
        private AssetSummary assetSummary; // 存储自定义注释
        private string path;
        private AssetImporter importer;

        private void OnEnable()
        {
            path = AssetDatabase.GetAssetPath(target);
            importer = AssetImporter.GetAtPath(path);

            Refresh();

            //检查是否为文件夹
            if (Directory.Exists(path))
            {
                data = Factory<Data>.Get();
                LoadFiles(data, AssetDatabase.GetAssetPath(Selection.activeObject));
            }
        }

        private void OnDisable()
        {
            RecycleDatas(data);

            void RecycleDatas(Data data)
            {
                if (data is null)
                    return;

                if (data.childs is not null)
                {
                    foreach (var child in data.childs)
                        RecycleDatas(child);

                    data.childs.Clear();
                }

                Factory<Data>.Release(data);
            }
        }

        protected override void OnHeaderGUI()
        {
            if (!string.IsNullOrEmpty(assetSummary.title))
            {
                GUILayout.Space(10);
                GUILayout.Label($"路径: {path}", EditorStyles.boldLabel);
                GUILayout.BeginVertical();
                GUILayout.Space(10);
                // 使用支持富文本的样式显示标题
                GUIStyle richTextStyle = new GUIStyle(EditorStyles.label)
                {
                    richText = true,
                    wordWrap = true
                };
                GUILayout.Label( assetSummary.GetRichTitle(), richTextStyle);
                GUILayout.Label(assetSummary.description, richTextStyle);
                GUILayout.Space(10);

                GUILayout.Label($"<i>更新时间: {assetSummary.updateTime}</i>", richTextStyle);
                GUILayout.Label($"<i>创建时间: {assetSummary.createTime}</i>", richTextStyle);

                GUILayout.EndVertical();
            }

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("修改备注"))
                AssetSummaryWindow.Show(path);
            if (GUILayout.Button("添加uri"))
                AssetMessageInputWindow.Show(path, string.Empty, string.Empty, (key, uri) => importer.SetUri(key,uri), Refresh);
            GUILayout.EndHorizontal();

            if (customUris.Count > 0)
            {
                GUILayout.Space(10);
                foreach (var pair in customUris)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label(pair.Key);
                    GUILayout.Label(pair.Value);
                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("修改"))
                    {
                        string oldKey = pair.Key;
                        AssetMessageInputWindow.Show(path, 
                            key: pair.Key, 
                            defaultMessage: pair.Value, 
                            save: (key, uri) =>
                            {
                                importer.RemoveUri(oldKey);
                                importer.SetUri(key, uri);
                            }, 
                            onWrited: Refresh);
                    }
                    if (GUILayout.Button("访问"))
                        Application.OpenURL(pair.Value);
                    if (GUILayout.Button("移除"))
                        importer.RemoveUri(pair.Key);
                    GUILayout.EndHorizontal();
                }
            }

            base.OnHeaderGUI();
        }

        public override void OnInspectorGUI()
        {
            //文件夹
            string path = AssetDatabase.GetAssetPath(target);
            if (Directory.Exists(path))
            {
                GUI.enabled = true;
                EditorGUIUtility.SetIconSize(Vector2.one * 16);
                DrawData(data);
            }
        }

        private void Refresh()
        {
            assetSummary = importer.GetDescription();
            //customComment = importer.GetDescription();
            customUris = importer.GetUris();
        }

        #region - 文件夹拓展 -

        private Data data;
        private Data selectData;

        private void LoadFiles(Data data, string currentPath, int indent = 0)
        {
            if (string.IsNullOrEmpty(currentPath))
                return;

            GUIContent content = GetGUIContent(currentPath);

            if (content != null)
            {
                data.indent = indent;
                data.content = content;
                data.assetPath = currentPath;
            }
            var files = Directory.GetFiles(currentPath);
            var directories = Directory.GetDirectories(currentPath);
            data.childs = data.childs ?? new List<Data>(files.Length + directories.Length);

            foreach (var path in Directory.GetFiles(currentPath))
            {
                content = GetGUIContent(path);
                if (content != null)
                {
                    Data child = new Data();
                    child.indent = indent + 1;
                    child.content = content;
                    child.assetPath = path;
                    data.childs.Add(child);
                }
            }


            foreach (var path in Directory.GetDirectories(currentPath))
            {
                Data childDir = new Data();
                data.childs.Add(childDir);
                LoadFiles(childDir, path, indent + 1);
            }
        }

        private void DrawData(Data data)
        {
            if (data.content != null)
            {
                EditorGUI.indentLevel = data.indent;
                DrawGUIData(data);
            }

            for (int i = 0; i < data.childs.Count; i++)
            {
                Data child = data.childs[i];
                if (child.content != null)
                {
                    EditorGUI.indentLevel = child.indent;
                    if (child.childs != null && data.childs.Count > 0)
                        DrawData(child);
                    else
                        DrawGUIData(child);
                }
            }
        }

        private void DrawGUIData(Data data)
        {
            GUIStyle style = "Label";
            Rect rt = GUILayoutUtility.GetRect(data.content, style);
            if (data.isSelected)
            {
                EditorGUI.DrawRect(rt, Color.gray);
            }

            rt.x += (16 * EditorGUI.indentLevel);
            if (GUI.Button(rt, data.content, style))
            {
                if (selectData != null)
                    selectData.isSelected = false;
                data.isSelected = true;

                EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<Object>(data.assetPath));

                if (selectData == data)
                {
                    // 双击打开资源
                    Object obj = AssetDatabase.LoadMainAssetAtPath(selectData.assetPath);
                    if (obj != null)
                        AssetDatabase.OpenAsset(obj);
                }
                else
                    selectData = data;
            }
        }

        private GUIContent GetGUIContent(string path)
        {
            if (path.EndsWith(".meta"))
                return null;

            return new GUIContent(Path.GetFileNameWithoutExtension(path), AssetDatabase.GetCachedIcon(path));
        }

        private class Data
        {
            public bool isSelected = false;
            public int indent = 0;
            public GUIContent content;
            public string assetPath;
            public List<Data> childs;
        }

        #endregion
    }
}