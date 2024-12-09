using System;
using UnityEngine;
//============================================================
namespace FKLib
{
    [Serializable]
    public class DefaultSettings : ISettings
    {
        public override string Name
        {
            get { return "默认设置"; }
        }

        [Header("【Debug设置】")]
        [InspectorLabel("是否显示Debug信息")]
        public bool IsShowDebugMessages = true;
    }
}
