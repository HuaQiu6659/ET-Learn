/*
┌────────────────────────────┐
│　Description: 展示所有Type类型脚本中的数值
│　Remark: 
└────────────────────────────┘
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace ET
{
    public enum SortMode
    {
        ValueAscending,    // 数值从小到大
        ValueDescending,   // 数值从大到小
        NameAscending,     // 属性名顺序
        NameDescending,    // 属性名倒序
        FileNameSorting    // 文件名排序
    }
    
    public class TypesDisplayerWindow : OdinEditorWindow
    {
        [MenuItem("ET/Types")]
        public static void ShowWindow()
        {
            GetWindow<TypesDisplayerWindow>("Types Displayer").Show();
        }

        [Serializable]
        public class TypeInfo
        {
            public string ClassName;
            public string FieldName;
            public int Value;
            public string FilePath;
            
            public TypeInfo(string className, string fieldName, int value, string filePath)
            {
                ClassName = className;
                FieldName = fieldName;
                Value = value;
                FilePath = filePath;
            }
        }

        private List<TypeInfo> typeInfos = new List<TypeInfo>();
        private Dictionary<string, bool> foldoutStates = new Dictionary<string, bool>();
        private Vector2 mainScrollPosition = Vector2.zero;
        private Dictionary<string, Vector2> classScrollPositions = new Dictionary<string, Vector2>();
        private Dictionary<string, SortMode> classSortModes = new Dictionary<string, SortMode>();

        [Button("刷新数据", ButtonSizes.Large)]
        private void RefreshData()
        {
            typeInfos.Clear();
            ScanTypeFiles();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            RefreshData();
        }

        private void ScanTypeFiles()
        {
            string[] searchPaths = { "Assets", "Packages" };
            
            foreach (string searchPath in searchPaths)
            {
                string fullPath = Path.Combine(Application.dataPath.Replace("Assets", ""), searchPath);
                if (Directory.Exists(fullPath))
                {
                    ScanDirectory(fullPath, searchPath);
                }
            }
            
            // 按类名排序
            typeInfos = typeInfos.OrderBy(t => t.ClassName).ThenBy(t => t.FieldName).ToList();
        }

        private void ScanDirectory(string directoryPath, string relativePath)
        {
            try
            {
                // 搜索所有以Type结尾的.cs文件
                string[] files = Directory.GetFiles(directoryPath, "*Type.cs", SearchOption.AllDirectories);
                
                foreach (string file in files)
                {
                    ProcessTypeFile(file, relativePath);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"扫描目录时出错: {directoryPath}, 错误: {e.Message}");
            }
        }

        private void ProcessTypeFile(string filePath, string basePath)
        {
            try
            {
                string content = File.ReadAllText(filePath);
                
                // 检查是否是static partial类
                if (!IsStaticPartialClass(content))
                    return;

                // 提取类名
                string className = ExtractClassName(content);
                if (string.IsNullOrEmpty(className))
                    return;

                // 提取const int字段
                var constFields = ExtractConstIntFields(content, className);
                
                string relativeFilePath = GetRelativeFilePath(filePath, basePath);
                
                foreach (var field in constFields)
                {
                    typeInfos.Add(new TypeInfo(className, field.Key, field.Value, relativeFilePath));
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"处理文件时出错: {filePath}, 错误: {e.Message}");
            }
        }

        private bool IsStaticPartialClass(string content)
        {
            // 匹配static partial class模式，支持更灵活的匹配
            string pattern = @"public\s+static\s+partial\s+class\s+\w*Type";
            bool result = Regex.IsMatch(content, pattern);
            
            // 如果第一个模式不匹配，尝试更宽松的模式
            if (!result)
            {
                // 尝试匹配没有public的情况或者顺序不同的情况
                string[] patterns = {
                    @"static\s+partial\s+class\s+\w*Type",
                    @"partial\s+static\s+class\s+\w*Type",
                    @"public\s+partial\s+static\s+class\s+\w*Type"
                };
                
                foreach (string p in patterns)
                {
                    if (Regex.IsMatch(content, p))
                    {
                        result = true;
                        break;
                    }
                }
            }
            
            return result;
        }

        private string ExtractClassName(string content)
        {
            // 提取类名，支持多种模式
            string[] patterns = {
                @"public\s+static\s+partial\s+class\s+(\w*Type)",
                @"static\s+partial\s+class\s+(\w*Type)",
                @"partial\s+static\s+class\s+(\w*Type)",
                @"public\s+partial\s+static\s+class\s+(\w*Type)"
            };
            
            foreach (string pattern in patterns)
            {
                Match match = Regex.Match(content, pattern);
                if (match.Success)
                {
                    return match.Groups[1].Value;
                }
            }
            
            return null;
        }
        
        private string ExtractNamespace(string content)
        {
            // 提取命名空间
            string pattern = @"namespace\s+([\w\.]+)";
            Match match = Regex.Match(content, pattern);
            
            if (match.Success)
            {
                return match.Groups[1].Value;
            }
            
            // 如果没有找到命名空间，默认返回"ET"
            return "ET";
        }

        private Dictionary<string, int> ExtractConstIntFields(string content, string className)
        {
            var fields = new Dictionary<string, int>();
            
            // 匹配const int字段，支持多种访问修饰符
            string[] patterns = {
                @"public\s+const\s+int\s+(\w+)\s*=\s*([^;]+);",
                @"const\s+int\s+(\w+)\s*=\s*([^;]+);",
                @"internal\s+const\s+int\s+(\w+)\s*=\s*([^;]+);",
                @"private\s+const\s+int\s+(\w+)\s*=\s*([^;]+);"
            };

            // 获取类的命名空间
            string classNamespace = ExtractNamespace(content);
            
            foreach (string pattern in patterns)
            {
                MatchCollection matches = Regex.Matches(content, pattern);
                
                foreach (Match match in matches)
                {
                    string fieldName = match.Groups[1].Value;
                    
                    // 避免重复添加同名字段
                    if (!fields.ContainsKey(fieldName))
                    {
                        // 尝试使用反射获取const int数值
                        if (TryGetConstValueByReflection($"{classNamespace}.{className}", fieldName, out int reflectionValue))
                        {
                            fields[fieldName] = reflectionValue;
                        }
                    }
                }
            }
            
            return fields;
        }
        
        private bool TryGetConstValueByReflection(string fullClassName, string fieldName, out int value)
        {
            value = 0;
            
            try
            {
                // 在所有已加载的程序集中查找类型
                Type targetType = null;
                foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
                {
                    targetType = assembly.GetType(fullClassName);
                    if (targetType != null)
                        break;
                }
                
                if (targetType == null)
                    return false;
                
                // 获取字段信息
                var fieldInfo = targetType.GetField(fieldName, 
                    BindingFlags.Public | 
                    BindingFlags.Static | 
                    BindingFlags.FlattenHierarchy);
                
                if (fieldInfo != null && fieldInfo.IsLiteral && !fieldInfo.IsInitOnly)
                {
                    // 确保是const int字段
                    if (fieldInfo.FieldType == typeof(int))
                    {
                        value = (int)fieldInfo.GetRawConstantValue();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"反射获取常量值失败 {fullClassName}.{fieldName}: {ex.Message}");
            }
            
            return false;
        }
        
        private bool EvaluateSimpleExpression(string expression, out int result)
        {
            result = 0;
            expression = expression.Trim();
            
            // 处理简单的乘法和加法表达式，如 "9 * 1000 + 1"
            if (expression.Contains("*") && expression.Contains("+"))
            {
                // 解析 "a * b + c" 格式
                var parts = expression.Split('+');
                if (parts.Length == 2)
                {
                    var leftPart = parts[0].Trim();
                    var rightPart = parts[1].Trim();
                    
                    if (leftPart.Contains("*"))
                    {
                        var multiplyParts = leftPart.Split('*');
                        if (multiplyParts.Length == 2 && 
                            int.TryParse(multiplyParts[0].Trim(), out int a) &&
                            int.TryParse(multiplyParts[1].Trim(), out int b) &&
                            int.TryParse(rightPart, out int c))
                        {
                            result = a * b + c;
                            return true;
                        }
                    }
                }
            }
            else if (expression.Contains("*"))
            {
                // 处理简单乘法 "a * b"
                var parts = expression.Split('*');
                if (parts.Length == 2 &&
                    int.TryParse(parts[0].Trim(), out int a) &&
                    int.TryParse(parts[1].Trim(), out int b))
                {
                    result = a * b;
                    return true;
                }
            }
            else if (expression.Contains("+"))
            {
                // 处理简单加法 "a + b"
                var parts = expression.Split('+');
                if (parts.Length == 2 &&
                    int.TryParse(parts[0].Trim(), out int a) &&
                    int.TryParse(parts[1].Trim(), out int b))
                {
                    result = a + b;
                    return true;
                }
            }
            else
            {
                // 尝试直接解析为整数
                return int.TryParse(expression, out result);
            }
            
            return false;
        }

        private string GetRelativeFilePath(string fullPath, string basePath)
        {
            string projectPath = Application.dataPath.Replace("Assets", "");
            return fullPath.Replace(projectPath, "").Replace("\\", "/");
        }
        
        private IEnumerable<TypeInfo> SortTypeInfos(IEnumerable<TypeInfo> typeInfos, SortMode sortMode)
        {
            switch (sortMode)
            {
                case SortMode.ValueAscending:
                    return typeInfos.OrderBy(t => t.Value).ThenBy(t => t.FieldName);
                case SortMode.ValueDescending:
                    return typeInfos.OrderByDescending(t => t.Value).ThenBy(t => t.FieldName);
                case SortMode.NameAscending:
                    return typeInfos.OrderBy(t => t.FieldName).ThenBy(t => t.Value);
                case SortMode.NameDescending:
                    return typeInfos.OrderByDescending(t => t.FieldName).ThenBy(t => t.Value);
                case SortMode.FileNameSorting:
                    return typeInfos.OrderBy(t => t.FilePath).ThenBy(t => t.FieldName);
                default:
                    return typeInfos.OrderBy(t => t.Value).ThenBy(t => t.FieldName);
            }
        }
        
        private string GetSortModeDisplayName(SortMode sortMode)
        {
            switch (sortMode)
            {
                case SortMode.ValueAscending: return "数值↑";
                case SortMode.ValueDescending: return "数值↓";
                case SortMode.NameAscending: return "名称↑";
                case SortMode.NameDescending: return "名称↓";
                case SortMode.FileNameSorting: return "文件";
                default: return "数值↑";
            }
        }
        [OnInspectorGUI]
        private void Draw()
        {
            if (typeInfos.Count == 0)
            {
                EditorGUILayout.HelpBox("没有找到符合条件的Type文件", MessageType.Info);
                return;
            }

            // 按类名分组显示，并按数值从小到大排列
            var groupedTypes = typeInfos.GroupBy(t => t.ClassName).OrderBy(g => g.Key);
            var classNames = groupedTypes.Select(g => g.Key).ToList();

            EditorGUILayout.LabelField("Type类型数值列表", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            // 5. 顶部添加Toggles，展示所有类型名作为Toggle选项
            EditorGUILayout.LabelField("类型显示控制:", EditorStyles.boldLabel);
            
            // 使用更合理的布局来避免重叠
            int togglesPerRow = Mathf.Max(1, (int)(position.width / 150)); // 增加每个toggle的宽度
            EditorGUILayout.Space();
            // 4. 所有类ScrollView放置在以窗口大小为大小的ScrollView中
            mainScrollPosition = EditorGUILayout.BeginScrollView(mainScrollPosition);

            foreach (var group in groupedTypes)
            {
                string className = group.Key;

                // 初始化foldout状态（默认折叠）
                if (!foldoutStates.ContainsKey(className))
                    foldoutStates[className] = false;

                // 初始化scroll位置
                if (!classScrollPositions.ContainsKey(className))
                    classScrollPositions[className] = Vector2.zero;

                // 获取当前类的排序模式
                if (!classSortModes.ContainsKey(className))
                {
                    classSortModes[className] = SortMode.ValueAscending;
                }
                
                // 水平布局：foldout + 排序按钮
                EditorGUILayout.BeginHorizontal();
                foldoutStates[className] = EditorGUILayout.Foldout(foldoutStates[className],
                    $"{className} ({group.Count()} 项)", true, EditorStyles.foldoutHeader);
                
                // 排序按钮
                var currentSortMode = classSortModes[className];
                if (GUILayout.Button(GetSortModeDisplayName(currentSortMode), GUILayout.Width(60)))
                {
                    // 循环切换排序模式
                    var sortModes = System.Enum.GetValues(typeof(SortMode)).Cast<SortMode>().ToArray();
                    int currentIndex = Array.IndexOf(sortModes, currentSortMode);
                    int nextIndex = (currentIndex + 1) % sortModes.Length;
                    classSortModes[className] = sortModes[nextIndex];
                }
                EditorGUILayout.EndHorizontal();

                if (foldoutStates[className])
                {
                    EditorGUI.indentLevel++;

                    // 3. 所有数值放置在各自类型的ScrollView中，高度都为150，可被foldout折叠
                    classScrollPositions[className] = EditorGUILayout.BeginScrollView(
                        classScrollPositions[className],
                        GUILayout.Height(150));

                    // 根据当前排序模式排列
                    var sortedTypeInfos = SortTypeInfos(group, classSortModes[className]);
                    foreach (var typeInfo in sortedTypeInfos)
                    {
                        EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField($"{typeInfo.FieldName}", GUILayout.Width(200));
                        EditorGUILayout.LabelField($"{typeInfo.Value}", GUILayout.Width(100));
                        if (GUILayout.Button("定位", GUILayout.Width(50)))
                        {
                            // 在Project窗口中定位文件
                            string assetPath = typeInfo.FilePath;
                            var asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(assetPath);
                            if (asset != null)
                            {
                                EditorGUIUtility.PingObject(asset);
                                Selection.activeObject = asset;
                            }
                        }
                        EditorGUILayout.LabelField(typeInfo.FilePath, EditorStyles.miniLabel);
                        EditorGUILayout.EndHorizontal();
                    }

                    EditorGUILayout.EndScrollView();
                    EditorGUI.indentLevel--;
                }

                EditorGUILayout.Space();
            }

            EditorGUILayout.EndScrollView();
        }
    }
}