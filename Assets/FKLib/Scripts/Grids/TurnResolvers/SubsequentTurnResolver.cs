using System.Linq;
//============================================================
namespace FKLib
{
    public class SubsequentTurnResolver : ITurnResolver
    {
        public override ITransitionResult ResolveStart(ICellGrid cellGrid)
        {
            var nextPlayerNumber = cellGrid.Players.Min(p => p.PlayerID);
            var nextPlayer = cellGrid.Players.Find(p => p.PlayerID == nextPlayerNumber);
            var allowedUnits = cellGrid.Units.FindAll(u => u.OwnerPlayerID == nextPlayerNumber);

            return new ITransitionResult(nextPlayer, allowedUnits);
        }
        public override ITransitionResult ResolveTurn(ICellGrid cellGrid)
        {
            var nextPlayerNumber = (cellGrid.NumberOfPlayers + 1) % cellGrid.NumberOfPlayers;
            while (cellGrid.Units.FindAll(u => u.OwnerPlayerID.Equals(nextPlayerNumber)).Count == 0)
            {
                nextPlayerNumber = (nextPlayerNumber + 1) % cellGrid.NumberOfPlayers;
            }

            var nextPlayer = cellGrid.Players.Find(p => p.PlayerID == nextPlayerNumber);
            var allowedUnits = cellGrid.Units.FindAll(u => u.OwnerPlayerID == nextPlayerNumber);

            return new ITransitionResult(nextPlayer, allowedUnits);
        }
    }
}
