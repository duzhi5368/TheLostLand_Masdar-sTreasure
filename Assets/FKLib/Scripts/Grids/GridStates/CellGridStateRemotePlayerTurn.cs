//============================================================
namespace FKLib
{
    public class CellGridStateRemotePlayerTurn : ICellGridState
    {
        public CellGridStateRemotePlayerTurn(ICellGrid cellGrid) : base(cellGrid) { }
        public override void EndTurn(bool isNetworkInvoked)
        {
            base.EndTurn(isNetworkInvoked);
        }
    }
}
