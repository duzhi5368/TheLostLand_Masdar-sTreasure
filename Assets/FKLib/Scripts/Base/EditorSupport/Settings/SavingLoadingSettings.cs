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
            get { return "����ͼ�������"; }
        }

        [Header("���Զ��������á�")]
        [InspectorLabel("�Ƿ��Զ��������״̬")]
        public bool IsAutoSave = true;
        [InspectorLabel("�Զ�������ʱ��: ��")]
        public float SavingRate = 60f;
        [InspectorLabel("����Key")]
        public string SavingKey = "Player";
    }
}
