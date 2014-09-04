using System;
using System.Reactive.Concurrency;

namespace eDrive.DSP.Filters
{
    public class OneEuroFilter : IFilter
    {
        private double _freq;
        private readonly double _mincutoff;
        private readonly double _beta;
        private readonly double _dcutoff;
        private LowpassFilter _xFilter;
        private LowpassFilter _dxFilter;

        public OneEuroFilter(double freq, double mincutoff, double beta, double dcutoff, IScheduler scheduler = null)
        {
            _scheduler = scheduler ?? ImmediateScheduler.Instance;
            _freq = freq;
            _mincutoff = mincutoff;
            _beta = beta;
            _dcutoff = dcutoff;

            _xFilter = new LowpassFilter(Alpha(_mincutoff));
            _dxFilter = new LowpassFilter(Alpha(_dcutoff));
        }

        public double Filter(double x)
        {
            var now = _scheduler.Now;

            if (_lastTime != DateTimeOffset.MinValue)
            {
                _freq = 1.0 / ((now - _lastTime).Seconds);
            }
            _lastTime = now;

            if (_hatXPrev != null && _hatXPrev == x)
            {
                return _hatXPrev.Value;
            }

            var previousX = _hatXPrev;
            var dx = (x - previousX) * _freq;
            _dxFilter = new LowpassFilter(Alpha(_dcutoff));
            var edx = _dxFilter.Filter(dx.Value);
            var cutoff = _mincutoff + _beta * Math.Abs(edx);

            _xFilter = new LowpassFilter(Alpha(cutoff));
            var hatX = _xFilter.Filter(x);
            _hatXPrev =hatX;

            return hatX;
        }

        private const double K = 2 * Math.PI;
        private double Alpha(double cutoff)
        {
            var te = 1.0 / _freq;
            var tau = 1.0 / (K * cutoff);
            return 1.0 / (1.0 + tau / te);
        }

        public double Last
        {
            get { return _hatXPrev ?? 0.0; }
        }

        private double? _hatXPrev;
        private readonly IScheduler _scheduler;
        private DateTimeOffset _lastTime;
    }
}