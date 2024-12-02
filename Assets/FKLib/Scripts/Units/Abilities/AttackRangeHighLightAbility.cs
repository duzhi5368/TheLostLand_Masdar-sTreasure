using System.Collections.Generic;
//============================================================
namespace FKLib
{
    public class AttackRangeHighLightAbility : IAbility
    {
        private List<IUnit> _unitsInRange;

        public override void OnCellSelected(ICell cell, ICellGrid cellGrid)
        {
            var availableDestinations = UnitReference.GetComponent<MoveAbility>().AvailableDestinations;
            if (!availableDestinations.Contains(cell))
                return;
            var enemyUnits = cellGrid.GetEnemyUnits(cellGrid.CurrentPlayer);
            _unitsInRange = enemyUnits.FindAll(u => UnitReference.IsUnitAttackable(u, cell));
            _unitsInRange.ForEach(u => u.MarkAsReachableEnemy());
        }

        public override void OnCellUnSelected(ICell cell, ICellGrid cellGrid)
        {
            _unitsInRange?.ForEach(u => u.UnMark());
            var enemyUnits = cellGrid.GetEnemyUnits(cellGrid.CurrentPlayer);
            var inRangeLocal = enemyUnits.FindAll(u => UnitReference.IsUnitAttackable(u, UnitReference.Cell));
            inRangeLocal.ForEach(u => u.MarkAsReachableEnemy());
        }

        public override void OnAbilitySelected(ICellGrid cellGrid)
        {
            _unitsInRange?.ForEach(u => u.UnMark());
        }

        public override void OnTurnEnd(ICellGrid cellGrid)
        {
            _unitsInRange = null;
        }
    }
}
