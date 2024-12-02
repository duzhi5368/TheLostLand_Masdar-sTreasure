using System.Collections.Generic;
using System.Linq;
//============================================================
namespace FKLib
{
    public class MoveAbility : IAbility
    {
        public ICell Destination { get; set; }
        public HashSet<ICell> AvailableDestinations { get; set; }
        private IList<ICell> _currentPath;

        public override System.Collections.IEnumerator Act(ICellGrid cellGrid)
        {
            if (UnitReference.ActionPoints > 0 && AvailableDestinations.Contains(Destination))
            {
                var path = UnitReference.FindPath(cellGrid.Cells, Destination);
                yield return UnitReference.Move(Destination, path);
            }
            yield return base.Act(cellGrid);
        }

        public override void Display(ICellGrid cellGrid)
        {
            if (UnitReference.ActionPoints <= 0)
                return;

            foreach(var cell in AvailableDestinations)
                cell.MarkAsReachable();
        }

        public override void OnUnitClicked(IUnit unit, ICellGrid cellGrid)
        {
            if (cellGrid.GetCurrentPlayerUnits().Contains(unit))
                cellGrid.cellGridState = new CellGridStateAbilitySelected(cellGrid, unit, unit.GetComponents<IAbility>().ToList());
        }

        public override void OnCellClicked(ICell cell, ICellGrid cellGrid)
        {
            if (AvailableDestinations.Contains(cell))
            {
                Destination = cell;
                _currentPath = null;
                StartCoroutine(HumanExecute(cellGrid));
            }
            else
            {
                cellGrid.cellGridState = new CellGridStateWaitingForInput(cellGrid);
            }
        }

        public override void OnCellSelected(ICell cell, ICellGrid cellGrid)
        {
            if(UnitReference.ActionPoints > 0 && AvailableDestinations.Contains(cell))
            {
                _currentPath = UnitReference.FindPath(cellGrid.Cells, cell);
                foreach (var c in _currentPath)
                    c.MarkAsPath();
            }
        }

        public override void OnCellUnSelected(ICell cell, ICellGrid cellGrid)
        {
            if(UnitReference.ActionPoints > 0 && AvailableDestinations.Contains(cell))
            {
                if (_currentPath == null)
                    return;
                foreach (var c in _currentPath)
                    c.MarkAsReachable();
            }
        }

        public override void OnAbilitySelected(ICellGrid cellGrid)
        {
            UnitReference.CachePaths(cellGrid.Cells);
            AvailableDestinations = UnitReference.GetAvailableDestinations(cellGrid.Cells);
        }

        public override void CleanUp(ICellGrid cellGrid)
        {
            foreach (var cell in AvailableDestinations)
                cell.UnMark();
        }

        public override bool CanPerform(ICellGrid cellGrid)
        {
            return UnitReference.ActionPoints > 0 && UnitReference.GetAvailableDestinations(cellGrid.Cells).Count > 0;
        }

        public override IDictionary<string, string> Encapsulate()
        {
            var actionParams = new Dictionary<string, string>();
            actionParams.Add("destination_x", Destination.OffsetCoord.x.ToString());
            actionParams.Add("destination_y", Destination.OffsetCoord.y.ToString());
            return actionParams;
        }

        public override System.Collections.IEnumerator Apply(ICellGrid cellGrid, IDictionary<string, string> actionParams)
        {
            var actionDestination = cellGrid.Cells.Find(c => c.OffsetCoord.Equals(new UnityEngine.Vector2(float.Parse(actionParams["destination_x"]), float.Parse(actionParams["destination_y"]))));
            Destination = actionDestination;
            yield return StartCoroutine(HumanExecute(cellGrid));
        }
    }
}
