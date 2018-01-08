using System;

namespace UniversalThreadManagement.Internal
{
    /// <summary>
    /// Stopwatch class
    /// Used with WindowsCE and Silverlight which don't have Stopwatch
    /// </summary>
    internal class Stopwatch
    {
        private long _elapsed;
        private bool _isRunning;
        private long _startTimeStamp;

        public Stopwatch() => Reset();

        private long GetElapsedDateTimeTicks()
        {
            long rawElapsedTicks = GetRawElapsedTicks();
            return rawElapsedTicks;
        }

        private long GetRawElapsedTicks()
        {
            long elapsed = _elapsed;
            if (_isRunning)
            {
                long ticks = GetTimestamp() - _startTimeStamp;
                elapsed += ticks;
            }
            return elapsed;
        }

        public static long GetTimestamp() => DateTime.UtcNow.Ticks;

        public void Reset()
        {
           _elapsed = 0L;
           _isRunning = false;
           _startTimeStamp = 0L;
        }

        public void Start()
        {
            if (!_isRunning)
            {
                _startTimeStamp = GetTimestamp();
                _isRunning = true;
            }
        }

        public static Stopwatch StartNew()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            return stopwatch;
        }

        public void Stop()
        {
            if (_isRunning)
            {
                long ticks = GetTimestamp() - _startTimeStamp;
                _elapsed += ticks;
                _isRunning = false;
            }
        }

        // Properties
        public TimeSpan Elapsed => new TimeSpan(GetElapsedDateTimeTicks());

        public long ElapsedMilliseconds => (GetElapsedDateTimeTicks() / 0x2710L);

        public long ElapsedTicks => GetRawElapsedTicks();

        public bool IsRunning => _isRunning;
    }
}
