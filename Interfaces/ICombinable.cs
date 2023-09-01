using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iress.Canada.Reporting.Reports.Other.MarketDataUsageReport.Interfaces
{
    public interface ICombinable<T>
    {
        T Combine(T other);
    }
}
