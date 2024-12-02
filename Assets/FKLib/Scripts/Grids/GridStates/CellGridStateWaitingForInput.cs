using System.Linq;
//============================================================
namespace FKLib
{
    public class CellGridStateWaitingForInput : ICellGridState
    {
        public CellGridStateWaitingForInput(ICellGrid cellGrid)
            : base(cellGrid) { }

        public override void OnUnitClicked(IUnit unit)
        {
            if (_cellGrid.GetCurrentPlayerUnits().Contains(unit))
            {
                _cellGrid.cellGridState = new CellGridStateAbilitySelected(_cellGrid, unit, unit.GetComponents<IAbility>().ToList());
            }
        }
    }
}
