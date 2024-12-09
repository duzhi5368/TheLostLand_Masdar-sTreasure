using System.Collections;
using UnityEngine;
//============================================================
namespace FKLib
{
    internal class TTweenRunner<T> where T : struct, ITweenValue
    {
        protected MonoBehaviour _coroutineContainer;
        protected IEnumerator _tween;

        private static IEnumerator Start(T tweenInfo)
        {
            if (!tweenInfo.ValidTarget())
                yield break;

            var elapsedTime = 0.0f;
            while (elapsedTime < tweenInfo.Duration)
            {
                elapsedTime += tweenInfo.IsIgnoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;
                var percentage = Mathf.Clamp01(elapsedTime / tweenInfo.Duration);
                tweenInfo.TweenValue(percentage);
                yield return null;
            }
            tweenInfo.TweenValue(1.0f);
            tweenInfo.OnFinish();
        }

        public void Init(MonoBehaviour coroutineContainer)
        {
            _coroutineContainer = coroutineContainer;
        }

        public void StartTween(T info)
        {
            if (_coroutineContainer == null)
            {
                Debug.LogWarning("Coroutine container not configured... did you forget to call Init?");
                return;
            }

            StopTween();
            if (!_coroutineContainer.gameObject.activeInHierarchy)
            {
                info.TweenValue(1.0f);
                return;
            }

            _tween = Start(info);
            _coroutineContainer.StartCoroutine(_tween);
        }

        public void StopTween()
        {
            if (_tween != null)
            {
                _coroutineContainer.StopCoroutine(_tween);
                _tween = null;
            }
        }
    }
}
