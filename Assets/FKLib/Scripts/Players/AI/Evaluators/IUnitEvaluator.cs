using UnityEngine;
//============================================================
namespace FKLib
{
    public abstract class IUnitEvaluator : MonoBehaviour
    {
        public float Weight = 1;

        public virtual void Precalculate(IUnit evaluatingUnit, IPlayer currentPlayer, ICellGrid cellGrid) { }
        public abstract float Evaluate(IUnit unitToEvaluate, IUnit evaluatingUnit, IPlayer currentPlayer, ICellGrid cellGrid);
    }
}
