using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Framework.Core.MemoryManagement;

// ReSharper disable ParameterHidesMember

namespace Framework.Modules.Time
{
    public abstract class Timer : MemoryObject, IDisposable, IPlayerLoopItem
    {
        private bool periodic;
        private PlayerLoopTiming playerLoopTiming;
        private CancellationToken cancellationToken;

        private Action<object> timerCallback;
        private object state;

        private bool isRunning;
        private bool tryStop;
        private bool isDisposed;

        #region Create

        private const PlayerLoopTiming DefaultPlayerLoopTiming = PlayerLoopTiming.EarlyUpdate;

        private static Timer Create(DelayType delayType)
        {
            Timer timer = delayType switch
            {
                DelayType.Realtime => MemoryPool.Alloc<RealTimer>(),
                _ => MemoryPool.Alloc<DeltaTimer>()
            };

            if (delayType == DelayType.UnscaledDeltaTime)
                (timer as DeltaTimer)!.ignoreTimeScale = false;
            return timer;
        }

        public static Timer Create(DelayType delayType, TimeSpan interval, Action<object> timerCallback, object state = null)
        {
            var timer = Create(delayType);
            timer.SetTimerCore(false, DefaultPlayerLoopTiming, CancellationToken.None, timerCallback, state);
            timer.ResetCore(interval);
            return timer;
        }

        public static Timer Create(DelayType delayType, TimeSpan interval, bool periodic, Action<object> timerCallback, object state = null)
        {
            var timer = Create(delayType);
            timer.SetTimerCore(periodic, DefaultPlayerLoopTiming, CancellationToken.None, timerCallback, state);
            timer.ResetCore(interval);
            return timer;
        }

        public static Timer Create(DelayType delayType, TimeSpan interval, CancellationToken cancellationToken, Action<object> timerCallback, object state = null)
        {
            var timer = Create(delayType);
            timer.SetTimerCore(false, DefaultPlayerLoopTiming, cancellationToken, timerCallback, state);
            timer.ResetCore(interval);
            return timer;
        }

        public static Timer Create(DelayType delayType, TimeSpan interval, bool periodic, PlayerLoopTiming playerLoopTiming, Action<object> timerCallback, object state = null)
        {
            var timer = Create(delayType);
            timer.SetTimerCore(periodic, playerLoopTiming, CancellationToken.None, timerCallback, state);
            timer.ResetCore(interval);
            return timer;
        }

        public static Timer Create(DelayType delayType, TimeSpan interval, bool periodic, CancellationToken cancellationToken, Action<object> timerCallback, object state = null)
        {
            var timer = Create(delayType);
            timer.SetTimerCore(periodic, DefaultPlayerLoopTiming, cancellationToken, timerCallback, state);
            timer.ResetCore(interval);
            return timer;
        }

        public static Timer Create(DelayType delayType, TimeSpan interval, bool periodic, PlayerLoopTiming playerLoopTiming, CancellationToken cancellationToken,
            Action<object> timerCallback, object state = null)
        {
            var timer = Create(delayType);
            timer.SetTimerCore(periodic, playerLoopTiming, cancellationToken, timerCallback, state);
            timer.ResetCore(interval);
            return timer;
        }

        private void SetTimerCore(bool periodic, PlayerLoopTiming playerLoopTiming, CancellationToken cancellationToken, Action<object> timerCallback, object state)
        {
            this.periodic = periodic;
            this.playerLoopTiming = playerLoopTiming;
            this.cancellationToken = cancellationToken;
            this.timerCallback = timerCallback;
            this.state = state;
        }

        #endregion

        private bool autoRelease;

        public bool AutoRelease
        {
            get => autoRelease;
            set
            {
                if (periodic)
                {
                    throw new InvalidOperationException("Cannot change 'AutoRelease' because it is periodic.");
                }

                if (isRunning)
                {
                    throw new InvalidOperationException("Cannot change 'AutoRelease' while running.");
                }

                if (isDisposed)
                {
                    throw new ObjectDisposedException("this timer is already disposed.");
                }

                autoRelease = value;
            }
        }

        /// <summary>
        /// 回收
        /// </summary>
        /// <param name="timer"></param>
        public static void Release(Timer timer)
        {
            MemoryPool.Dealloc(timer);
        }

        /// <summary>
        /// Restart(Reset and Start) timer.
        /// </summary>
        public void Restart()
        {
            if (isDisposed) throw new ObjectDisposedException(null);

            ResetCore(null); // init state
            if (!isRunning)
            {
                isRunning = true;
                PlayerLoopHelper.AddAction(playerLoopTiming, this);
            }

            tryStop = false;
        }

        /// <summary>
        /// Restart(Reset and Start) and change interval.
        /// </summary>
        public void Restart(TimeSpan interval)
        {
            if (isDisposed) throw new ObjectDisposedException(null);

            ResetCore(interval); // init state
            if (!isRunning)
            {
                isRunning = true;
                PlayerLoopHelper.AddAction(playerLoopTiming, this);
            }

            tryStop = false;
        }

        /// <summary>
        /// Stop timer.
        /// </summary>
        public void Stop()
        {
            tryStop = true;
        }

        protected abstract void ResetCore(TimeSpan? interval);

        public void Dispose()
        {
            isDisposed = true;
        }

        bool IPlayerLoopItem.MoveNext()
        {
            if (isDisposed)
            {
                isRunning = false;
                return false;
            }

            if (tryStop)
            {
                isRunning = false;
                return false;
            }

            if (cancellationToken.IsCancellationRequested)
            {
                isRunning = false;
                return false;
            }

            if (!MoveNextCore())
            {
                timerCallback(state);

                if (periodic)
                {
                    ResetCore(null);
                    return true;
                }

                if (AutoRelease)
                {
                    Release(this);
                }

                isRunning = false;
                return false;
            }

            return true;
        }

        protected abstract bool MoveNextCore();

        public override void InitFromPool()
        {
            isDisposed = false;
        }

        public override void RecycleToPool()
        {
            Dispose();
        }
    }
}