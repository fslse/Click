using Cysharp.Threading.Tasks;
using DG.Tweening;
using Scripts.Fire.Log;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.UIModule
{
    public abstract class UIPanelBehaviour : MonoBehaviour
    {
        [Header("Base")] // / 
        [SerializeField]
        protected RectTransform rectTransform;

        [SerializeField] protected CanvasGroup canvasGroup;

        protected enum AnimationType
        {
            None,
            Fade,
            Zoom,
            Animator
        }

        [Header("Animation Settings")] // / 
        [SerializeField]
        protected Animator animator;

        [SerializeField] protected AnimationType animationType = AnimationType.Fade;
        [SerializeField] protected AnimationCurve animationCurve;
        [SerializeField] protected float animationDuration = 0.25f;

        protected virtual void Awake()
        {
            GameLog.LogDebug($"{name} Awake");

            rectTransform = transform as RectTransform;

            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }

            animator = GetComponent<Animator>();
        }

        protected void Show()
        {
            switch (animationType)
            {
                case AnimationType.Fade:
                    DisableAnimator();
                    canvasGroup.alpha = 0;
                    if (animationCurve.length == 0)
                        canvasGroup.DOFade(1, animationDuration).SetEase(Ease.Linear).SetId(int.MaxValue - 1);
                    else
                        canvasGroup.DOFade(1, animationDuration).SetEase(animationCurve).SetId(int.MaxValue - 1);
                    break;
                case AnimationType.Zoom:
                    DisableAnimator();
                    rectTransform.localScale = Vector3.zero;
                    if (animationCurve.length == 0)
                        rectTransform.DOScale(Vector3.one, animationDuration).SetEase(Ease.Linear).SetId(int.MaxValue - 2);
                    else
                        rectTransform.DOScale(Vector3.one, animationDuration).SetEase(animationCurve).SetId(int.MaxValue - 2);
                    break;
                case AnimationType.Animator:
                    EnableAnimator();
                    break;
                case AnimationType.None:
                default:
                    DisableAnimator();
                    break;
            }
        }

        private void EnableAnimator()
        {
            if (animator)
                animator.enabled = true;
        }

        private void DisableAnimator()
        {
            if (animator)
                animator.enabled = false;
        }

        protected async UniTask Hide(TweenCallback tweenCallback = null)
        {
            canvasGroup.blocksRaycasts = false;
            switch (animationType)
            {
                case AnimationType.Fade when animationCurve.length == 0:
                    await canvasGroup.DOFade(0, animationDuration).SetEase(Ease.Linear);
                    break;
                case AnimationType.Fade:
                    await canvasGroup.DOFade(0, animationDuration).SetEase(animationCurve);
                    break;
                case AnimationType.Zoom when animationCurve.length == 0:
                    await rectTransform.DOScale(Vector3.zero, animationDuration).SetEase(Ease.Linear);
                    break;
                case AnimationType.Zoom:
                    await rectTransform.DOScale(Vector3.zero, animationDuration).SetEase(animationCurve);
                    break;
                case AnimationType.Animator:
                    animator.Play("popup_close");
                    await UniTask.WaitUntil(() =>
                    {
                        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                        if (stateInfo.IsName("Base Layer.popup_close") == false)
                            return true;
                        return stateInfo.IsName("Base Layer.popup_close") && stateInfo.normalizedTime >= 0.999f;
                    });
                    break;
                case AnimationType.None:
                default:
                    break;
            }

            tweenCallback?.Invoke();
        }

#if VERSION_DEV
        private void OnEnable()
        {
            GameLog.LogDebug($"{name} OnEnable");
        }

        private void Start()
        {
            GameLog.LogDebug($"{name} Start");
        }

        private void OnDisable()
        {
            GameLog.LogDebug($"{name} OnDisable");
        }
#endif
    }

    /// <summary>
    /// UIPanel基类
    /// <para></para>
    /// --- 不建议使用 OnEnable、Start、OnDisable
    /// <para></para>
    /// --- 初始化工作放在 Awake、Init
    /// <para></para>
    /// --- 游戏逻辑从 OnStart 开始
    /// </summary>
    public abstract class UIPanelBase : UIPanelBehaviour
    {
        public UIPanelLayer PanelLayer { get; set; }
        public string PanelName => name;

        [Header("UIBehaviour")] [SerializeField]
        private Button closeButton;

        protected override void Awake()
        {
            base.Awake();

            foreach (var button in GetComponentsInChildren<Button>())
            {
                if (button.name == "CloseButton")
                    closeButton = button.GetComponent<Button>();
            }

            if (closeButton != null)
            {
                closeButton.onClick.AddListener(OnClose);
            }
        }

        public async UniTaskVoid OnEnter(object udata)
        {
            GameLog.LogDebug($"{name} OnEnter");
            canvasGroup.blocksRaycasts = false;
            gameObject.SetActive(true);
            Show();
            Init(udata);

            // 等待动画结束
            switch (animationType)
            {
                case AnimationType.Fade:
                    await UniTask.WaitUntil(() => DOTween.IsTweening(int.MaxValue - 1) == false);
                    break;
                case AnimationType.Zoom:
                    await UniTask.WaitUntil(() => DOTween.IsTweening(int.MaxValue - 2) == false);
                    break;
                case AnimationType.Animator:
                    await UniTask.WaitUntil(() =>
                    {
                        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                        if (stateInfo.IsName("Base Layer.popup_open") == false)
                            return true;
                        return stateInfo.IsName("Base Layer.popup_open") && stateInfo.normalizedTime >= 0.999f;
                    });
                    break;
                case AnimationType.None:
                default:
                    break;
            }

            canvasGroup.blocksRaycasts = true;
            GameLog.LogDebug($"{name} OnStart");
            OnStart();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        protected abstract void Init(object udata);

        /// <summary>
        /// 初始化完成且动画结束后调用
        /// </summary>
        protected abstract void OnStart();

        protected virtual void OnClose()
        {
            Hide(() => UIPanelManager.Instance.RecyclePanel(this)).Forget();
        }
    }

    public enum UIPanelLayer
    {
        /// <summary>
        /// 主玩法UI或场景的一部分
        /// </summary>
        MainGame,

        /// <summary>
        /// UI小游戏，如：老虎机、刮刮乐等
        /// </summary>
        MiniGame,

        /// <summary>
        /// 普通UI，一级、二级、三级等窗口
        /// </summary>
        Normal,

        /// <summary>
        /// 消息通知
        /// </summary>
        Message,

        /// <summary>
        /// 错误弹窗，网络连接弹窗等
        /// </summary>
        System,

        /// <summary>
        /// 已回收待复用的UIPanel
        /// </summary>
        Recycle
    }
}