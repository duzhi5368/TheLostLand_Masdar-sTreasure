using UnityEngine;
//============================================================
namespace FKLib
{
    public enum ENUM_ActionTargetType
    {
        eATT_Self,
        eATT_Player,
        eATT_Camera
    }

    [UnityEngine.Scripting.APIUpdating.MovedFromAttribute(true, null, "Assembly-CSharp")]
    [System.Serializable]
    public abstract class Action : IAction
    {
        [HideInInspector]
        [SerializeField]
        private string _type;
        [HideInInspector]
        [SerializeField]
        private bool _isEnabled = true;
        public bool Enabled
        {
            get { return _isEnabled; }
            set { _isEnabled = value; }
        }

        protected MainPlayerInfo _playerInfo;
        protected GameObject _gameObject;
        protected Blackboard _blackboard;

        public bool IsActiveAndEnabled
        {
            get { return _isEnabled && (_gameObject == null || _gameObject.activeSelf); }
        }

        public Action()
        {
            _type = GetType().FullName;
        }

        public void Initialize(GameObject gameObject, MainPlayerInfo mainPlayerInfo, Blackboard blackboard)
        {
            _gameObject = gameObject;
            _playerInfo = mainPlayerInfo;
            _blackboard = blackboard;
        }

        public abstract ENUM_ActionStatus OnUpdate();
        public virtual void Update() { }
        public virtual void OnStart() { }
        public virtual void OnEnd() { }
        public virtual void OnSequenceStart() { }
        public virtual void OnSequenceEnd() { }
        public virtual void OnInterrupt() { }

        protected GameObject GetTarget(ENUM_ActionTargetType type)
        {
            switch (type) 
            {
                case ENUM_ActionTargetType.eATT_Player:
                    return _playerInfo.GameObject;
                case ENUM_ActionTargetType.eATT_Camera:
                    return Camera.main.gameObject;
            }
            return _gameObject;
        }

    }
}
