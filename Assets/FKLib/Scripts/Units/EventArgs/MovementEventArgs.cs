using System;
using System.Collections.Generic;
//============================================================
namespace FKLib
{
    public class MovementEventArgs : EventArgs
    {
        public ICell OriginCell;
        public ICell DestinationCell;
        public IList<ICell> Path;
        public IUnit Unit;

        public MovementEventArgs(ICell sourceCell, ICell destinationCell, IList<ICell> path, IUnit unit)
        {
            OriginCell = sourceCell;
            DestinationCell = destinationCell;
            Path = path;
            Unit = unit;
        }
    }
}
