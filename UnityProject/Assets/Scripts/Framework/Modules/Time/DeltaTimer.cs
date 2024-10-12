using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Framework.Modules.Time
{
    public class DeltaTimer : Timer
    {
        private const PlayerLoopTiming DefaultPlayerLoopTiming = PlayerLoopTiming.EarlyUpdate;

        private int initialFrame;
        private float elapsed;
        private float interval;
        private readonly bool ignoreTimeScale;

        

        protected override void ResetCore(TimeSpan? interval)
        {
            elapsed = 0.0f;
            initialFrame = PlayerLoopHelper.IsMainThread ? UnityEngine.Time.frameCount : -1;
            if (interval != null)
            {
                this.interval = (float)interval.Value.TotalSeconds;
            }
        }

        protected override bool MoveNextCore()
        {
            if (elapsed == 0.0f)
            {
                if (initialFrame == UnityEngine.Time.frameCount)
                {
                    return true;
                }
            }

            elapsed += ignoreTimeScale ? UnityEngine.Time.unscaledDeltaTime : UnityEngine.Time.deltaTime;
            return elapsed < interval;
        }

        public override void InitFromPool()
        {
        }

        public override void RecycleToPool()
        {
            Stop();
        }
    }
}