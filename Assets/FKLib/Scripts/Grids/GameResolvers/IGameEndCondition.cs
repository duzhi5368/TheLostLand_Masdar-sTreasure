using UnityEngine;
//============================================================
namespace FKLib
{
    public abstract class IGameEndCondition : MonoBehaviour
    {
        public abstract GameResult CheckCondition(ICellGrid cellGrid);
    }
}
