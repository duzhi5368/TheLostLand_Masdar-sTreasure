using System;
using System.Collections.Generic;
using UnityEngine;
//============================================================
namespace FKLib
{
    [Serializable]
    public abstract class ICell : MonoBehaviour, IGraphPathFindingNode, IEquatable<ICell>
    {
        #region params
        private int _hash = -1;
        [SerializeField]
        private Vector2 _offsetCoord;

        public List<ICellHighLighter> MarkAsReachableFn;
        public List<ICellHighLighter> MarkAsPathFn;
        public List<ICellHighLighter> MarkAsHighlightedFn;
        public List<ICellHighLighter> UnMarkFn;

        public bool IsTaken;                                        // Is something taken the cell.
        public Dictionary<ENUM_MoveType, int> MovementCost = new Dictionary<ENUM_MoveType, int>();  // Cost of moving though the cell.
        public List<IUnit> CurrentUnits { get; private set; } = new List<IUnit>();

        public event EventHandler CellClicked;      // Event will be called when a cell is clicked.
        public event EventHandler CellHighLighted;  // Event will be called when a cursor enter the cell.
        public event EventHandler CellUnHighLighted;// Event will be called when a cursor exit the cell.

        public Vector2 OffsetCoord { get { return _offsetCoord; } set { _offsetCoord = value; } }
        #endregion

        #region functions that force to be inherit
        // Return the distance to a cell that is given as parameter
        public abstract int GetDistance(ICell other);
        // Return neighbor cells to current cell. (Only select from the list of cells given)
        public abstract List<ICell> GetNeighbors(List<ICell> cells);
        // Return current cell's physical dimensions, this will be used in grid generators.
        // - If the cell's size if 16pixel by 16pixels, and if we set the [Pixels per unit] is 10,
        // - we need to return Vector3(1.6f, 1.6f, 0.0f).
        public abstract Vector3 GetCellDimensions();
        // Clone this cell data to the new cell which is given 
        public abstract void CopyFields(ICell other);
        #endregion

        public virtual void OnMouseEnter()
        {
            if (CellHighLighted != null)
                CellHighLighted.Invoke(this, EventArgs.Empty);
        }
        public virtual void OnMouseExit()
        {
            if (CellUnHighLighted != null)
                CellUnHighLighted.Invoke(this, EventArgs.Empty);
        }
        public virtual void OnMouseDown()
        {
            if (CellClicked != null)
                CellClicked.Invoke(this, EventArgs.Empty);
        }

        // Mark the cells which the selected unit can reach. 
        public virtual void MarkAsReachable()
        {
            MarkAsReachableFn?.ForEach(o => o.Apply(this));
        }
        // Mark the cells which is a part of a path.
        public virtual void MarkAsPath()
        {
            MarkAsPathFn?.ForEach(o => o.Apply(this));
        }
        // Mark the cells which the mouse is on the cell.
        public virtual void MarkAsHighlighted()
        {
            MarkAsHighlightedFn?.ForEach(o => o.Apply(this));
        }
        // Recover the cells
        public virtual void UnMark()
        {
            UnMarkFn?.ForEach(o => o.Apply(this));
        }

        public virtual void SetColor(Color color) { }
        public virtual void Initialize(ICellGrid cellGrid) { }

        public int GetDistance(IGraphPathFindingNode other)
        {
            return GetDistance(other as ICell);
        }

        public virtual bool Equals(ICell other)
        {
            return OffsetCoord == other.OffsetCoord;
        }

        public override bool Equals(object other)
        {
            ICell otherCell = other as ICell;
            if (otherCell == null) 
                return false;
            return OffsetCoord == otherCell.OffsetCoord;
        }

        public override int GetHashCode()
        {
            if(_hash == -1)
                _hash = OffsetCoord.GetHashCode();
            return _hash;
        }

        public override string ToString()
        {
            return OffsetCoord.ToString();
        }
    }
}
