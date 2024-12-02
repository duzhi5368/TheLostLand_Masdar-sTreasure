using System.Collections.Generic;
using UnityEngine;
//============================================================
namespace FKLib
{
    public class IGridInfo
    {
        public Vector3 Dimensions { get; set; }
        public Vector3 Center { get; set; }
        public List<ICell> Cells { get; set; }
    }
}
