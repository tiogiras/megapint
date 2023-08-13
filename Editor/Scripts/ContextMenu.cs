﻿#if UNITY_EDITOR
using Editor.Scripts.Settings;
using Editor.Scripts.Windows;
using UnityEditor;
using UnityEngine;

namespace Editor.Scripts
{
    public static partial class ContextMenu
    {
        [MenuItem("MegaPint/Open", false, 0)]
        private static void Open() => TryOpen<MegaPintBaseWindow>(false);

        [MenuItem("MegaPint/PackageManager", false, 11)]
        private static void OpenImporter() => MegaPintBaseWindow.OnOpenPackageManager();

        public static MegaPintEditorWindowBase TryOpen<T>(bool utility, string title = "") where T : MegaPintEditorWindowBase
        {
            if (typeof(T) == typeof(MegaPintFirstSteps)) return EditorWindow.GetWindow<T>(utility, title).ShowWindow();

            var exists = MegaPintSettings.Exists();
            
            return ! exists
                ? EditorWindow.GetWindow<MegaPintFirstSteps>(utility, title).ShowWindow() 
                : EditorWindow.GetWindow<T>(utility, title).ShowWindow();
        }

        [MenuItem("MegaPint/TEST", false, 11)]
        private static void TEST()
        {
            Debug.Log(MegaPintSettings.Instance);
        }
        
        [MenuItem("MegaPint/RESET", false, 11)]
        private static void RESET()
        {
            MegaPintSettings.Instance = null;
        }
    }
}
#endif