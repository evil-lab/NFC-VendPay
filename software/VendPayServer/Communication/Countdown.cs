using System;
using System.Diagnostics;

namespace com.IntemsLab.Communication
{
    public sealed class Countdown
    {
        private readonly TimeSpan _period;
        private readonly Stopwatch _stopwatch = new Stopwatch();

        public Countdown(TimeSpan period)
        {
            this._period = period;
        }

        public Countdown(long period)
        {
            this._period = TimeSpan.FromMilliseconds(period);
        }

        public TimeSpan Elapsed
        {
            get
            {
                var elapsed = this._stopwatch.Elapsed;
                if (elapsed <= this._period)
                {
                    return elapsed;
                }

                return this.Period;
            }
        }

        public long ElapsedMilliseconds
        {
            get { return (long)this.Elapsed.TotalMilliseconds; }
        }

        public bool IsOver
        {
            get { return this.Elapsed >= this.Period; }
        }

        public bool IsRunning
        {
            get { return this._stopwatch.IsRunning; }
        }

        public TimeSpan Period
        {
            get { return this._period; }
        }

        public TimeSpan Remains
        {
            get { return this._period - this._stopwatch.Elapsed; }
        }

        public long RemainsMilliseconds
        {
            get { return (long)this.Remains.TotalMilliseconds; }
        }

        public void Reset()
        {
            this._stopwatch.Stop();
            this._stopwatch.Reset();
        }

        public void Restart()
        {
            var running = this._stopwatch.IsRunning;
            this._stopwatch.Stop();
            this._stopwatch.Reset();

            if (running)
            {
                this._stopwatch.Start();
            }
        }

        public void Start()
        {
            this._stopwatch.Start();
        }

        public static Countdown StartNew(long period)
        {
            var countdown = new Countdown(period);
            countdown.Start();
            return countdown;
        }

        public static Countdown StartNew(TimeSpan period)
        {
            var countdown = new Countdown(period);
            countdown.Start();
            return countdown;
        }

        public void Stop()
        {
            this._stopwatch.Stop();
        }
    }
}