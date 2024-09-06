using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Scripts.Framework.Log;
using UnityEngine;
using UnityEngine.LowLevel;

namespace Scripts.Framework.UniTaskTimer;

public sealed class Timer : PlayerLoopTimer
{
    private int initialFrame;
    private float elapsed;
    private float interval;
    private readonly bool ignoreTimeScale;

    public Timer(TimeSpan interval, bool periodic, PlayerLoopTiming playerLoopTiming, CancellationToken cancellationToken,
        Action<object> timerCallback, object state, bool ignoreTimeScale = false) : base(periodic, playerLoopTiming, cancellationToken, timerCallback, state)
    {
        this.ignoreTimeScale = ignoreTimeScale;
        ResetCore(interval);
    }

    protected override bool MoveNextCore()
    {
        if (elapsed == 0.0f)
        {
            if (initialFrame == Time.frameCount)
            {
                return true;
            }
        }

        elapsed += ignoreTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;
        return elapsed < interval;
    }

    // ReSharper disable once ParameterHidesMember
    protected override void ResetCore(TimeSpan? interval)
    {
        elapsed = 0.0f;
        initialFrame = PlayerLoopHelper.IsMainThread ? Time.frameCount : -1;
        if (interval != null)
        {
            this.interval = (float)interval.Value.TotalSeconds;
        }
    }

    // AfterAssembliesLoaded 表示将会在 BeforeSceneLoad之前调用
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
    public static void InitUniTaskLoop()
    {
        GameLog.LogDebug("InitUniTaskLoop");
        var loop = PlayerLoop.GetCurrentPlayerLoop();
        PlayerLoopHelper.Initialize(ref loop);
    }
}