using System.Collections.Generic;
using UnityEngine;
//============================================================
namespace FKLib
{
    public class UnitHighLighterAggregator : MonoBehaviour
    {
        public List<IUnitHighLighter> MarkAsAttackingFn;
        public List<IUnitHighLighter> MarkAsDefendingFn;
        public List<IUnitHighLighter> MarkAsSelectedFn;
        public List<IUnitHighLighter> MarkAsFriendlyFn;
        public List<IUnitHighLighter> MarkAsFinishedFn;
        public List<IUnitHighLighter> MarkAsDestroyedFn;
        public List<IUnitHighLighter> MarkAsReachableEnemyFn;
        public List<IUnitHighLighter> UnMarkFn;
    }
}
