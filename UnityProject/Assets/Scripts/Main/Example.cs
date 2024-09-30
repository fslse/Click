using System;
using Cysharp.Threading.Tasks;
using Framework.Audio;
using Framework.Time;
using Framework.UI;
using Scripts.Fire.Log;
using UnityEngine;
using AudioType = Framework.Audio.AudioType;

namespace Main
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
        }

        private void Update()
        {
            // 检测鼠标左键是否按下
            if (Input.GetMouseButtonDown(0))
            {
                if (audioAgent.IsPlaying)
                    audioAgent.Stop(true);
                else
                    audioAgent.GetAudioSource().Play();
            }
            else if (Input.GetMouseButtonDown(1))
            {
                AudioModule.Instance.MusicEnable = !AudioModule.Instance.MusicEnable;
            }
        }
    }
}