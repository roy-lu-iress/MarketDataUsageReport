using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iress.Canada.Reporting.Reports.Other.MarketDataUsageReport.Model
{
    public class Entitlement
    {
        public enum QuoteTiming
        {
            RealTime
            ,Delayed
        }

        public enum Exchange
        {
            TSX
            ,TSXV
            ,CSE
            , NEO
        }

        
        public enum TradingHours
        {
            Market = 1
            ,AfterMarket = 2
        }

        public enum Level
        {
            One = 1
            ,Two = 2
        }

        public enum SecurityType
        {
            Equities
            ,Options
            ,Indices
            ,NonSecurity
        }
    }
}
