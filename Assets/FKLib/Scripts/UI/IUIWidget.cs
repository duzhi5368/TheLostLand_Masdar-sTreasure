using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//============================================================
namespace FKLib
{
    [RequireComponent(typeof(CanvasGroup))]
    public class IUIWidget : ICallbackHandler
    {
        [Tooltip("Name of the widget. You can find a reference to a widget with WidgetUtility.Find<T>(name).")]
        [SerializeField]
        protected new string name;
        public string Name { get { return name; } set { name = value; } }

        // Callbacks for Inspector.
        public override string[] Callbacks { get { return new string[] { "OnShow", "OnClose", }; } }

        // Widgets with higher priority will be prefered when used with WidgetUtility.Find<T>(name).
        [Tooltip("Widgets with higher priority will be prefered when used with WidgetUtility.Find<T>(name).")]
        [Range(0, 100)]
        public int Priority;

        // Key to toggle show and close.
        [Header("Appearence")]
        [Tooltip("Key to show or close this widget.")]
        [SerializeField]
        protected KeyCode _keyCode = KeyCode.None;

        [Tooltip("Easing equation type used to tween this widget.")]
        [SerializeField]
        private EasingEquations.ENUM_EaseType _easeType = EasingEquations.ENUM_EaseType.eET_EaseInOutBack;

        [Tooltip("The duration to tween this widget.")]
        [SerializeField]
        protected float _duration = 0.7f;

        [SerializeField]
        protected bool _isIgnoreTimeScale = true;
        public bool IsIgnoreTimeScale { get { return _isIgnoreTimeScale; } }

        [Tooltip("The AudioClip that will be played when this widget shows.")]
        [SerializeField]
        protected AudioClip _showSound;

        [Tooltip("The AudioClip that will be played when this widget closes.")]
        [SerializeField]
        protected AudioClip _closeSound;

        [Tooltip("Focus the widget. This will bring the widget to front when it is shown.")]
        [SerializeField]
        protected bool _isFocus = true;

        [Tooltip("If true, deactivates the game object when it gets closed. This prevets Update() to be called every frame.")]
        [SerializeField]
        protected bool _isDeactivateOnClose = true;

        [Tooltip("Enables Cursor when this window is shown. Hides it again when the window is closed or character moves.")]
        [SerializeField]
        protected bool _isShowAndHideCursor = false;

        [SerializeField]
        protected string _cameraPreset = "UI";

        [Tooltip("Close this widget when the player moves.")]
        [SerializeField]
        protected bool _isCloseOnMove = true;

        [Tooltip("This option allows to focus and rotate player. This functionality only works with the included ThirdPersonCamera and FocusTarget component!")]
        [SerializeField]
        protected bool _isFocusPlayer = false;

        [Tooltip("The RectTransform of the widget.")]
        protected RectTransform _rectTransform;

        [Tooltip("The CanvasGroup of the widget.")]
        protected CanvasGroup _canvasGroup;

        [Tooltip("Checks if Show() is already called. This prevents from calling Show() multiple times when the widget is not finished animating. ")]
        protected bool _isShowing;

        protected static CursorLockMode _sPreviousCursorLockMode;
        protected static bool _sPreviousCursorVisibility;
        protected static bool _sIsPreviousCameraControllerEnabled;
        protected static List<IUIWidget> _sCurrentVisibleWidgets = new List<IUIWidget>();
        protected Transform _cameraTransform;
        protected MonoBehaviour _cameraController;
        protected MonoBehaviour _thirdPersonController;
        protected CursorLockMode _previousLockMode;
        protected Scrollbar[] _scrollbars;
        private TTweenRunner<FloatTween> _alphaTweenRunner;
        private TTweenRunner<Vector3Tween> _scaleTweenRunner;
        protected bool _isLocked = false;

        public bool IsLocked { get { return _isLocked; } }

        public bool IsVisible
        {
            get
            {
                if (_canvasGroup == null)
                    _canvasGroup = GetComponent<CanvasGroup>();
                return _canvasGroup.alpha == 1f;
            }
        }

        private void Awake()
        {
            WidgetInputHandler.RegisterInput(_keyCode, this);

            _rectTransform = GetComponent<RectTransform>();
            _canvasGroup = GetComponent<CanvasGroup>();
            _scrollbars = GetComponentsInChildren<Scrollbar>();
            _cameraTransform = Camera.main.transform;
            _cameraController = _cameraTransform.GetComponent("ThirdPersonCamera") as MonoBehaviour;

            MainPlayerInfo playerInfo = new MainPlayerInfo("Player");
            if (playerInfo.GameObject != null)
                _thirdPersonController = playerInfo.GameObject.GetComponent("ThirdPersonCamera") as MonoBehaviour;
            if (!IsVisible)
                _rectTransform.localScale = Vector3.zero;
            if (_alphaTweenRunner == null)
                _alphaTweenRunner = new TTweenRunner<FloatTween>();
            _alphaTweenRunner.Init(this);
            if (_scaleTweenRunner == null)
                _scaleTweenRunner = new TTweenRunner<Vector3Tween>();
            _scaleTweenRunner.Init(this);

            _isShowing = IsVisible;
            OnAwake();
        }

