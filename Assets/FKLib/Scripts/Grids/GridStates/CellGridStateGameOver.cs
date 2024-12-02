//============================================================
namespace FKLib
{
    public class CellGridStateGameOver : ICellGridState
    {
        public CellGridStateGameOver(ICellGrid cellGrid)
            : base(cellGrid) { }

        public override ICellGridState MakeTransition(ICellGridState nextState)
        {
            return this;
        }
    }
}
