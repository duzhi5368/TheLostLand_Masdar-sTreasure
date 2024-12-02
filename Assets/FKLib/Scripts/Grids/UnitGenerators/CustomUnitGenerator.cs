using System.Collections.Generic;
using UnityEngine;
//============================================================
namespace FKLib
{
    public class CustomUnitGenerator : MonoBehaviour, IUnitGenerator
    {
        public Transform UnitsParent;
        public Transform CellsParent;

        public List<IUnit> SpawnUnits(List<ICell> cells)
        {
            List<IUnit> ret = new List<IUnit>();
            for (int i = 0; i < UnitsParent.childCount; i++)
            {
                var unit = UnitsParent.GetChild(i).GetComponent<IUnit>();
                if (unit != null)
                {
                    ret.Add(unit);
                }
                else
                {
                    Debug.LogError("Invalid object in Units Parent game object");
                }
            }
            return ret;
        }
    }
}
