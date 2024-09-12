using DG.Tweening;
using Scripts.Fire.Log;
using Scripts.Fire.Startup;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public sealed class StartupModel
{
    public ReactiveProperty<float> CurrentProgress { get; }

    public StartupModel()
    {
        CurrentProgress = new ReactiveProperty<float>(0.0f);
        MessageBroker.Default.Receive<StartupProgressMessage>().Subscribe(progress => { CurrentProgress.Value = progress.Value; });
    }
}

namespace Scripts.Fire
{
    public class HotFixPresenter : MonoBehaviour
    {
        private StartupModel model;
        private Tweener tween;

        [SerializeField] private Slider slider;
        [SerializeField] private TMP_Text text;

        private void Awake()
        {
            GameLog.LogDebug("HotFixPresenter Awake");

            model = new StartupModel();
            model.CurrentProgress.Subscribe(target =>
            {
                GameLog.LogDebug($"HotFixPresenter Progress: {target}");

                if (target <= slider.value) return;

                tween?.Kill();
                tween = DOTween.To(() => slider.value, x =>
                {
                    slider.value = x / 100f;
                    text.text = $"{x:F1}%";
                }, target, 0.5f);

                tween.OnComplete(() =>
                {
                    if (target >= 0.999999f)
                    {
                        GameLog.LogWarning("HotFixPresenter Progress Complete");
                    }
                });
            });
        }
    }
}