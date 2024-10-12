using System;
using System.Reflection;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Framework.Modules.Audio;
using Framework.Modules.ObjectPool;
using Framework.Modules.UI;
using Scripts.Fire;
using Scripts.Fire.Log;
using Scripts.Fire.Manager;
using Scripts.Fire.Singleton;
using Scripts.Fire.Startup;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

namespace Framework
{
    public partial class GameApp : MonoSingleton<GameApp>
    {
        private Image mask;

        private void Awake()
        {
            GameLog.LogDebug("GameApp Awake");

            var transition = GameObject.Find("--- Transition ---");
            DontDestroyOnLoad(transition);
            mask = transition.transform.Find("Mask").GetComponent<Image>();
        }

        public void Transition(Action action, float duration = 0.5f)
        {
            mask.gameObject.SetActive(true);
            var seq = DOTween.Sequence();
            var tweener1 = DOTween.To(() => mask.color, value => mask.color = value, new Color(0, 0, 0, 1), duration);
            var tweener2 = DOTween.To(() => mask.color, value => mask.color = value, new Color(0, 0, 0, 0), duration);
            seq.Append(tweener1);
            seq.AppendCallback(() => { action?.Invoke(); });
            seq.Append(tweener2);
            seq.AppendCallback(() => { mask.gameObject.SetActive(false); });
        }
    }

    public partial class GameApp
    {
        /// <summary>
        /// 框架初始化 + 业务初始化 + 切场景
        /// <para></para>
        /// 业务初始化在 Assembly-CSharp 的 NewBehaviourScript
        /// <para></para>
        /// 业务初始化完成后调用 GameApp.Instance.StartGame() 切场景
        /// <remarks> 10% + 20% + 10% </remarks>
        /// </summary>
        private void Start()
        {
            // UIPanelManager 初始化
            DontDestroyOnLoad(UIPanelManager.Instance.UIRoot);

            MessageBroker.Default.Publish(StartupProgressMessage.Instance.Message(0.65f));

            // GameApp下模块初始化

            // AudioModule 初始化
            if (!AudioModule.Instance.InstanceRoot.parent)
                DontDestroyOnLoad(AudioModule.Instance.InstanceRoot.gameObject);
            // ObjectPoolModule 初始化
            if (!ObjectPoolModule.Instance.InstanceRoot.parent)
                DontDestroyOnLoad(ObjectPoolModule.Instance.InstanceRoot.gameObject);

            MessageBroker.Default.Publish(StartupProgressMessage.Instance.Message(0.7f));

            // 业务初始化
            Type type = GameManager.Instance.assembly[3].GetType("Main.NewBehaviourScript");
            gameObject.AddComponent(type);
        }

        public async UniTaskVoid StartGame()
        {
            // 切场景，开始游戏 10%
#if UNITY_EDITOR
            AsyncOperation asyncOperation = EditorSceneManager.LoadSceneAsyncInPlayMode("Assets/Scenes/Game.unity", new LoadSceneParameters(LoadSceneMode.Single));
#else
            await AssetManager.Instance.LoadAssetBundleAsync("scenes.ab");
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync("Scenes/Game");
#endif
            asyncOperation.allowSceneActivation = false;
            while (asyncOperation.progress < 0.899999f)
            {
                await UniTask.NextFrame();
                MessageBroker.Default.Publish(StartupProgressMessage.Instance.Message(0.9f + asyncOperation.progress / 0.9f / 10));
            }


            var _ = typeof(MessageBroker).GetField("isDisposed", BindingFlags.Instance | BindingFlags.NonPublic);
            await UniTask.WaitUntil(() => (bool)_!.GetValue(MessageBroker.Default));

            GameLog.LogWarning("GC");
            GC.Collect();

            await UniTask.NextFrame();
            await UniTask.NextFrame();

            GameLog.LogWarning("Transition");
            Transition(() =>
            {
                GameLog.LogWarning("切场景");
                asyncOperation.allowSceneActivation = true;
            });
        }
    }
}