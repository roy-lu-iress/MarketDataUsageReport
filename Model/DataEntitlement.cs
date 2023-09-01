using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Iress.Canada.Reporting.Reports.Other.MarketDataUsageReport.Model;
using Iress.Canada.Reporting.Reports.Other.MarketDataUsageReport.Helper;

namespace Iress.Canada.Reporting.Reports.Other.MarketDataUsageReport.Model
{
    public class DataEntitlement
    {

        public Entitlement.TradingHours? TradingHoursEntitlement
        {
            get;
            protected internal set;
        }

        public Entitlement.QuoteTiming? QuoteTimingEntitlement
        {
            get;
            protected internal set;
        }

        public Entitlement.SecurityType? EntitlementSecurityType
        {
            get;
            protected set;
        }

        public string Description
        {
            get;
            protected internal set;
        }

        public Exchange EntitledExchange
        {
            get;
            private set;
        }

        public DataEntitlement(Exchange exch, Entitlement.TradingHours hrs, Entitlement.QuoteTiming timing, string desc)
        {
            EntitledExchange = exch;
            TradingHoursEntitlement = hrs;
            QuoteTimingEntitlement = timing;
            Description = desc;
        }

        public DataEntitlement(string desc)
        {
            Description = desc;
            Exchange exch;
            //Console.WriteLine("BUILDING DATA ENTITLEMENT FOR DESCRIPTION: " + desc);

            if (Exchange.exchanges.TryGetValue(desc.Split(' ')[0], out exch))
            {
                //Console.WriteLine("FOUND EXCHANGE FOR ENTITLEMENT");
                EntitledExchange = exch;
            }
        }

        public DataEntitlement(string desc, Entitlement.QuoteTiming qt, Entitlement.SecurityType secType)
        {
            Description = desc;
            QuoteTimingEntitlement = qt;
            EntitlementSecurityType = secType;

            Exchange exch;

            if (Exchange.exchanges.TryGetValue(desc.Split(' ')[0], out exch))
                EntitledExchange = exch;
        }

        public DataEntitlement(string desc, Entitlement.SecurityType secType)
        {
            Description = desc;
            EntitlementSecurityType = secType;

            Exchange exch;

            if (Exchange.exchanges.TryGetValue(desc.Split(' ')[0], out exch))
                EntitledExchange = exch;
        }

        public List<EntitlementOutput> GetEntitlementAccessOutputsForUser(User user, DateTime reportDate, int loginCount, bool collateEntitlements, bool showZeroCountUsers, string channel = "web")
        {
            List<EntitlementOutput> toReturn = new List<EntitlementOutput>();

            StringBuilder entString;

                if ((EntitledExchange != null && EntitlementSecurityType != Entitlement.SecurityType.NonSecurity)
                    || (Description != null && EntitlementSecurityType == Entitlement.SecurityType.Indices))
                {

                    if (loginCount > 0 || (loginCount == 0 && showZeroCountUsers))
                    {
                        string formattedEntStr = FormatExchangeEntitlementString(user, reportDate, channel);
                        toReturn.Add(new EntitlementOutput(formattedEntStr, Description, loginCount));
                    }

                }
                else if (EntitlementSecurityType == Entitlement.SecurityType.NonSecurity)
                {
                    if (loginCount > 0 || (loginCount == 0 && showZeroCountUsers))
                    {
                        entString = new StringBuilder();

                        if (collateEntitlements)
                        {
                            entString.Append(OutputHelper.CsvFieldFormat(user.Name, true));
                            entString.Append(OutputHelper.CsvFieldFormat("Desktop"));
                            entString.Append(OutputHelper.CsvFieldFormat("Web Browser"));
                        }
                        else
                        {
                            if (Description.Trim() == "Globe and Mail News")
                                entString.Append(OutputHelper.CsvFieldFormat("GM", true));
                            else if (Description.Trim() == "BNN Video")
                                entString.Append(OutputHelper.CsvFieldFormat("BNN", true));
                            else if (Description.Trim() == "M")
                                entString.Append(OutputHelper.CsvFieldFormat("MTN", true));
                            else
                                entString.Append(OutputHelper.CsvFieldFormat(Description, true));

                            entString.Append(OutputHelper.CsvFieldFormat(user.Name));
                            entString.Append(OutputHelper.CsvFieldFormat(channel));
                            entString.Append(",,");
                        }

                        if (entString.ToString() != "")
                            toReturn.Add(new EntitlementOutput(entString.ToString(), Description, loginCount));
                        else
                            toReturn.Add(EntitlementOutput.GeneralError);
                    }
                }

            return toReturn;
        }

        private string FormatExchangeEntitlementString(User user, DateTime reportDate, string channel)
        {
            StringBuilder entString;
            entString = new StringBuilder();

            //REMOVED AS PER CIBC REQUEST
            //entString.Append(OutputHelper.CsvFieldFormat(reportDate.ToString("MM'/'dd'/'yyyy"), true));

            switch (EntitlementSecurityType)
            {
                case Entitlement.SecurityType.Equities:
                    entString.Append(OutputHelper.CsvFieldFormat("E", true));
                    break;
                case Entitlement.SecurityType.Indices:
                    entString.Append(OutputHelper.CsvFieldFormat("I", true));
                    break;
                case Entitlement.SecurityType.Options:
                    entString.Append(OutputHelper.CsvFieldFormat("O", true));
                    break;
            }

            entString.Append(OutputHelper.CsvFieldFormat(user.Name));
            entString.Append(OutputHelper.CsvFieldFormat(channel));

            if (QuoteTimingEntitlement == null)
                entString.Append(OutputHelper.CsvFieldFormat("UNKNOWN"));
            else
            {
                switch (QuoteTimingEntitlement)
                {
                    case Entitlement.QuoteTiming.Delayed:
                        entString.Append(OutputHelper.CsvFieldFormat("D"));
                        break;
                    case Entitlement.QuoteTiming.RealTime:
                        entString.Append(OutputHelper.CsvFieldFormat("R"));
                        break;
                }
            }

            entString.Append(OutputHelper.CsvFieldFormat(this.Description));

            //REMOVED IN-MARKET VS. OUT-OF-MARKET HOURS AS PER CIBC
            //entString.Append(OutputHelper.CsvFieldFormat(marketHours ? "Y" : "N"));

            //entString.Append(OutputHelper.CsvFieldFormat("" + loginCount));

            return entString.ToString();
        }
    }
}
