using System;
using System.Diagnostics;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Framework.TimerModule
{
    public sealed class Timer : PlayerLoopTimer
    {
        private int initialFrame;
        private float elapsed;
        private float interval;
        private readonly bool ignoreTimeScale;

        /// <summary>
        /// Timer 构造函数
        /// </summary>
        /// <param name="interval">间隔</param>
        /// <param name="periodic">是否循环</param>
        /// <param name="playerLoopTiming">tick时间点</param>
        /// <param name="cancellationToken">MonoBehaviour类中一般使用this.GetCancellationTokenOnDestroy()</param>
        /// <param name="timerCallback">回调事件</param>
        /// <param name="state">事件参数</param>
        /// <param name="ignoreTimeScale">是否忽略时间缩放</param>
        public Timer(TimeSpan interval, bool periodic, PlayerLoopTiming playerLoopTiming, CancellationToken cancellationToken,
            Action<object> timerCallback, object state = null, bool ignoreTimeScale = false) : base(periodic, playerLoopTiming, cancellationToken, timerCallback, state)
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

        /// <summary>
        /// Start timer.
        /// </summary>
        public void Start()
        {
            Restart();
        }

        /// <summary>
        /// Stop timer.
        /// </summary>
        public new void Stop()
        {
            base.Stop();
        }

        public new void Dispose()
        {
            base.Dispose();
        }
    }

    public sealed class RealTimer : PlayerLoopTimer
    {
        private ValueStopwatch stopwatch;
        private long intervalTicks;

        public RealTimer(TimeSpan interval, bool periodic, PlayerLoopTiming playerLoopTiming, CancellationToken cancellationToken,
            Action<object> timerCallback, object state = null) : base(periodic, playerLoopTiming, cancellationToken, timerCallback, state)
        {
            ResetCore(interval);
        }

        protected override bool MoveNextCore()
        {
            return stopwatch.ElapsedTicks < intervalTicks;
        }

        protected override void ResetCore(TimeSpan? interval)
        {
            stopwatch = ValueStopwatch.StartNew();
            if (interval != null)
            {
                intervalTicks = interval.Value.Ticks;
            }
        }

        public void Start()
        {
            Restart();
        }

        public new void Stop()
        {
            base.Stop();
        }

        public new void Dispose()
        {
            base.Dispose();
        }
    }

    /// <summary>
    /// 秒表
    /// </summary>
    internal readonly struct ValueStopwatch
    {
        private static readonly double TimestampToTicks = TimeSpan.TicksPerSecond / (double)Stopwatch.Frequency;

        private readonly long startTimestamp;

        private ValueStopwatch(long startTimestamp)
        {
            this.startTimestamp = startTimestamp;
        }

        public static ValueStopwatch StartNew() => new(Stopwatch.GetTimestamp());

        public TimeSpan Elapsed => TimeSpan.FromTicks(ElapsedTicks);

        public bool IsInvalid => startTimestamp == 0;

        public long ElapsedTicks
        {
            get
            {
                if (startTimestamp == 0)
                {
                    throw new InvalidOperationException("Detected invalid initialization(use 'default'), only to create from StartNew().");
                }

                var delta = Stopwatch.GetTimestamp() - startTimestamp;
                return (long)(delta * TimestampToTicks);
            }
        }
    }
}