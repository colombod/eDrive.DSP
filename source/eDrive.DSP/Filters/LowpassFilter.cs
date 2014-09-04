namespace eDrive.DSP.Filters
{
    public class LowpassFilter : IFilter
    {
        private readonly double _alpha;

        public LowpassFilter(double alpha)
        {
            _alpha = alpha;
        }

        private double? _hatXPrev;

        public double Last
        {
            get { return _hatXPrev ?? 0.0; }
        }

        public double Filter(double x)
        {
            double hatX = 0;
            if (_hatXPrev == null)
            {

                hatX = x;
            }
            else
            {
                hatX = _alpha * x + (1 - _alpha) * Last;
            }

            _hatXPrev = hatX;

            return hatX;
        }
    }
}