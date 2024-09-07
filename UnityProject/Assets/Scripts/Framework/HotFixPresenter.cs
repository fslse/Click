using System;
using Cysharp.Threading.Tasks;
using Scripts.Framework.Log;
using Scripts.Framework.UniTaskTimer;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Scripts.Framework
{
    public class HotFixPresenter : MonoBehaviour
    {
        private void Start()
        {
            // 
            GameLog.LogDebug("HotFixPresenter Start");

            // StartGame().Forget();

            var timer = new Timer(TimeSpan.FromSeconds(3), false, PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy(),
                obj =>
                {
                    GameLog.LogDebug("Timer");
                    SceneManager.LoadSceneAsync("Scenes/Game");
                }, null);
            timer.Restart();
        }

        private static async UniTaskVoid StartGame()
        {
            await UniTask.Delay(3000);
            SceneManager.LoadSceneAsync("Scenes/Game");
        }
    }
}