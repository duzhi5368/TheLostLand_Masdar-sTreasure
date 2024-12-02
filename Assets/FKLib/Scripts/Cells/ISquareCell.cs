using System.Collections.Generic;
using UnityEngine;
//============================================================
namespace FKLib
{
    public abstract class ISquareCell : ICell
    {
        private List<ICell> _neighbors = null;

        protected static readonly Vector2[] _directions =
        {
            new Vector2(1, 0), new Vector2(-1, 0), new Vector2(0, 1), new Vector2(0, -1)
        };

        // return 2 cells distance
        public override int GetDistance(ICell other)
        {
            return (int)(Mathf.Abs(OffsetCoord.x - other.OffsetCoord.x) + Mathf.Abs(OffsetCoord.y - other.OffsetCoord.y));
        }

        public override List<ICell> GetNeighbors(List<ICell> cells)
        {
            if (_neighbors == null)
            {
                _neighbors = new List<ICell>(4);
                foreach (var direction in _directions)
                {
                    var neighbour = cells.Find(c => c.OffsetCoord == OffsetCoord + direction);
                    if (neighbour == null) continue;

                    _neighbors.Add(neighbour);
                }
            }
            return _neighbors;
        }

        public override void CopyFields(ICell newCell)
        {
            newCell.OffsetCoord = OffsetCoord;
        }
    }
}
