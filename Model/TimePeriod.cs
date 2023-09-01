using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iress.Canada.Reporting.Reports.Other.MarketDataUsageReport.Model
{
    public class TimePeriod
    {
        public TimeSpan Start
        {
            get;
            protected internal set;
        }

        public TimeSpan End
        {
            get;
            protected internal set;
        }

        public TimePeriod(TimeSpan start, TimeSpan end)
        {
            Start = start;
            End = end;
        }

        public bool IsInPeriod(TimeSpan dt)
        {
            return dt >= Start && dt <= End;
        }

        public bool Overlaps (TimePeriod other)
        {
            if (this.Start > other.End || this.End < other.Start)
                return false;
            else
                return true;
        }

        public override string ToString()
        {
            return "" + Start + " TO " + End;
        }
    }
}
