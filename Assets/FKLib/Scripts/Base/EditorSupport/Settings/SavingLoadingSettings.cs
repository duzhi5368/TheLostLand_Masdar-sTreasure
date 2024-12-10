using System;
using UnityEngine;
//============================================================
namespace FKLib
{
    [Serializable]
    public class SavingLoadingSettings : ISettings
    {
        public override string Name
        {
            get { return "保存和加载设置"; }
        }

        [Header("【自动保存设置】")]
        [InspectorLabel("是否自动保存玩家状态")]
        public bool IsAutoSave = true;
        [InspectorLabel("自动保存间隔时间: 秒")]
        public float SavingRate = 60f;
        [InspectorLabel("保存Key")]
        public string SavingKey = "Player";
    }
}
