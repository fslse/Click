using System;
using Cysharp.Threading.Tasks;
using Framework.TimeModule;
using Framework.UIModule;
using Scripts.Fire.Log;
using UnityEngine;

namespace Game.GameLogic
{
    public class Example : MonoBehaviour
    {
        private void Awake()
        {
            GameLog.LogDebug("Example Awake");
            var timer = new Timer(TimeSpan.FromSeconds(2), false, PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy(),
                _ => { UIPanelManager.Instance.ShowPanel("ExamplePanel", UIPanelLayer.Normal).Forget(); });
            timer.Start();
        }
    }
}