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
            get { return "Ĭ������"; }
        }

        [Header("��Debug���á�")]
        [InspectorLabel("�Ƿ���ʾDebug��Ϣ")]
        public bool IsShowDebugMessages = true;
    }
}
