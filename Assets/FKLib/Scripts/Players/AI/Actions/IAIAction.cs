using System.Collections;
using UnityEngine;
//============================================================
namespace FKLib
{
    public abstract class IAIAction : MonoBehaviour
    {
        public abstract void InitializeAction(IPlayer player, IUnit unit, ICellGrid cellGrid);
        public abstract bool ShouldExecute(IPlayer player, IUnit unit, ICellGrid cellGrid);
        public abstract void Precalculate(IPlayer player, IUnit unit, ICellGrid cellGrid);
        public abstract IEnumerator Execute(IPlayer player, IUnit unit, ICellGrid cellGrid);
        public abstract void CleanUp(IPlayer player, IUnit unit, ICellGrid cellGrid);
        public abstract void ShowDebugInfo(IPlayer player, IUnit unit, ICellGrid cellGrid);
    }
}
