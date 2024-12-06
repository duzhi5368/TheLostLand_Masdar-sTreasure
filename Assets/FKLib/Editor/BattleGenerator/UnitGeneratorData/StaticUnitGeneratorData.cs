using System;
using System.Collections.Generic;
//============================================================
namespace FKLib
{
    [Serializable]
    public class StaticUnitGeneratorData
    {
        public List<string> RandomNeutrallyUnitPrefabPaths = new List<string>();
        public int NeutrallyUnitNumber; // 战场上随机生成的中立Unit数量
        public int ProtectedColumns;    // 被保护的前后列数（在前后几列不会刷新出中立Unit，目的是使中立战斗对象尽量居中）
    }
}
