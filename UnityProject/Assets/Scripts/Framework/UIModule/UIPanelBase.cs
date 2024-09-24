using Cysharp.Threading.Tasks;
using Scripts.Fire.Log;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.UIModule
{
    public abstract class UIPanelBase : MonoBehaviour
    {
        public UIPanelLayer panelLayer;
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

        protected virtual void OnEnable()
        {
            OnEnableAsync().Forget();
        }

        protected virtual async UniTaskVoid OnEnableAsync()
        {
            await UniTask.NextFrame();
            GameLog.LogDebug($"{name} OnEnable");
            canvasGroup.blocksRaycasts = true;
        }

        protected virtual void OnDisable()
        {
            OnDisableAsync().Forget();
        }

        protected virtual async UniTaskVoid OnDisableAsync()
        {
            canvasGroup.blocksRaycasts = false;
        }

        protected virtual void OnClose()
        {
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