using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iress.Canada.Reporting.Reports.Other.MarketDataUsageReport.Model
{
    public class AuditEntry<T>
    {
        public DateTime EntryTimestamp
        {
            get;
            protected internal set;
        }

        public T Before
        {
            get;
            protected internal set;
        }

        public T After
        {
            get;
            protected internal set;
        }

        public int ID
        {
            get;
            private set;
        }

        public AuditEntry(DateTime entryTime)
        {
            EntryTimestamp = entryTime;
        }

        public AuditEntry(int id, DateTime entryTime)
        {
            this.ID = id;
            EntryTimestamp = entryTime;
        }
    }
}
