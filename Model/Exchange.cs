using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iress.Canada.Reporting.Reports.Other.MarketDataUsageReport.Model
{
    public class Exchange
    {
        public static Dictionary<string, Exchange> exchanges;
        

        static Exchange()
        {
            exchanges = new Dictionary<string, Exchange>();

            Exchange tsx, tsxv, cse, neo, mx, nyse, amex, nasdaq, opra;
            tsx = new Exchange("TSX");
            tsxv = new Exchange("TSX-V");
            cse = new Exchange("CSE");
            neo = new Exchange("NEO");
            mx = new Exchange("MX");
            nyse = new Exchange("NYSE");
            amex = new Exchange("AMEX");
            nasdaq = new Exchange("NASDAQ");
            opra = new Exchange("OPRA");

            tsx.PreMarketHours = new TimePeriod(new TimeSpan(0, 0, 0, 0, 0), new TimeSpan(0, 9, 29, 59, 999));
            tsx.MarketHours = new TimePeriod(new TimeSpan(9, 30, 0), new TimeSpan(17, 0, 0));
            //tsx.ExtendedHours = new TimePeriod(new TimeSpan(16, 15, 0), new TimeSpan(17, 0, 0));
            tsx.AfterMarketHours = new TimePeriod(new TimeSpan(0, 17, 0, 0, 1), new TimeSpan(0, 23, 59, 59, 999));
            exchanges.Add("TSX", tsx);

            tsxv.PreMarketHours = new TimePeriod(new TimeSpan(0, 0, 0, 0, 0), new TimeSpan(0, 9, 29, 59, 999));
            tsxv.MarketHours = new TimePeriod(new TimeSpan(9, 30, 0), new TimeSpan(17, 0, 0));
            //tsxv.ExtendedHours = new TimePeriod(new TimeSpan(16, 15, 0), new TimeSpan(17, 0, 0));
            tsxv.AfterMarketHours = new TimePeriod(new TimeSpan(0, 17, 0, 0, 1), new TimeSpan(0, 23, 59, 59, 999));
            exchanges.Add("TSX-V", tsxv);

            cse.PreMarketHours = new TimePeriod(new TimeSpan(0, 0, 0, 0, 0), new TimeSpan(0, 9, 29, 59, 999));
            cse.MarketHours = new TimePeriod(new TimeSpan(9, 30, 0), new TimeSpan(16, 0, 0));
            cse.AfterMarketHours = new TimePeriod(new TimeSpan(0, 16, 0, 0, 1), new TimeSpan(0, 23, 59, 59, 999));
            exchanges.Add("CSE", cse);

            neo.PreMarketHours = new TimePeriod(new TimeSpan(0, 0, 0, 0, 0), new TimeSpan(0, 7, 59, 59, 999));
            neo.MarketHours = new TimePeriod(new TimeSpan(8, 0, 0), new TimeSpan(17, 0, 0));
            neo.AfterMarketHours = new TimePeriod(new TimeSpan(0, 17, 0, 0, 1), new TimeSpan(0, 23, 59, 59, 999));
            exchanges.Add("NEO", neo);

            mx.PreMarketHours = new TimePeriod(new TimeSpan(0, 0, 0, 0, 0), new TimeSpan(0, 9, 29, 59, 999));
            mx.MarketHours = new TimePeriod(new TimeSpan(9, 30, 0), new TimeSpan(16, 15, 0));
            mx.AfterMarketHours = new TimePeriod(new TimeSpan(0, 16, 15, 0, 1), new TimeSpan(0, 23, 59, 59, 999));
            exchanges.Add("MX", mx);

            nyse.PreMarketHours = new TimePeriod(new TimeSpan(0, 0, 0, 0, 0), new TimeSpan(0, 9, 29, 59, 999));
            nyse.MarketHours = new TimePeriod(new TimeSpan(9, 30, 0), new TimeSpan(16, 0, 0));
            nyse.AfterMarketHours = new TimePeriod(new TimeSpan(0, 16, 0, 0, 1), new TimeSpan(0, 23, 59, 59, 999));
            exchanges.Add("NYSE", nyse);

            amex.PreMarketHours = new TimePeriod(new TimeSpan(0, 0, 0, 0, 0), new TimeSpan(0, 9, 29, 59, 999));
            amex.MarketHours = new TimePeriod(new TimeSpan(9, 30, 0), new TimeSpan(16, 0, 0));
            amex.AfterMarketHours = new TimePeriod(new TimeSpan(0, 16, 0, 0, 1), new TimeSpan(0, 23, 59, 59, 999));
            exchanges.Add("AMEX", amex);

            nasdaq.PreMarketHours = new TimePeriod(new TimeSpan(0, 0, 0, 0, 0), new TimeSpan(0, 9, 29, 59, 999));
            nasdaq.MarketHours = new TimePeriod(new TimeSpan(9, 30, 0), new TimeSpan(16, 0, 0));
            nasdaq.AfterMarketHours = new TimePeriod(new TimeSpan(0, 16, 0, 0, 1), new TimeSpan(0, 23, 59, 59, 999));
            exchanges.Add("NASDAQ", nasdaq);

            opra.MarketHours = new TimePeriod(new TimeSpan(0, 0, 0), new TimeSpan(0, 23, 59, 59, 999));
            opra.HasNonMarketHours = false;
            exchanges.Add("OPRA", opra);
        }

        public string Name
        {
            get;
            protected internal set;
        }

        public TimePeriod PreMarketHours
        {
            get;
            protected internal set;
        }

        public TimePeriod MarketHours
        {
            get;
            protected internal set;
        }

        public TimePeriod ExtendedHours
        {
            get;
            protected set;
        }

        public TimePeriod AfterMarketHours
        {
            get;
            protected internal set;
        }

        public bool HasNonMarketHours
        {
            get;
            protected internal set;
        }

        /*public Exchange(string name, TimePeriod marketHours)
        {
            Name = name;
            MarketHours = marketHours;
        }*/

        public Exchange(string name)
        {
            Name = name;
            HasNonMarketHours = true;
        }
    }
}
