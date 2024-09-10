using System;
using Cysharp.Threading.Tasks;
using Scripts.Fire.Log;
using Scripts.Fire.UniTaskTimer;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scripts.Fire
{
    public class HotFixPresenter : MonoBehaviour
    {
        private void Start()
        {
            // 
            GameLog.LogDebug("HotFixPresenter Start");

            StartGame().Forget();
        }

        private async UniTaskVoid StartGame()
        {
            // 计时器测试
            var timer = new Timer(TimeSpan.FromSeconds(3), false, PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy(),
                _ =>
                {
                    GameLog.LogDebug("Timer");
                    SceneManager.LoadSceneAsync("Scenes/Game");
                });
            timer.Start();

            await UniTask.Delay(1000);

            timer.Dispose();

            var realTimer = new RealTimer(TimeSpan.FromSeconds(2), false, PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy(),
                _ =>
                {
                    GameLog.LogDebug("RealTimer");
                    SceneManager.LoadSceneAsync("Scenes/Game");
                });
            realTimer.Start();
        }
    }
}