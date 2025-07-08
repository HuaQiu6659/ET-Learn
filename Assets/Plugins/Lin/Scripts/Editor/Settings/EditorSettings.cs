using Lin.Runtime.Helper;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Lin.Editor
{
    public class EditorSettings : ScriptableObject
    {
        //----------------- 资源注释 -----------------
        public List<string> descriptionFilters;
        public int assetSummaryTitleSize = 14;
        public Color scriptDscColor;
        public bool scriptDscBold;
        public bool scriptDscItalic;

        public static EditorSettings GetInstance()
        {
            var settings = AssetDatabase.FindAssets($"t:{nameof(EditorSettings)}");
            if (settings.Length < 1)
            {
                var toSave = CreateInstance<EditorSettings>();
                toSave.descriptionFilters = new List<string>() { "Description: ", "Description：", "功能说明: " };
                IOHelper.InsureExist("Assets/Settings", false);
                AssetDatabase.CreateAsset(toSave, $"Assets/Settings/{nameof(EditorSettings)}.asset");
                AssetDatabase.Refresh();
                return toSave;
            }
            string path = AssetDatabase.GUIDToAssetPath(settings.First());
            return AssetDatabase.LoadAssetAtPath<EditorSettings>(path);
        }
    }
}
