using System.Collections.Generic;
using System.Linq;
using UnityEngine;
//============================================================
namespace FKLib
{
    public class CellGridStateAbilitySelected : ICellGridState
    {
        private List<IAbility> _abilities;
        private IUnit _unit;

        public CellGridStateAbilitySelected(ICellGrid cellGrid, IUnit unit, List<IAbility> abilities)
            : base(cellGrid)
        {
            if (abilities.Count == 0)
            {
                Debug.LogError("No abilities were selected, check if your unit has any abilities attached to it");
            }
            _abilities = abilities;
            _unit = unit;
        }

        public CellGridStateAbilitySelected(ICellGrid cellGrid, IUnit unit, IAbility ability)
            : this(cellGrid, unit, new List<IAbility>() { ability }) { }



        public override void OnUnitClicked(IUnit unit)
        {
            _abilities.ForEach(a => a.OnUnitClicked(unit, _cellGrid));
        }
        public override void OnUnitHighLighted(IUnit unit)
        {
            _abilities.ForEach(a => a.OnUnitHighLighted(unit, _cellGrid));
        }
        public override void OnUnitUnHighLighted(IUnit unit)
        {
            _abilities.ForEach(a => a.OnUnitUnHighLighted(unit, _cellGrid));
        }
        public override void OnCellClicked(ICell cell)
        {
            _abilities.ForEach(a => a.OnCellClicked(cell, _cellGrid));
        }
        public override void OnCellSelected(ICell cell)
        {
            base.OnCellSelected(cell);
            _abilities.ForEach(a => a.OnCellSelected(cell, _cellGrid));
        }
        public override void OnCellUnSelected(ICell cell)
        {
            base.OnCellUnSelected(cell);
            _abilities.ForEach(a => a.OnCellUnSelected(cell, _cellGrid));
        }

        public override void OnStateEnter()
        {
            _unit?.OnUnitSelected();
            _abilities.ForEach(a => a.OnAbilitySelected(_cellGrid));
            _abilities.ForEach(a => a.Display(_cellGrid));

            var canPerformAction = _abilities.Select(a => a.CanPerform(_cellGrid))
                                             .DefaultIfEmpty()
                                             .Aggregate((result, next) => result || next);
            if (!canPerformAction)
            {
                _unit?.SetState(new UnitStateMarkedAsFinished(_unit));
            }
        }
        public override void OnStateExit()
        {
            _unit?.OnUnitUnSelected();
            _abilities.ForEach(a => a.OnAbilityUnSelected(_cellGrid));
            _abilities.ForEach(a => a.CleanUp(_cellGrid));
        }
    }
}
