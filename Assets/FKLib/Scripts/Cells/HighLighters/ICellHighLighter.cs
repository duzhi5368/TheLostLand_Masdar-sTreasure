using UnityEngine;
//============================================================
namespace FKLib
{
    public abstract class ICellHighLighter : MonoBehaviour
    {
        public abstract void Apply(ICell cell);
    }
}
