using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Framework.Core.MemoryPool;

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

        #region SetTimer

        private const PlayerLoopTiming DefaultPlayerLoopTiming = PlayerLoopTiming.EarlyUpdate;

        public void SetTimer(TimeSpan interval, Action<object> timerCallback, object state = null)
        {
            SetTimerCore(false, DefaultPlayerLoopTiming, CancellationToken.None, timerCallback, state);
            ResetCore(interval);
        }

        public void SetTimer(TimeSpan interval, CancellationToken cancellationToken, Action<object> timerCallback, object state = null)
        {
            SetTimerCore(false, DefaultPlayerLoopTiming, cancellationToken, timerCallback, state);
            ResetCore(interval);
        }

        public void SetTimer(TimeSpan interval, bool periodic, Action<object> timerCallback, object state = null)
        {
            SetTimerCore(periodic, DefaultPlayerLoopTiming, CancellationToken.None, timerCallback, state);
            ResetCore(interval);
        }

        public void SetTimer(TimeSpan interval, bool periodic, PlayerLoopTiming playerLoopTiming, Action<object> timerCallback, object state = null)
        {
            SetTimerCore(periodic, playerLoopTiming, CancellationToken.None, timerCallback, state);
            ResetCore(interval);
        }

        public void SetTimer(TimeSpan interval, bool periodic, CancellationToken cancellationToken, Action<object> timerCallback, object state = null)
        {
            SetTimerCore(periodic, DefaultPlayerLoopTiming, cancellationToken, timerCallback, state);
            ResetCore(interval);
        }

        public void SetTimer(TimeSpan interval, bool periodic, PlayerLoopTiming playerLoopTiming, CancellationToken cancellationToken, Action<object> timerCallback,
            object state = null)
        {
            SetTimerCore(periodic, playerLoopTiming, cancellationToken, timerCallback, state);
            ResetCore(interval);
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

                isRunning = false;
                return false;
            }

            return true;
        }

        protected abstract bool MoveNextCore();
    }
}