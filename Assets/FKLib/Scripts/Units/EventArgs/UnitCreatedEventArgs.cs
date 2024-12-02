using UnityEngine;
//============================================================
namespace FKLib
{
    public class UnitCreatedEventArgs
    {
        public Transform Unit;

        public UnitCreatedEventArgs(Transform unit)
        {
            this.Unit = unit;
        }
    }
}
