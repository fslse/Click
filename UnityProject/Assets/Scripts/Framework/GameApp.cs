using System;
using Cysharp.Threading.Tasks;
using Framework.UIModule;
using Scripts.Fire.Log;
using Scripts.Fire.Manager;
using Scripts.Fire.Singleton;
using Scripts.Fire.Startup;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor.SceneManagement;
#endif

namespace Framework
{
    public class GameApp : MonoSingleton<GameApp>
    {
        private void Awake()
        {
            GameLog.LogDebug("GameApp Awake");
        }

        private void Start()
        {
            Init().Forget();
        }

        private async UniTaskVoid Init()
        {
            // UIPanelManager 初始化
            DontDestroyOnLoad(UIPanelManager.Instance.UIRoot);

            // 初始化完成 30%
            UniRx.MessageBroker.Default.Publish(new StartupProgressMessage
            {
                Value = 0.9f
            });

            // 切场景
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
                UniRx.MessageBroker.Default.Publish(new StartupProgressMessage
                {
                    Value = 0.9f + asyncOperation.progress / 0.9f / 10
                });
            }

            await UniTask.Delay(480);
            GameLog.LogWarning("GC");
            GC.Collect();
            GameLog.LogWarning("切场景");
            asyncOperation.allowSceneActivation = true;
        }

#if VERSION_DEV

        private const double MaxFrameTime = 1d / 30;
        private void Update()
        {
            if (Time.deltaTime > MaxFrameTime)
            {
                GameLog.LogWarning($"第{Time.frameCount - 1}帧耗时过长: {Time.deltaTime}");
            }
        }
#endif
    }
}