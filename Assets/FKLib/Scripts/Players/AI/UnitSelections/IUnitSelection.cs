using System;
using System.Collections.Generic;
using UnityEngine;
//============================================================
namespace FKLib
{
    public abstract class IUnitSelection : MonoBehaviour
    {
        public abstract IEnumerable<IUnit> SelectNext(Func<List<IUnit>> getUnits, ICellGrid cellGrid);
    }
}
