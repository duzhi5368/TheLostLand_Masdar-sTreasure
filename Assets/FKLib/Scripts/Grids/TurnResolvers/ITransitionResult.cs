using System;
using System.Collections.Generic;
//============================================================
namespace FKLib
{
    public class ITransitionResult
    {
        public IPlayer NextPlayer { get; set; }
        public Func<List<IUnit>> PlayableUnits { get; set; }

        public ITransitionResult(IPlayer nextPlayer, Func<List<IUnit>> allowedUnits) {
            NextPlayer = nextPlayer;
            PlayableUnits = allowedUnits;
        }

        public ITransitionResult(IPlayer nextPlayer, List<IUnit> allowedUnits)
            : this(nextPlayer, () => allowedUnits)
        { }
    }
}
