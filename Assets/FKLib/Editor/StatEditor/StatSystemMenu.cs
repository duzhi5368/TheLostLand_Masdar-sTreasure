using UnityEditor;
using UnityEngine;
//============================================================
namespace FKLib
{
    public static class StatSystemMenu
    {
        [MenuItem("Windows/FK״̬�༭��", false, 0)]
        private static void OpenItemEditor()
        {
            StatEditor.ShowWindow();
        }

        [MenuItem("Windows/����FK״̬������", false, 1)]
        private static void CreateStatManager()
        {
            GameObject go = new GameObject("Stats Manager");
            go.AddComponent<StatsManager>();
            Selection.activeGameObject = go;
        }

        [MenuItem("Windows/����FK״̬������", true)]
        private static bool ValidateCreateStatusSystem()
        {
            return GameObject.FindFirstObjectByType<StatsManager>() == null;
        }
    }
}
