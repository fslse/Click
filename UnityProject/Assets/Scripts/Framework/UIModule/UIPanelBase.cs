using System.Diagnostics;
using Scripts.Fire.Log;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.UIModule
{
    public abstract class UIPanelBase : MonoBehaviour
    {
        public UIPanelLayer PanelLayer { get; set; }
        public string PanelName => name;

        protected CanvasGroup canvasGroup;
        protected Button closeButton;

        protected virtual void Awake()
        {
            GameLog.LogDebug($"{name} Awake");

            foreach (var button in GetComponentsInChildren<Button>())
            {
                if (button.name == "CloseButton")
                    closeButton = button.GetComponent<Button>();
            }

            canvasGroup = GetComponent<CanvasGroup>();

            if (closeButton != null)
            {
                closeButton.onClick.AddListener(OnClose);
            }

            if (canvasGroup == null)
            {
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
        }

        #region 禁用部分生命周期函数

        [Conditional("VERSION_DEV")]
        private void OnEnable()
        {
            GameLog.LogDebug($"{name} OnEnable");
        }

        [Conditional("VERSION_DEV")]
        private void Start()
        {
            GameLog.LogDebug($"{name} Start");
        }

        [Conditional("VERSION_DEV")]
        private void OnDisable()
        {
            GameLog.LogDebug($"{name} OnDisable");
        }

        #endregion

        /// <summary>
        /// 初始化
        /// </summary>
        public virtual void Init(object udata)
        {
            canvasGroup.blocksRaycasts = false;
            gameObject.SetActive(true);

            GameLog.LogDebug($"{name} Init");
            // todo: 进入动画 回调OnStart

            // 子类重写 初始化
        }

        /// <summary>
        /// 动画结束后调用
        /// </summary>
        protected virtual void OnStart()
        {
            canvasGroup.blocksRaycasts = true;
        }

        protected virtual void OnClose()
        {
            // todo: 退出动画 回调回收
            UIPanelManager.Instance.RecyclePanel(this);
        }
    }


    public enum UIPanelLayer
    {
        /// <summary>
        /// Home、大厅、纯UI游戏主玩法界面等
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