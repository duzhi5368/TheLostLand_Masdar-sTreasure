using UnityEngine;
//============================================================
namespace FKLib
{
    public abstract class ITurnResolver : MonoBehaviour
    {
        public abstract ITransitionResult ResolveStart(ICellGrid cellGrid);
        public abstract ITransitionResult ResolveTurn(ICellGrid cellGrid);
    }
}
