using System;

namespace IotDataStation.Util
{
    internal static class CachedDateTime
    {
        private static readonly CachedLocalTimeSource CachedLocalTimeSource = new CachedLocalTimeSource(10);
        private static readonly CachedUtcTimeSource CachedUtcTimeSource = new CachedUtcTimeSource(10);

        public static DateTime Now => CachedLocalTimeSource.Now;
        public static DateTime UtcNow => CachedUtcTimeSource.Now;
    }

    internal class CachedLocalTimeSource : CachedTimeSource
    {
        public CachedLocalTimeSource(int cachingTickCount = 0) : base(cachingTickCount)
        {
        }

        protected override DateTime FreshTime => DateTime.Now;
    }
    internal class CachedUtcTimeSource : CachedTimeSource
    {
        public CachedUtcTimeSource(int cachingTickCount = 0) : base(cachingTickCount)
        {
        }

        protected override DateTime FreshTime => DateTime.UtcNow;
    }

    internal abstract class CachedTimeSource
    {
        private readonly int _cachingTickCount;
        private int _lastTicks = -1;
        private DateTime _lastTime = DateTime.MinValue;
        protected abstract DateTime FreshTime { get; }

        protected CachedTimeSource(int cachingTickCount = 0)
        {
            _cachingTickCount = cachingTickCount;
        }
        /// <summary>
        /// Gets current time cached for one system tick (15.6 milliseconds).
        /// </summary>
        public DateTime Now
        {
            get
            {
                int tickCount = Environment.TickCount;
                if (Math.Abs(tickCount - _lastTicks) < _cachingTickCount)
                {
                    return _lastTime;
                }
                DateTime time = FreshTime;
                _lastTicks = tickCount;
                _lastTime = time;
                return time;
            }
        }
    }
}
