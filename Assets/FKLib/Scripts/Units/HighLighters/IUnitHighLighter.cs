using UnityEngine;
//============================================================
namespace FKLib
{
    public abstract class IUnitHighLighter : MonoBehaviour
    {
        public abstract void Apply(IUnit unit, IUnit otherUnit);
    }
}
