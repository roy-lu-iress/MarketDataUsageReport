using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iress.Canada.Reporting.Reports.Other.MarketDataUsageReport.Model
{
    public class Company
    {
        public virtual int CompanyID
        {
            get;
            protected internal set;
        }

        public virtual string Name
        {
            get;
            protected internal set;
        }

        public virtual string Description
        {
            get;
            protected internal set;
        }

        public virtual List<Group> Groups
        {
            get;
            protected internal set;
        }

        public virtual List<GroupMembership> MemberGroups
        {
            get;
            protected internal set;
        }

        public Company(int id, string name, string description)
        {
            CompanyID = id;
            Name = name;
            Description = description;
        }
    }
}
