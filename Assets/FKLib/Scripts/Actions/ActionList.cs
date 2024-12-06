using System;
using System.Collections.Generic;
using UnityEngine;
//============================================================
namespace FKLib
{
    [Serializable]
    public class ActionList
    {
        [SerializeReference]
        public List<Action> actions = new List<Action>();
    }
}
