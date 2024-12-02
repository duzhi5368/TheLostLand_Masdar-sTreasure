using System.Collections.Generic;
//============================================================
namespace FKLib
{
    public class GameResult
    {
        public bool IsFinished { get; private set; }
        public List<int> WinningPlayers { get; private set; }
        public List<int> LoosingPlayers { get; private set; }

        public GameResult(bool isFinished, List<int> winningPlayers, List<int> loosingPlayers)
        {
            IsFinished = isFinished;
            WinningPlayers = winningPlayers;
            LoosingPlayers = loosingPlayers;
        }
    }
}
