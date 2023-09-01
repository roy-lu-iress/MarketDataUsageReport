using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iress.Canada.Reporting.Reports.Other.MarketDataUsageReport.Interfaces;
using Iress.Canada.Reporting.Reports.Other.MarketDataUsageReport.Helper;

namespace Iress.Canada.Reporting.Reports.Other.MarketDataUsageReport.Model
{
    public class EntitlementOutput : IEquatable<EntitlementOutput>, ICombinable<EntitlementOutput>
    {
        public static EntitlementOutput GeneralError = new EntitlementOutput("ERROR - NOT WELL FORMATTED ENTITLEMENT OUTPUT", "ERROR - NOT WELL FORMATTED ENTITLEMENT OUTPUT", - 1);
        public static EntitlementOutput EntitlementCodeError = new EntitlementOutput("ERROR - DESCRIPTION LENGTH NOT 5", "ERROR - DESCRIPTION LENGTH NOT 5", - 1);

        public string EntitlementDescription
        {
            get;
            private set;
        }

        public string EntitlementCodeString
        {
            get;
            protected internal set;
        }

        private int m_loginCount;
        public int LoginCount
        {
            get
            {
                return m_loginCount;
            }
            protected internal set
            {
                if (value >= 0)
                    m_loginCount = value;
                else
                    m_loginCount = 0;
            }
        }

        public string EntitlementString
        {
            get
            {
                if (LoginCount != -1)
                    return EntitlementCodeString + OutputHelper.CsvFieldFormat(LoginCount.ToString());
                else
                    return EntitlementCodeString;
            }
        }

        public EntitlementOutput(string entitlementCodes, string description) : this(entitlementCodes, description, 0)
        {
        }

        public EntitlementOutput(string entitlementCodes, string description, int loginCount)
        {
            EntitlementCodeString = entitlementCodes;
            EntitlementDescription = description;
            m_loginCount = loginCount;
        }

        public bool Equals(EntitlementOutput other)
        {
            return this.EntitlementCodeString == other.EntitlementCodeString
                    && this.EntitlementDescription == other.EntitlementDescription;
        }

        public EntitlementOutput Combine(EntitlementOutput other)
        {
            if (!this.Equals(other))
                throw new ArgumentException("Cannot combine unequal EntitlementOutput objects");

            this.LoginCount += other.LoginCount;
            return this;
        }

        public override int GetHashCode()
        {
            int hash = 13;

            if (EntitlementCodeString != null)
                hash = hash * 17 + EntitlementCodeString.GetHashCode();

            if (EntitlementDescription != null)
                hash = hash * 23 + EntitlementDescription.GetHashCode();

            //Login count shouldn't affect equality, so shouldn't affect hash
            //hash = hash * 31 + LoginCount;

            return hash;
        }
    }
}
