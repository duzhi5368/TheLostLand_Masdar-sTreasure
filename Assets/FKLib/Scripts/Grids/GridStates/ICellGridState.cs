using UnityEngine;
//============================================================
namespace FKLib
{
    public abstract class ICellGridState
    {
        protected ICellGrid _cellGrid;

        protected ICellGridState(ICellGrid cellGrid)
        {
            _cellGrid = cellGrid;
        }

        // Initiates a transition to a specified next state of the CellGrid.
        // Params: the next state to transition to
        // Return: the next state after the transition
        public virtual ICellGridState MakeTransition(ICellGridState nextState)
        {
            return nextState;
        }

        // Method will be called when a unit is clicked on.
        public virtual void OnUnitClicked(IUnit unit)
        {
        }

        // Method will be called when a unit is highlighted.
        public virtual void OnUnitHighLighted(IUnit unit)
        {
        }

        // Method will be called when a unit is no longer highlighted.
        public virtual void OnUnitUnHighLighted(IUnit unit)
        {
        }

        // Method will be called when mouse enters cell's collider.
        public virtual void OnCellSelected(ICell cell) 
        {
            cell.MarkAsHighlighted();
        }

        // Method will be called when mouse exits cell's collider.
        public virtual void OnCellUnSelected(ICell cell)
        {
            cell.UnMark();
        }

        // Method will be called when a cell is clicked.
        public virtual void OnCellClicked(ICell cell)
        {
        }

        // Triggers ending the turn.
        public virtual void EndTurn(bool isNetworkInvoked)
        {
            _cellGrid.EndTurnExecute(isNetworkInvoked);
        }

        // Method will be called on transitioning into of a state.
        public virtual void OnStateEnter()
        {
            foreach(var cell in _cellGrid.Cells)
            {
                cell.UnMark();
            }
        }

        // Method will be called on transitioning out of a state.
        public virtual void OnStateExit()
        {
        }
    }
}