        protected virtual void OnAwake()
        {

        }

        private void Start()
        {
            OnStart();
            StartCoroutine(OnDelayedStart());
        }

        protected virtual void OnStart()
        {

        }

        private IEnumerator OnDelayedStart()
        {
            yield return null;
            if (!IsVisible && _isDeactivateOnClose)
                gameObject.SetActive(false);
        }

        protected virtual void Update()
        {
            if (_isShowAndHideCursor && IsVisible && _isCloseOnMove
                && (_thirdPersonController == null || _thirdPersonController.enabled)
                && (Input.GetAxis("Vertical") != 0f || Input.GetAxis("Horizontal") != 0f))
            { 
                Close(); 
            }
        }

        public virtual void Show()
        {
            if (_isShowing)
                return;

            _isShowing = true;
            gameObject.SetActive(true);
            if (_isFocus)
            {
                Focus();
            }
            TweenCanvasGroupAlpha(_canvasGroup.alpha, 1f);
            TweenTransformScale(Vector3.ClampMagnitude(_rectTransform.localScale, 1.9f), Vector3.one);

            WidgetUtility.PlaySound(_showSound, 1.0f);
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
            Canvas.ForceUpdateCanvases();
            for (int i = 0; i < _scrollbars.Length; i++)
            {
                _scrollbars[i].value = 1f;
            }
            if (_isShowAndHideCursor)
            {
                _sCurrentVisibleWidgets.Add(this);
                if (_isFocusPlayer && !_isLocked)
                    _cameraController.SendMessage("Focus", true, SendMessageOptions.DontRequireReceiver);

                if (_sCurrentVisibleWidgets.Count == 1)
                {
                    if (_cameraController != null)
                    {
                        _cameraTransform.SendMessage("Activate", _cameraPreset, SendMessageOptions.DontRequireReceiver);
                    }
                    else
                    {
                        _previousLockMode = Cursor.lockState;
                        Cursor.lockState = CursorLockMode.None;
                        Cursor.visible = true;
                    }
                }
            }

            Execute("OnShow", new CallbackEventData());
        }

        public virtual void Close()
        {
            if (!_isShowing)
                return;

            _isShowing = false;
            TweenCanvasGroupAlpha(_canvasGroup.alpha, 0f);
            TweenTransformScale(_rectTransform.localScale, Vector3.zero);

            WidgetUtility.PlaySound(_closeSound, 1.0f);
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
            if (_isShowAndHideCursor)
            {
                _sCurrentVisibleWidgets.Remove(this);
                if (_sCurrentVisibleWidgets.Find(x => x._isFocusPlayer) == null)
                    _cameraController.SendMessage("Focus", false, SendMessageOptions.DontRequireReceiver);
                if (_sCurrentVisibleWidgets.Count == 0)
                {
                    if (_cameraController != null)
                    {
                        _cameraTransform.SendMessage("Deactivate", _cameraPreset, SendMessageOptions.DontRequireReceiver);
                    }
                    else
                    {
                        Cursor.lockState = _previousLockMode;
                        Cursor.visible = false;
                    }
                }
            }
            Execute("OnClose", new CallbackEventData());
        }

        private void TweenCanvasGroupAlpha(float startValue, float targetValue)
        {
            FloatTween alphaTween = new FloatTween
            {
                EaseType = _easeType,
                Duration = _duration,
                StartValue = startValue,
                TargetValue = targetValue,
                IsIgnoreTimeScale = _isIgnoreTimeScale
            };

            alphaTween.AddOnChangedCallback((float value) =>
            {
                _canvasGroup.alpha = value;
            });
            alphaTween.AddOnFinishCallback(() =>
            {
                if (alphaTween.StartValue > alphaTween.TargetValue)
                {
                    if (_isDeactivateOnClose && !_isShowing)
                    {
                        gameObject.SetActive(false);
                    }
                }
            });

            _alphaTweenRunner.StartTween(alphaTween);
        }

        private void TweenTransformScale(Vector3 startValue, Vector3 targetValue)
        {
            Vector3Tween scaleTween = new Vector3Tween
            {
                EaseType = _easeType,
                Duration = _duration,
                StartValue = startValue,
                TargetValue = targetValue,
                IsIgnoreTimeScale = _isIgnoreTimeScale
            };
            scaleTween.AddOnChangedCallback((Vector3 value) =>
            {
                _rectTransform.localScale = value;
            });

            _scaleTweenRunner.StartTween(scaleTween);
        }

        public virtual void Toggle()
        {
            if (!IsVisible)
            {
                Show();
            }
            else
            {
                Close();
            }
        }

        public virtual void Focus()
        {
            _rectTransform.SetAsLastSibling();
        }

        protected virtual void OnDestroy()
        {
            WidgetInputHandler.UnregisterInput(_keyCode, this);
        }

        public void Lock(bool state)
        {
            _isLocked = state;
        }

        public static void LockAll(bool state)
        {
            IUIWidget[] widgets = WidgetUtility.FindAll<IUIWidget>();
            for (int i = 0; i < widgets.Length; i++)
            {
                widgets[i].Lock(state);
            }
        }
    }
}
