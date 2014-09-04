using System.Collections.Generic;

namespace eDrive.DSP.Filters
{
    public interface IFilter
    {
        double Filter(double x);
        double Last { get; }
    }
}