using System;
using Cysharp.Threading.Tasks;
using Framework.Core.MemoryPool;
using Framework.Modules.Audio;
using Framework.Modules.Time;
using Framework.Modules.UI;
using Scripts.Fire.Log;
using UnityEngine;
using AudioType = Framework.Modules.Audio.AudioType;
using MemoryPoolManager = Framework.Core.MemoryPool.MemoryPoolManager;

namespace Main.GamePlay
{
    public class Example : MonoBehaviour
    {
        private void Awake()
        {
            GameLog.LogDebug("Example Awake");
            var timer = new Timer(TimeSpan.FromSeconds(2), false, PlayerLoopTiming.Update, this.GetCancellationTokenOnDestroy(),
                _ => { UIPanelManager.Instance.ShowPanel("ExamplePanel", UIPanelLayer.Normal).Forget(); });
            timer.Start();

            test();
        }

        private AudioAgent audioAgent;

        private async void test()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(2));
            audioAgent = AudioModule.Instance.Play(AudioType.Music, "Assets/AssetPackages/Audio/AudioClip/test.mp3", true);

            var _ = MemoryPoolManager.Alloc<MyClass>();
        }

        private void Update()
        {
            if (audioAgent == null) return;

            // 检测鼠标左键是否按下
            if (Input.GetMouseButtonDown(0))
            {
                if (audioAgent.IsPlaying)
                    audioAgent.Stop(true);
                else
                    audioAgent.GetAudioSource().Play();
            }
        }
    }

    public class MyClass : MemoryObject
    {
        public override void InitFromPool()
        {
        }

        public override void RecycleToPool()
        {
        }
    }
}