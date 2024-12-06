using System;
using System.Collections.Generic;
using UnityEngine;
//============================================================
namespace FKLib
{
    [Serializable]
    public class StatEffect : ScriptableObject, INameable
    {
        [InspectorLabel("Ãû³Æ")]
        [SerializeField]
        protected string _statEffectName = "New Effect";
        [SerializeField]
        protected int _repeat = -1;
        [SerializeReference]
        protected List<Action> _actions = new List<Action>();

        protected ActionSequence _actionSequence;

        [NonSerialized]
        protected int _currentRepeat = 0;

        protected StatsHandler _statsHandler;

        public string Name
        {
            get { return _statEffectName; }
            set { _statEffectName = value; }
        }

        public void Initialize(StatsHandler handler)
        {
            _statsHandler = handler;
            _actionSequence = new ActionSequence(handler.gameObject, new MainPlayerInfo("Player"),
                handler.GetComponent<Blackboard>(), _actions.ToArray());
            _actionSequence.Start();
        }

        public void Execute()
        {
            if (!_actionSequence.Tick()) 
            {
                _actionSequence.Stop();
                _actionSequence.Start();
                _currentRepeat += 1;
            }
            _actionSequence.Update();
            if (_repeat > 0 && _currentRepeat >= _repeat)
            {
                _statsHandler.RemoveEffect(this);
            }
        }
    }
}
