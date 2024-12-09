using UnityEngine;
//============================================================
namespace FKLib
{
    public class ExampleDialogBox : MonoBehaviour
    {
        public string Title;
        [TextArea]
        public string Text;
        public Sprite Icon;
        public string[] Options;

        private DialogBox _dialogBox;

        private void Start()
        {
            _dialogBox = FindFirstObjectByType<DialogBox>();
        }

        public void Show()
        {
            _dialogBox.Show(Title, Text, Icon, null, Options);
        }

        public void ShowWithCallback()
        {
            _dialogBox.Show(Title, Text, Icon, OnDialogResult, Options);
        }

        private void OnDialogResult(int index)
        {
            _dialogBox.Show("Result", "Callback Result: " + Options[index], Icon, null, "OK");
        }
    }
}