using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iress.Canada.Reporting.Reports.Other.MarketDataUsageReport.Model
{
    public class EntitlementSetMembership
    {
        public EntitlementSet EntitlementSet
        {
            get;
            protected internal set;
        }

        public DateTimePeriod MembershipPeriod
        {
            get;
            protected internal set;
        }
    }
}
