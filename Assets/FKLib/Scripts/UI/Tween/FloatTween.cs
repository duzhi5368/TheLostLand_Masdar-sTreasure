using UnityEngine.Events;
//============================================================
namespace FKLib
{
    public class FloatTweenCallback : UnityEvent<float>
    {
        public FloatTweenCallback() { }
    }
    public class FloatTweenFinishCallback : UnityEvent
    {
        public FloatTweenFinishCallback() { }
    }
    internal struct FloatTween : ITweenValue
    {
        private FloatTweenCallback _target;
        private FloatTweenFinishCallback _onFinish;

        private EasingEquations.ENUM_EaseType _easeType;
        public EasingEquations.ENUM_EaseType EaseType
        {
            get { return _easeType; }
            set { _easeType = value; }
        }
        private float _startValue;
        public float StartValue
        {
            get { return _startValue; }
            set { _startValue = value; }
        }
        private float _targetValue;
        public float TargetValue
        {
            get { return _targetValue; }
            set { _targetValue = value; }
        }
        private float _duration;
        public float Duration
        {
            get { return _duration; }
            set { _duration = value; }
        }
        private bool _isIgnoreTimeScale;
        public bool IsIgnoreTimeScale
        {
            get { return _isIgnoreTimeScale; }
            set { _isIgnoreTimeScale = value; }
        }

        public bool ValidTarget()
        {
            return _target != null;
        }
        public void TweenValue(float floatPercentage)
        {
            if (!this.ValidTarget())
            {
                return;
            }
            float value = EasingEquations.GetValue(EaseType, StartValue, TargetValue, floatPercentage);
            _target.Invoke(value);
        }
        public void AddOnChangedCallback(UnityAction<float> callback)
        {
            if (_target == null)
                _target = new FloatTweenCallback();
            _target.AddListener(callback);
        }
        public void AddOnFinishCallback(UnityAction callback)
        {
            if (_onFinish == null)
                _onFinish = new FloatTweenFinishCallback();
            _onFinish.AddListener(callback);
        }
        public void OnFinish()
        {
            if (_onFinish != null)
                _onFinish.Invoke();
        }
    }
}
