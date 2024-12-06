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
            get { return "Default"; }
        }

        [Header("Debug")]
        public bool IsShowDebugMessages = true;
    }
}
