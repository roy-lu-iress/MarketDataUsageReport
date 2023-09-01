using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iress.Canada.Reporting.Reports.Other.MarketDataUsageReport.Model
{
    public class GroupMembership
    {
        public Group EntitlementGroup
        {
            get;
            protected internal set;
        }

        public DateTimePeriod MembershipPeriod
        {
            get;
            protected internal set;
        }

        public GroupMembership(Group grp, DateTimePeriod period)
        {
            EntitlementGroup = grp;
            MembershipPeriod = period;
        }
    }
}
