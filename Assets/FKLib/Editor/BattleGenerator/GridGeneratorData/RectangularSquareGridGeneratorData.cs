using System;
using System.Collections.Generic;
//============================================================
namespace FKLib
{
    [Serializable]
    public class RectangularSquareGridGeneratorData
    {
        // this data should be as same as class RectangularSquareGridGenerator's params.
        public string BaseSquarePrefabPath;
        public int Width;
        public int Height;
        public int ReplaceRate;
        public List<string> RandomSquarePrefabPaths;
    }
}
