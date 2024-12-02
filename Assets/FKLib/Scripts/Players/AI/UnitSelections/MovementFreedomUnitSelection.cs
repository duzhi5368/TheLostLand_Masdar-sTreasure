using System;
using System.Collections.Generic;
using System.Linq;
//============================================================
namespace FKLib
{
    public class MovementFreedomUnitSelection : IUnitSelection
    {
        private HashSet<IUnit> alreadySelected = new HashSet<IUnit>();

        public override IEnumerable<IUnit> SelectNext(Func<List<IUnit>> getUnits, ICellGrid cellGrid)
        {
            var units = getUnits();
            while (alreadySelected.Count < units.Count)
            {
                var nextUnit = units.Where(u => !alreadySelected.Contains(u))
                    .OrderByDescending(u => u.Cell.GetNeighbors(cellGrid.Cells)
                    .Where(u.IsCellTraversable).Count()).First();
                alreadySelected.Add(nextUnit);
                yield return nextUnit;
            }
            alreadySelected = new HashSet<IUnit>();
        }
    }
}
