using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iress.Canada.Reporting.Reports.Other.MarketDataUsageReport.Model
{

    public class User : IEquatable<User>
    {
        public int UserID
        {
            get;
            protected internal set;
        }

        public int GroupID
        {
            get;
            protected internal set;
        }

        public string Name
        {
            get;
            protected internal set;
        }

        public string Description
        {
            get;
            protected internal set;
        }

        public List<Login> Logins
        {
            get;
            protected internal set;
        }

        public Group EntitlementGroup
        {
            get;
            protected internal set;
        }

        public string DummyEntitlementString
        {
            get;
            protected internal set;
        }

        public List<GroupMembership> GroupMemberships
        {
            get;
            protected internal set;
        }

        public Dictionary<string, int> EntitlementStringLoginCounts
        {
            get;
            protected set;
        }

        public Dictionary<string, List<Login>> EntitlementStringLoginLists
        {
            get;
            protected set;
        }

        public User(int id, string name, string description)
        {
            UserID = id;
            Name = name.Trim();
            Description = description.Trim();

            Logins = new List<Login>();
            EntitlementStringLoginCounts = new Dictionary<string, int>();
            EntitlementStringLoginLists = new Dictionary<string, List<Login>>();
        }

        public User(int id, string name, string description, string dummyEntitlementString)
        {
            UserID = id;
            Name = name.Trim();
            Description = description.Trim();

            DummyEntitlementString = dummyEntitlementString;

            Logins = new List<Login>();
            EntitlementStringLoginCounts = new Dictionary<string, int>();
            EntitlementStringLoginLists = new Dictionary<string, List<Login>>();
        }

        public bool Equals(User other)
        {
            return this.UserID == other.UserID;
        }

        public int GetLoginsInPeriod(TimePeriod period)
        {
            int count = 0;

            foreach(Login login in Logins)
            {
                if (period.IsInPeriod(login.LoginDateTime.TimeOfDay) || period.IsInPeriod(login.LogoutDateTime.TimeOfDay))
                    count++;
            }

            return count;
        }
    }
}
