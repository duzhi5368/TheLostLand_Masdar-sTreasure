using UnityEngine;
//============================================================
namespace FKLib
{
    // 编辑器主面板抽象基类
    public interface ICollectionEditor
    {
        string ToolbarName { get; }

        void OnGUI(Rect position);
        void OnEnable();
        void OnDisable();
        void OnDestroy();
    }
}
