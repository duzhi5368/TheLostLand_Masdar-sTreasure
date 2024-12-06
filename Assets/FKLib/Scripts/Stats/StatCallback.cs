using UnityEngine;
//============================================================
namespace FKLib
{
    public enum ENUM_ValueType
    {
        [Header("值")]
        eVT_Value,
        [Header("当前值")]
        eVT_CurrentValue
    }

    public enum ENUM_ConditionType
    {
        [Header("大于")]
        eCT_Greater,
        [Header("大于等于")]
        eCT_GreaterOrEqual,
        [Header("小于")]
        eCT_Less,
        [Header("大于等于")]
        eCT_LessOrEqual,
    }

    [UnityEngine.Scripting.APIUpdating.MovedFromAttribute(true, null, "Assembly-CSharp")]
    [System.Serializable]
    public class StatCallback
    {
        [InspectorLabel("值类型")]
        [SerializeField]
        protected ENUM_ValueType _valueType = ENUM_ValueType.eVT_CurrentValue;

        [InspectorLabel("条件")]
        [SerializeField]
        protected ENUM_ConditionType _condition;

        [InspectorLabel("值")]
        [SerializeField]
        protected float _value = 0f;

        [SerializeField]
        protected ActionList _actions;

        protected IStat _stat;
        protected StatsHandler _handler;
        protected ActionSequence _actionSequence;

        public virtual void Initialize(StatsHandler handler, IStat stat)
        {
            _handler = handler;
            _stat = stat;
            switch (_valueType) 
            {
                case ENUM_ValueType.eVT_Value:
                    stat.OnValueChange += OnValueChange;
                    break;
                case ENUM_ValueType.eVT_CurrentValue:
                    if(stat is StatAttribute attribute)
                        attribute.OnCurrentValueChange += OnCurrentValueChange;
                    break;
            }
            _actionSequence = new ActionSequence(handler.gameObject, new MainPlayerInfo("Player"), 
                handler.GetComponent<Blackboard>(), _actions.actions.ToArray());
            _handler.OnUpdate += Update;
        }

        private void Update() {
            if (_actionSequence != null)
                _actionSequence.Tick();
        }

        private void OnValueChange()
        {
            if(TriggerCallback(_stat.Value))
                _actionSequence.Start();
        }

        private void OnCurrentValueChange()
        {
            if(TriggerCallback((_stat as StatAttribute).CurrentValue))
                    _actionSequence.Start();
        }

        private bool TriggerCallback(float value)
        {
            switch (_condition) 
            {
                case ENUM_ConditionType.eCT_Greater:
                    return value > _value;
                case ENUM_ConditionType.eCT_GreaterOrEqual:
                    return value >= _value;
                case ENUM_ConditionType.eCT_Less:
                    return value < _value;
                case ENUM_ConditionType.eCT_LessOrEqual:
                    return value <= _value;
            }
            return false;
        }
    }
}
