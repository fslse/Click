using System;
using System.Reflection;
using Cysharp.Threading.Tasks;
using Framework;
using Scripts.Fire;
using Scripts.Fire.Log;
using Scripts.Fire.Manager;
using Scripts.Fire.Singleton;
using Scripts.Fire.Startup;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

namespace Main
{
    public class GameApp : MonoSingleton<GameApp>
    {
        private void Start()
        {
            GameLog.LogDebug("GameApp Start");
            StartGame().Forget();
        }

        /// <summary>
        /// 框架初始化 + 业务初始化 + 切场景
        /// <remarks> 5% + 25% + 10% </remarks>
        /// </summary>
        public async UniTaskVoid StartGame()
        {
            // 框架初始化 = 5%
            if (GameSystem.Instance)
                MessageBroker.Default.Publish(StartupProgressMessage.Instance.Message(0.65f));

            // 业务初始化 = 25%
            // todo: 业务逻辑初始化

            // 切场景 = 10%
#if UNITY_EDITOR
            AsyncOperation asyncOperation = EditorSceneManager.LoadSceneAsyncInPlayMode("Assets/Scenes/Game.unity", new LoadSceneParameters(LoadSceneMode.Single));
#else
            await AssetManager.Instance.LoadAssetBundleAsync("scenes.ab");
            AsyncOperation asyncOperation = SceneManager.LoadSceneAsync("Scenes/Game");
#endif
            asyncOperation.allowSceneActivation = false;
            while (asyncOperation.progress < 0.9f)
            {
                await UniTask.NextFrame();
                MessageBroker.Default.Publish(StartupProgressMessage.Instance.Message(0.9f + asyncOperation.progress / 0.9f / 10));
            }

            MessageBroker.Default.Publish(StartupProgressMessage.Instance.Message(1));

            var _ = typeof(MessageBroker).GetField("isDisposed", BindingFlags.Instance | BindingFlags.NonPublic);
            await UniTask.WaitUntil(() => (bool)_!.GetValue(MessageBroker.Default));

            GameLog.LogWarning("GC");
            GC.Collect();

            await UniTask.NextFrame();
            await UniTask.NextFrame();

            GameLog.LogWarning("Transition");
            GameManager.Instance.Transition(() =>
            {
                GameLog.LogWarning("切场景");
                asyncOperation.allowSceneActivation = true;
            });
        }
    }
}