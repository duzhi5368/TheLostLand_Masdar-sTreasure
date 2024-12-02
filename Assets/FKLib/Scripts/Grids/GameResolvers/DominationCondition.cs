using System.Linq;
//============================================================
namespace FKLib
{
    public class DominationCondition : IGameEndCondition
    {
        public override GameResult CheckCondition(ICellGrid cellGrid)
        {
            var playersAlive = cellGrid.Units.Select(u => u.OwnerPlayerID).Distinct().ToList();
            if (playersAlive.Count == 1)
            {
                var playersDead = cellGrid.Players.Where(p => p.PlayerID != playersAlive[0])
                                                  .Select(p => p.PlayerID).ToList();
                return new GameResult(true, playersAlive, playersDead);
            }
            return new GameResult(false, null, null);
        }
    }
}
