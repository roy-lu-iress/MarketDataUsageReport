using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iress.Canada.Reporting.Reports.Other.MarketDataUsageReport.Model
{
    using EntitlementChar = Tuple<char, int>;
    public class Group : IEquatable<Group>
    {
        public virtual int GroupID
        {
            get;
            protected internal set;
        }

        public virtual int PermissionID
        {
            get;
            protected internal set;
        }

        public virtual string Description
        {
            get;
            protected internal set;
        }

        public List<User> Users
        {
            get;
            protected internal set;
        }

        public List<EntitlementSet> EntitlementSets
        {
            get;
            protected internal set;
        }

        public List<GroupMembership> GroupMemberships
        {
            get;
            protected internal set;
        }

        public Group(int id, string description)
        {
            GroupID = id;
            Description = description;

            EntitlementSets = new List<EntitlementSet>();

            for(int i = 0; i < Description.Length; i++)
            {
                EntitlementChar key = new Tuple<char, int>(Description[i], i);

                EntitlementSet entSet;
                if (EntitlementSet.EntitlementSets.TryGetValue(key, out entSet))
                    EntitlementSets.Add(entSet);
            }
        }

        public bool Equals(Group other)
        {
            bool equality = this.GroupID == other.GroupID;
            equality = equality && this.PermissionID == other.PermissionID;
            equality = equality && this.Description == other.Description;
            return equality;
        }

        /*public List<string> GenerateEntitlementStringsForUser(User user, DateTime reportDate)
        {
            if (Description.Length != 5)
                return new List<string>(new string[] { "ERROR - DESCRIPTION LENGTH NOT 5" });

            List<string> entitlementStrings = new List<string>();

            foreach(EntitlementSet entSet in EntitlementSets)
            {
                entitlementStrings.AddRange(GenerateStringsForUserEntitlementSet(user, entSet, reportDate));
            }

            return entitlementStrings;
        }*/

        /*public List<string> GenerateStringsForUserEntitlementSet(User user, EntitlementSet entSet, DateTime reportDate)
        {
            List<string> entitlementStrings = new List<string>();

            foreach(DataEntitlement entitlement in entSet.Entitlements)
            {
                entitlementStrings.AddRange(entitlement.GetEntitlementAccessStringsForUser(user, this.Description, reportDate));
            }

            return entitlementStrings;
        }*/
    }
}
