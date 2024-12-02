using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
//============================================================
namespace FKLib
{
    public class AttackAbility : IAbility
    {
        public IUnit UnitToAttack { get; set; }
        public int UnitToAttackID { get; set; }
        private List<IUnit> _unitsInAttackRange;

        public override System.Collections.IEnumerator Act(ICellGrid cellGrid)
        {
            if(CanPerform(cellGrid) && UnitReference.IsUnitAttackable(UnitToAttack, UnitReference.Cell))
            {
                UnitReference.AttackHandler(UnitToAttack);
                yield return new WaitForSeconds(0.5f);
            }
            yield return null;
        }

        public override void Display(ICellGrid cellGrid)
        {
            var enemyUnits = cellGrid.GetEnemyUnits(cellGrid.CurrentPlayer);
            _unitsInAttackRange = enemyUnits.Where(
                u => UnitReference.IsUnitAttackable(u, UnitReference.Cell)).ToList();
            _unitsInAttackRange.ForEach(u => u.MarkAsReachableEnemy());
        }

        public override void OnUnitClicked(IUnit unit, ICellGrid cellGrid)
        {
            if (UnitReference.IsUnitAttackable(unit, UnitReference.Cell))
            {
                UnitToAttack = unit;
                UnitToAttackID = UnitToAttack.UnitID;
                StartCoroutine(HumanExecute(cellGrid));
            }
            else if (cellGrid.GetCurrentPlayerUnits().Contains(unit))
            {
                cellGrid.cellGridState = new CellGridStateAbilitySelected(cellGrid, unit, unit.GetComponents<IAbility>().ToList());
            }
        }

        public override void OnCellClicked(ICell cell, ICellGrid cellGrid)
        {
            cellGrid.cellGridState = new CellGridStateWaitingForInput(cellGrid);
        }

        public override void CleanUp(ICellGrid cellGrid)
        {
            _unitsInAttackRange.ForEach(u =>
            {
                if (u != null) { u.UnMark(); }
            });
        }

        public override bool CanPerform(ICellGrid cellGrid)
        {
            if(UnitReference.ActionPoints <= 0) 
                return false;

            var enemyUnits = cellGrid.GetEnemyUnits(cellGrid.CurrentPlayer);
            _unitsInAttackRange = enemyUnits.Where(u => UnitReference.IsUnitAttackable(u, UnitReference.Cell)).ToList();
            return _unitsInAttackRange.Count > 0;
        }

        public override IDictionary<string, string> Encapsulate()
        {
            Dictionary<string, string> actionParameters = new Dictionary<string, string>();
            actionParameters.Add("target_id", UnitToAttackID.ToString());

            return actionParameters;
        }

        public override IEnumerator Apply(ICellGrid cellGrid, IDictionary<string, string> actionParams)
        {
            var targetID = int.Parse(actionParams["target_id"]);
            var target = cellGrid.Units.Find(u => u.UnitID == targetID);

            UnitToAttack = target;
            UnitToAttackID = targetID;
            yield return StartCoroutine(HumanExecute(cellGrid));
        }
    }
}
