/*
┌────────────────────────────┐
│　Description: 
│　Remark: 
└────────────────────────────┘
*/
using System;
using UnityEditor;
using UnityEditor.Callbacks;

namespace ET
{
    public static class OnScriptReload
    {
        private const string SCRIPT_RELOAD_KEY = nameof(SCRIPT_RELOAD_KEY);

        [DidReloadScripts]
        private static void OnReload()
        {
            EditorPrefs.SetString(SCRIPT_RELOAD_KEY, DateTime.Now.ToString());
        }
    }
}