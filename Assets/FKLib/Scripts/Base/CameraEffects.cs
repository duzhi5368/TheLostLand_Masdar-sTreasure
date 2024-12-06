using System.Security.Cryptography;
using UnityEngine;
//============================================================
namespace FKLib
{
    public class CameraEffects : MonoBehaviour
    {
        public Vector3          Amount = new Vector3(1f, 1f, 0);
        public float            Duration = 1;
        public float            Speed = 10;
        public AnimationCurve   Curve = AnimationCurve.EaseInOut(0, 1, 1, 0);
        public bool             IsDeltaMovement = true;

        protected Camera        _camera;
        protected float         _time = 0;
        protected Vector3       _lastPosition;
        protected Vector3       _nextPosition;
        protected float         _lastFieldOfView;
        protected float         _nextFieldOfView;

        private void Awake()
        {
            _camera = GetComponent<Camera>();
        }

        public static void Shake(float duration = 1f, float speed = 10f, Vector3? amount = null, 
            Camera camera = null, bool deltaMovement = true, AnimationCurve curve = null)
        {
            var instance = ((camera != null) ? camera : Camera.main).gameObject.AddComponent<CameraEffects>();
            instance.Duration = duration;
            instance.Speed = speed;
            if (amount != null)
                instance.Amount = (Vector3)amount;
            if (curve != null)
                instance.Curve = curve;
            instance.IsDeltaMovement = deltaMovement;
            instance.ResetCamera();
            instance._time = duration;
        }

        private void LateUpdate()
        {
            if (_time > 0)
            {
                _time -= Time.deltaTime;
                if (_time > 0)
                {
                    _nextPosition = (Mathf.PerlinNoise(_time * Speed, _time * Speed * 2) - 0.5f) * Amount.x * transform.right * Curve.Evaluate(1f - _time / Duration) +
                              (Mathf.PerlinNoise(_time * Speed * 2, _time * Speed) - 0.5f) * Amount.y * transform.up * Curve.Evaluate(1f - _time / Duration);
                    _nextFieldOfView = (Mathf.PerlinNoise(_time * Speed * 2, _time * Speed * 2) - 0.5f) * Amount.z * Curve.Evaluate(1f - _time / Duration);

                    _camera.fieldOfView += (_nextFieldOfView - _lastFieldOfView);
                    _camera.transform.Translate(IsDeltaMovement ? (_nextPosition - _lastPosition) : _nextPosition);

                    _lastPosition = _nextPosition;
                    _lastFieldOfView = _nextFieldOfView;
                }
            }
        }

        private void ResetCamera()
        {
            _camera.transform.Translate(IsDeltaMovement ? -_lastPosition : Vector3.zero);
            _camera.fieldOfView -= _lastFieldOfView;
            _lastPosition = _nextPosition = Vector3.zero;
            _lastFieldOfView = _nextFieldOfView = 0f;
        }
    }
}
