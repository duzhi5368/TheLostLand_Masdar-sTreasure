using UnityEditor;
using UnityEngine;
//============================================================
namespace FKLib
{
    public static class StatSystemMenu
    {
        [MenuItem("Windows/FK状态编辑器", false, 0)]
        private static void OpenItemEditor()
        {
            StatEditor.ShowWindow();
        }

        [MenuItem("Windows/创建FK状态管理器", false, 1)]
        private static void CreateStatManager()
        {
            GameObject go = new GameObject("Stats Manager");
            go.AddComponent<StatsManager>();
            Selection.activeGameObject = go;
        }

        [MenuItem("Windows/创建FK状态管理器", true)]
        private static bool ValidateCreateStatusSystem()
        {
            return GameObject.FindFirstObjectByType<StatsManager>() == null;
        }
    }
}
