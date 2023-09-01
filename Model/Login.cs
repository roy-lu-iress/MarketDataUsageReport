using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iress.Canada.Reporting.Reports.Other.MarketDataUsageReport.Model
{
    public class Login
    {
        public DateTime LoginDateTime
        {
            get;
            protected internal set;
        }

        public DateTime LogoutDateTime
        {
            get;
            protected internal set;
        }

        public TimeSpan Duration
        {
            get;
            protected internal set;
        }

        public DateTimePeriod DateTimePeriod
        {
            get
            {
                return new DateTimePeriod(LoginDateTime, LogoutDateTime);
            }
        }

        public int RequestCount
        {
            get;
            protected set;
        }

        public Login(DateTime dt, TimeSpan dur, int rqCount)
        {
            LoginDateTime = dt;
            Duration = dur;
            LogoutDateTime = LoginDateTime + Duration;

            RequestCount = rqCount;
        }
    }
}
