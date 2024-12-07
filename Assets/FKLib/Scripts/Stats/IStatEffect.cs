using System;
using System.Collections.Generic;
using UnityEngine;
//============================================================
namespace FKLib
{
    [Serializable]
    public class IStatEffect : ScriptableObject, INameable
    {
        #region 编辑器中的数据
        [InspectorLabel("名称")]
        [SerializeField]
        protected string _statEffectName = "新效果";
        [InspectorLabel("重复次数")]
        [SerializeField]
        protected int _repeatTimes = -1;
        [SerializeReference]
        protected List<Action> _actions = new List<Action>();
        #endregion

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
            if (_repeatTimes > 0 && _currentRepeat >= _repeatTimes)
            {
                _statsHandler.RemoveEffect(this);
            }
        }
    }
}
