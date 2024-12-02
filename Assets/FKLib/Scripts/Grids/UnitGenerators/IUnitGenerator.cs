using System.Collections.Generic;
//============================================================
namespace FKLib
{
    public interface IUnitGenerator
    {
        List<IUnit> SpawnUnits(List<ICell> cells);
    }
}
