using System;
using System.Collections.Generic;
using UnityEngine;
//============================================================
namespace FKLib
{
    [Serializable]
    public class StatDatabase : ScriptableObject
    {
        public List<IStat> Stats = new List<IStat> ();
        public List<IStatEffect> Effects = new List<IStatEffect>();
        public List<ISettings> Settings = new List<ISettings> ();
    }
}
