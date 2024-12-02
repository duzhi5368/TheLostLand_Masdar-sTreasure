//============================================================
namespace FKLib
{
    public class HumanPlayer : IPlayer
    {
        public override void Play(ICellGrid cellGrid)
        {
            cellGrid.cellGridState = new CellGridStateWaitingForInput(cellGrid);
        }
    }
}
