using System;
using System.Diagnostics;

namespace Framework.Modules.Time
{
    public class RealTimer : Timer
    {
        private ValueStopwatch stopwatch;
        private long intervalTicks;

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

        public override void InitFromPool()
        {
        }

        public override void RecycleToPool()
        {
            Stop();
        }
    }

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