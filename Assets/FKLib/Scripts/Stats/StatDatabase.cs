using System;
using System.Collections.Generic;
using UnityEngine;
//============================================================
namespace FKLib
{
    [Serializable]
    public class StatDatabase : ScriptableObject
    {
        public List<IStat> Items = new List<IStat> ();
        public List<StatEffect> Effects = new List<StatEffect>();
        public List<ISettings> Settings = new List<ISettings> ();
    }
}
