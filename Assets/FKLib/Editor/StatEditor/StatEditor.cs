using UnityEditor;
using UnityEngine;
//============================================================
namespace FKLib
{
    public class StatEditor : EditorWindow
    {
        private StatInspector _statInspector;

        public static void ShowWindow()
        {
            StatEditor[] objArray = Resources.FindObjectsOfTypeAll<StatEditor>();
            StatEditor editor = (objArray.Length <= 0 ? CreateInstance<StatEditor>() : objArray[0]);
            editor.hideFlags = HideFlags.HideAndDontSave;
            editor.minSize = new Vector2(690, 300);
            editor.titleContent = new GUIContent("×´Ì¬±à¼­Æ÷");
            editor.Show();
        }

        private void OnEnable()
        {
            _statInspector = new StatInspector();
            _statInspector.OnEnable();
        }

        private void OnDisable()
        {
            _statInspector.OnDisable();
        }

        private void OnDestroy()
        {
            _statInspector.OnDestroy();
        }

        private void Update()
        {
            if (mouseOverWindow == this)
                Repaint();
        }

        private void OnGUI()
        {
            _statInspector.OnGUI(position);
        }
    }
}
