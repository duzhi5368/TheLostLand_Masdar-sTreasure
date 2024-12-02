using UnityEngine;
//============================================================
namespace FKLib
{
    public abstract class ICellEvaluator : MonoBehaviour
    {
        public float Weight = 1;

        public virtual void Precalculate(IUnit evaluatingUnit, IPlayer currentPlayer, ICellGrid cellGrid) { }
        public abstract float Evaluate(ICell cellToEvaluate, IUnit evaluatingUnit, IPlayer currentPlayer, ICellGrid cellGrid);
    }
}
