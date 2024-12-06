using System.Linq;
using UnityEngine;
//============================================================
namespace FKLib
{
    public class ActionSequence
    {
        private ENUM_ActionStatus _status;
        public ENUM_ActionStatus Status { get { return _status; } }

        private int _actionIndex;
        private ENUM_ActionStatus _actionStatus;
        private readonly IAction[] _allActions;
        private IAction[] _actions;

        public ActionSequence(GameObject gameObject, MainPlayerInfo playerInfo, Blackboard blackboard, IAction[] actions)
        {
            _allActions = actions;
            for(int i = 0; i < _allActions.Length; i++)
            {
                _allActions[i].Initialize(gameObject, playerInfo, blackboard);
            }
            _status = ENUM_ActionStatus.eAS_Inactive;
            _actionStatus = ENUM_ActionStatus.eAS_Inactive;
        }

        public void Start()
        {
            _actions = this._allActions.Where(x => x.IsActiveAndEnabled).ToArray();
            for (int i = 0; i < _actions.Length; i++) {
                _actions[i].OnSequenceStart();
            }
            _actionIndex = 0;
            _status = ENUM_ActionStatus.eAS_Running;
        }

        public void Stop()
        {
            if (_actions == null)
                return;
            for (int i = 0;i < _allActions.Length;i++)
            {
                _actions[i].OnSequenceEnd();
            }
            _status = ENUM_ActionStatus.eAS_Inactive;
        }

        public void Update()
        {
            for (int i = 0; i < _actions.Length; i++)
                _actions[i].Update();
        }

        public void Interrupt()
        {
            if (_actions == null)
                return;

            for (int i = 0; i <= _actionIndex; i++)
            {
                if (i < _actions.Length)
                    _actions[i].OnInterrupt();
            }
        }

        public bool Tick()
        {
            if( _status == ENUM_ActionStatus.eAS_Running)
            {
                if (_actionIndex >= _actions.Length)
                    _actionIndex = 0;

                while(_actionIndex < _allActions.Length)
                {
                    if(_actionStatus != ENUM_ActionStatus.eAS_Running)
                        _actions[_actionIndex].OnStart();

                    _actionStatus = _actions[_actionIndex].OnUpdate();

                    if(_actionStatus != ENUM_ActionStatus.eAS_Running)
                        _actions[_actionIndex].OnEnd();

                    if (_actionStatus == ENUM_ActionStatus.eAS_Success)
                        ++_actionIndex;
                    else
                        break;
                }

                _status = _actionStatus;

                if(_status != ENUM_ActionStatus.eAS_Running)
                {
                    for (int i = 0; i < _allActions.Length; i++)
                    {
                        _actions[i].OnSequenceEnd();
                    }
                }
            }
            return _status == ENUM_ActionStatus.eAS_Running;
        }
    }
}
