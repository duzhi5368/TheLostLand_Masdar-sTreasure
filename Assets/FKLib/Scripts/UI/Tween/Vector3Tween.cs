using UnityEngine;
using UnityEngine.Events;
//============================================================
namespace FKLib
{
    internal struct Vector3Tween : ITweenValue
    {
        public class Vector3TweenCallback : UnityEvent<Vector3>
        {
            public Vector3TweenCallback() { }
        }
        public class Vector3TweenFinishCallback : UnityEvent
        {
            public Vector3TweenFinishCallback() { }
        }

        private Vector3TweenCallback _target;
        private Vector3TweenFinishCallback _onFinish;

        private EasingEquations.ENUM_EaseType _easeType;
        public EasingEquations.ENUM_EaseType EaseType
        {
            get { return this._easeType; }
            set { this._easeType = value; }
        }
        private Vector3 _startValue;
        public Vector3 StartValue
        {
            get { return this._startValue; }
            set { this._startValue = value; }
        }
        private Vector3 _targetValue;
        public Vector3 TargetValue
        {
            get { return this._targetValue; }
            set { this._targetValue = value; }
        }
        private float _duration;
        public float Duration
        {
            get { return this._duration; }
            set { this._duration = value; }
        }
        private bool _isIgnoreTimeScale;
        public bool IsIgnoreTimeScale
        {
            get { return this._isIgnoreTimeScale; }
            set { this._isIgnoreTimeScale = value; }
        }

        public bool ValidTarget()
        {
            return this._target != null;
        }

        public void TweenValue(float floatPercentage)
        {
            if (!this.ValidTarget())
            {
                return;
            }
            float x = EasingEquations.GetValue(EaseType, StartValue.x, TargetValue.x, floatPercentage);
            float y = EasingEquations.GetValue(EaseType, StartValue.y, TargetValue.y, floatPercentage);
            float z = EasingEquations.GetValue(EaseType, StartValue.z, TargetValue.z, floatPercentage);
            this._target.Invoke(new Vector3(x, y, z));
        }

        public void AddOnChangedCallback(UnityAction<Vector3> callback)
        {
            if (_target == null)
                _target = new Vector3TweenCallback();
            _target.AddListener(callback);
        }

        public void AddOnFinishCallback(UnityAction callback)
        {
            if (_onFinish == null)
                _onFinish = new Vector3TweenFinishCallback();
            _onFinish.AddListener(callback);
        }

        public void OnFinish()
        {
            if (_onFinish != null)
                _onFinish.Invoke();
        }
    }
}
