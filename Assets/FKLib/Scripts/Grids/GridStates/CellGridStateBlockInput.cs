//============================================================
namespace FKLib
{
    public class CellGridStateBlockInput : ICellGridState
    {
        public CellGridStateBlockInput(ICellGrid cellGrid) : base(cellGrid){}

        public override void EndTurn(bool isNetworkInvoked) {}
    }
}
