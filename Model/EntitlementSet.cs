using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Iress.Canada.Reporting.Reports.Other.MarketDataUsageReport.Model
{
    using EntitlementChar = Tuple<char, int>;

    public class EntitlementSet
    {
        public static Dictionary<EntitlementChar, EntitlementSet> EntitlementSets
        { 
            get;
            private set;
        }

        /*public static Dictionary<string, EntitlementSet> entitlementSetsUS
        {
            get;
            private set;
        }*/

        static EntitlementSet()
        {
            EntitlementSets = new Dictionary<EntitlementChar, EntitlementSet>();
            //entitlementSetsUS = new Dictionary<char, EntitlementSet>();

            #region Canada entitlement sets - first character in entitlement string

            Entitlement.QuoteTiming realTime = Entitlement.QuoteTiming.RealTime;
            Entitlement.QuoteTiming delayed = Entitlement.QuoteTiming.Delayed;

            Entitlement.SecurityType equities = Entitlement.SecurityType.Equities;
            Entitlement.SecurityType options = Entitlement.SecurityType.Options;
            Entitlement.SecurityType indices = Entitlement.SecurityType.Indices;

            //First character is A
            EntitlementSet toAdd = new EntitlementSet();
            toAdd.Entitlements.Add(new DataEntitlement("TSX L1", delayed, equities));
            toAdd.Entitlements.Add(new DataEntitlement("TSX-V L1", delayed, equities));
            toAdd.Entitlements.Add(new DataEntitlement("CSE L1", delayed, equities));
            toAdd.Entitlements.Add(new DataEntitlement("NEO Primary Listing L1", delayed, equities));
            toAdd.Entitlements.Add(new DataEntitlement("MX L1", delayed, options));
            toAdd.Entitlements.Add(new DataEntitlement("TMX Indices", delayed, indices));

            EntitlementSets.Add(new EntitlementChar('A', 0), toAdd);

            //First character is B
            toAdd = new EntitlementSet();
            toAdd.Entitlements.Add(new DataEntitlement("TSX L1", realTime, equities));
            toAdd.Entitlements.Add(new DataEntitlement("TSX-V L1", delayed, equities));
            toAdd.Entitlements.Add(new DataEntitlement("CSE L1", realTime, equities));
            toAdd.Entitlements.Add(new DataEntitlement("NEO Primary Listing L1", realTime, equities));
            toAdd.Entitlements.Add(new DataEntitlement("TMX Indices", realTime, indices));

            EntitlementSets.Add(new EntitlementChar('B', 0), toAdd);

            //First character is C
            toAdd = new EntitlementSet();
            toAdd.Entitlements.Add(new DataEntitlement("TSX L1", realTime, equities));
            toAdd.Entitlements.Add(new DataEntitlement("TSX L2 MBP", realTime, equities));
            toAdd.Entitlements.Add(new DataEntitlement("TSX-V L1", delayed, equities));
            toAdd.Entitlements.Add(new DataEntitlement("CSE L1", realTime, equities));
            toAdd.Entitlements.Add(new DataEntitlement("NEO Primary Listing L1", realTime, equities));
            toAdd.Entitlements.Add(new DataEntitlement("TMX Indices", realTime, indices));

            EntitlementSets.Add(new EntitlementChar('C', 0), toAdd);

            //First character is D
            toAdd = new EntitlementSet();
            toAdd.Entitlements.Add(new DataEntitlement("TSX L1", realTime, equities));
            toAdd.Entitlements.Add(new DataEntitlement("TSX-V L1", realTime, equities));
            toAdd.Entitlements.Add(new DataEntitlement("CSE L1", realTime, equities));
            toAdd.Entitlements.Add(new DataEntitlement("NEO Primary Listing L1", realTime, equities));
            toAdd.Entitlements.Add(new DataEntitlement("TMX Indices", realTime, indices));

            EntitlementSets.Add(new EntitlementChar('D', 0), toAdd);

            //First character is E
            toAdd = new EntitlementSet();
            toAdd.Entitlements.Add(new DataEntitlement("TSX L1", realTime, equities));
            toAdd.Entitlements.Add(new DataEntitlement("TSX L2 MBP", realTime, equities));
            toAdd.Entitlements.Add(new DataEntitlement("TSX-V L1", realTime, equities));
            toAdd.Entitlements.Add(new DataEntitlement("TSX-V L2 MBP", realTime, equities));
            toAdd.Entitlements.Add(new DataEntitlement("CSE L1", realTime, equities));
            toAdd.Entitlements.Add(new DataEntitlement("NEO Primary Listing L1", realTime, equities));
            toAdd.Entitlements.Add(new DataEntitlement("TMX Indices", realTime, indices));

            EntitlementSets.Add(new EntitlementChar('E', 0), toAdd);

            //First character is F
            toAdd = new EntitlementSet();
            toAdd.Entitlements.Add(new DataEntitlement("TSX + TSX L2 MBP", equities));
            toAdd.Entitlements.Add(new DataEntitlement("TSX-V", equities));
            toAdd.Entitlements.Add(new DataEntitlement("CSE", equities));
            toAdd.Entitlements.Add(new DataEntitlement("NEO Primary Listing", equities));
            toAdd.Entitlements.Add(new DataEntitlement("TMX Indices", realTime, indices));

            EntitlementSets.Add(new EntitlementChar('F', 0), toAdd);

            //First character is G
            toAdd = new EntitlementSet();
            toAdd.Entitlements.Add(new DataEntitlement("TSX + TSX L2 MBK", equities));
            toAdd.Entitlements.Add(new DataEntitlement("TSX-V", equities));
            toAdd.Entitlements.Add(new DataEntitlement("CSE", equities));
            toAdd.Entitlements.Add(new DataEntitlement("NEO Primary Listing", equities));
            toAdd.Entitlements.Add(new DataEntitlement("TMX Indices", realTime, indices));

            EntitlementSets.Add(new EntitlementChar('G', 0), toAdd);

            //First character is H
            toAdd = new EntitlementSet();
            toAdd.Entitlements.Add(new DataEntitlement("TSX + TSX L2 MBK", equities));
            toAdd.Entitlements.Add(new DataEntitlement("TSX-V + TSX-V L2 MBK", equities));
            toAdd.Entitlements.Add(new DataEntitlement("CSE", equities));
            toAdd.Entitlements.Add(new DataEntitlement("NEO Primary Listing", equities));
            toAdd.Entitlements.Add(new DataEntitlement("TMX Indices", realTime, indices));

            EntitlementSets.Add(new EntitlementChar('H', 0), toAdd);

            //First character is I
            toAdd = new EntitlementSet();
            toAdd.Entitlements.Add(new DataEntitlement("TSX", equities));
            toAdd.Entitlements.Add(new DataEntitlement("CSE", equities));
            toAdd.Entitlements.Add(new DataEntitlement("NEO Primary Listing", equities));
            toAdd.Entitlements.Add(new DataEntitlement("MX", options));
            toAdd.Entitlements.Add(new DataEntitlement("TMX Indices", realTime, indices));

            EntitlementSets.Add(new EntitlementChar('I', 0), toAdd);

            //First character is J
            toAdd = new EntitlementSet();
            toAdd.Entitlements.Add(new DataEntitlement("TSX L1", realTime, equities));
            toAdd.Entitlements.Add(new DataEntitlement("TSX L2 MBP", realTime, equities));
            toAdd.Entitlements.Add(new DataEntitlement("TSX-V L1", delayed, equities));
            toAdd.Entitlements.Add(new DataEntitlement("CSE L1", realTime, equities));
            toAdd.Entitlements.Add(new DataEntitlement("NEO Primary Listing L1", realTime, equities));
            toAdd.Entitlements.Add(new DataEntitlement("MX L1", realTime, options));
            toAdd.Entitlements.Add(new DataEntitlement("TMX Indices", realTime, indices));

            EntitlementSets.Add(new EntitlementChar('J', 0), toAdd);

            //First character is K
            toAdd = new EntitlementSet();
            toAdd.Entitlements.Add(new DataEntitlement("TSX", equities));
            toAdd.Entitlements.Add(new DataEntitlement("TSX-V", equities));
            toAdd.Entitlements.Add(new DataEntitlement("CSE", equities));
            toAdd.Entitlements.Add(new DataEntitlement("NEO Primary Listing", equities));
            toAdd.Entitlements.Add(new DataEntitlement("MX", options));
            toAdd.Entitlements.Add(new DataEntitlement("TMX Indices", realTime, indices));

            EntitlementSets.Add(new EntitlementChar('K', 0), toAdd); 

            //First character is L
            toAdd = new EntitlementSet();
            toAdd.Entitlements.Add(new DataEntitlement("TSX + TSX L2 MBP", equities));
            toAdd.Entitlements.Add(new DataEntitlement("TSX-V", equities));
            toAdd.Entitlements.Add(new DataEntitlement("CSE", equities));
            toAdd.Entitlements.Add(new DataEntitlement("NEO Primary Listing", equities));
            toAdd.Entitlements.Add(new DataEntitlement("MX", options));
            toAdd.Entitlements.Add(new DataEntitlement("TMX Indices", realTime, indices));

            EntitlementSets.Add(new EntitlementChar('L', 0), toAdd); 

            //First character is M
            toAdd = new EntitlementSet();
            toAdd.Entitlements.Add(new DataEntitlement("TSX L1", realTime, equities));
            toAdd.Entitlements.Add(new DataEntitlement("TSX L2 MBP", realTime, equities));
            toAdd.Entitlements.Add(new DataEntitlement("TSX-V L1", realTime, equities));
            toAdd.Entitlements.Add(new DataEntitlement("TSX-V L2 MBP", realTime, equities));
            toAdd.Entitlements.Add(new DataEntitlement("CSE L1", realTime, equities));
            toAdd.Entitlements.Add(new DataEntitlement("NEO Primary Listing L1", realTime, equities));
            toAdd.Entitlements.Add(new DataEntitlement("MX L1", realTime, options));
            toAdd.Entitlements.Add(new DataEntitlement("TMX Indices", realTime, indices));

            EntitlementSets.Add(new EntitlementChar('M', 0), toAdd);

            //First character is N
            toAdd = new EntitlementSet();
            toAdd.Entitlements.Add(new DataEntitlement("TSX + TSX L2 MBK", equities));
            toAdd.Entitlements.Add(new DataEntitlement("TSX-V + TSX-V L2 MBK", equities));
            toAdd.Entitlements.Add(new DataEntitlement("CSE", equities));
            toAdd.Entitlements.Add(new DataEntitlement("NEO Primary Listing", equities));
            toAdd.Entitlements.Add(new DataEntitlement("MX", options));
            toAdd.Entitlements.Add(new DataEntitlement("TMX Indices", realTime, indices));
            toAdd.Entitlements.Add(new DataEntitlement("TSX-V Indices", realTime, indices));

            EntitlementSets.Add(new EntitlementChar('N', 0), toAdd);

            //First character is O
            toAdd = new EntitlementSet();
            toAdd.Entitlements.Add(new DataEntitlement("TSX L1", delayed, equities));
            toAdd.Entitlements.Add(new DataEntitlement("TSX-V L1", delayed, equities));
            toAdd.Entitlements.Add(new DataEntitlement("CSE L1", delayed, equities));
            toAdd.Entitlements.Add(new DataEntitlement("NEO Primary Listing L1", delayed, equities));
            toAdd.Entitlements.Add(new DataEntitlement("TMX Indices", delayed, indices));

            EntitlementSets.Add(new EntitlementChar('O', 0), toAdd); 

            #endregion

            #region US entitlement sets - second character in entitlement string
            //Second character is A
            toAdd = new EntitlementSet();
            toAdd.Entitlements.Add(new DataEntitlement("NYSE L1", delayed, equities));
            toAdd.Entitlements.Add(new DataEntitlement("AMEX L1", delayed, equities));
            toAdd.Entitlements.Add(new DataEntitlement("NASDAQ L1", delayed, equities));
            toAdd.Entitlements.Add(new DataEntitlement("OPRA L1", delayed, options));
            toAdd.Entitlements.Add(new DataEntitlement("S&P DJ Indices", delayed, indices));
            toAdd.Entitlements.Add(new DataEntitlement("NASDAQ Indices", delayed, indices));
            toAdd.Entitlements.Add(new DataEntitlement("MDX Indices", delayed, indices));

            EntitlementSets.Add(new EntitlementChar('A', 1), toAdd);

            //Second character is B
            toAdd = new EntitlementSet();
            toAdd.Entitlements.Add(new DataEntitlement("NYSE L1", realTime, equities));
            toAdd.Entitlements.Add(new DataEntitlement("AMEX L1", realTime, equities));
            toAdd.Entitlements.Add(new DataEntitlement("NASDAQ L1", realTime, equities));
            toAdd.Entitlements.Add(new DataEntitlement("S&P DJ Indices", realTime, indices));
            toAdd.Entitlements.Add(new DataEntitlement("NASDAQ Indices", realTime, indices));
            toAdd.Entitlements.Add(new DataEntitlement("MDX Indices", realTime, indices));

            EntitlementSets.Add(new EntitlementChar('B', 1), toAdd);

            //Second character is C
            toAdd = new EntitlementSet();
            toAdd.Entitlements.Add(new DataEntitlement("NYSE L1", realTime, equities));
            toAdd.Entitlements.Add(new DataEntitlement("AMEX L1", realTime, equities));
            toAdd.Entitlements.Add(new DataEntitlement("NASDAQ L1", realTime, equities));
            toAdd.Entitlements.Add(new DataEntitlement("NASDAQ L2 OB", realTime, equities));
            toAdd.Entitlements.Add(new DataEntitlement("S&P DJ Indices", realTime, indices));
            toAdd.Entitlements.Add(new DataEntitlement("NASDAQ Indices", realTime, indices));
            toAdd.Entitlements.Add(new DataEntitlement("MDX Indices", realTime, indices));

            EntitlementSets.Add(new EntitlementChar('C', 1), toAdd);

            //Second character is D
            toAdd = new EntitlementSet();
            toAdd.Entitlements.Add(new DataEntitlement("NYSE", equities));
            toAdd.Entitlements.Add(new DataEntitlement("AMEX", equities));
            toAdd.Entitlements.Add(new DataEntitlement("NASDAQ + NASDAQ L2 TV", equities));
            toAdd.Entitlements.Add(new DataEntitlement("S&P DJ Indices", realTime, indices));
            toAdd.Entitlements.Add(new DataEntitlement("NASDAQ Indices", realTime, indices));
            toAdd.Entitlements.Add(new DataEntitlement("MDX Indices", realTime, indices));

            EntitlementSets.Add(new EntitlementChar('D', 1), toAdd);

            //Second character is E
            toAdd = new EntitlementSet();
            toAdd.Entitlements.Add(new DataEntitlement("NYSE", equities));
            toAdd.Entitlements.Add(new DataEntitlement("AMEX", equities));
            toAdd.Entitlements.Add(new DataEntitlement("NASDAQ", equities));
            toAdd.Entitlements.Add(new DataEntitlement("OPRA", options));
            toAdd.Entitlements.Add(new DataEntitlement("S&P DJ Indices", realTime, indices));
            toAdd.Entitlements.Add(new DataEntitlement("NASDAQ Indices", realTime, indices));
            toAdd.Entitlements.Add(new DataEntitlement("MDX Indices", realTime, indices));

            EntitlementSets.Add(new EntitlementChar('E', 1), toAdd);

            //Second character is F
            toAdd = new EntitlementSet();
            toAdd.Entitlements.Add(new DataEntitlement("NYSE L1", realTime, equities));
            toAdd.Entitlements.Add(new DataEntitlement("AMEX L1", realTime, equities));
            toAdd.Entitlements.Add(new DataEntitlement("NASDAQ L1", realTime, equities));
            toAdd.Entitlements.Add(new DataEntitlement("NASDAQ L2 OB", realTime, equities));
            toAdd.Entitlements.Add(new DataEntitlement("OPRA L1", realTime, options));
            toAdd.Entitlements.Add(new DataEntitlement("S&P DJ Indices", realTime, indices));
            toAdd.Entitlements.Add(new DataEntitlement("NASDAQ Indices", realTime, indices));
            toAdd.Entitlements.Add(new DataEntitlement("MDX Indices", realTime, indices));

            EntitlementSets.Add(new EntitlementChar('F', 1), toAdd);

            //Second character is G
            toAdd = new EntitlementSet();
            toAdd.Entitlements.Add(new DataEntitlement("NYSE", equities));
            toAdd.Entitlements.Add(new DataEntitlement("AMEX", equities));
            toAdd.Entitlements.Add(new DataEntitlement("NASDAQ + NASDAQ L2 TV", equities));
            toAdd.Entitlements.Add(new DataEntitlement("OPRA", options));
            toAdd.Entitlements.Add(new DataEntitlement("S&P DJ Indices", realTime, indices));
            toAdd.Entitlements.Add(new DataEntitlement("NASDAQ Indices", realTime, indices));
            toAdd.Entitlements.Add(new DataEntitlement("MDX Indices", realTime, indices));

            EntitlementSets.Add(new EntitlementChar('G', 1), toAdd);

            //Second character is H
            toAdd = new EntitlementSet();
            toAdd.Entitlements.Add(new DataEntitlement("NYSE L1", delayed, equities));
            toAdd.Entitlements.Add(new DataEntitlement("AMEX L1", delayed, equities));
            toAdd.Entitlements.Add(new DataEntitlement("NASDAQ L1", delayed, equities));
            toAdd.Entitlements.Add(new DataEntitlement("S&P DJ Indices", delayed, indices));
            toAdd.Entitlements.Add(new DataEntitlement("NASDAQ Indices", delayed, indices));
            toAdd.Entitlements.Add(new DataEntitlement("MDX Indices", delayed, indices));

            EntitlementSets.Add(new EntitlementChar('H', 1), toAdd);

            //Second character is I
            toAdd = new EntitlementSet();
            toAdd.Entitlements.Add(new DataEntitlement("NASDAQ Basic L1", delayed, equities));
            toAdd.Entitlements.Add(new DataEntitlement("NASDAQ L1", delayed, equities));
            toAdd.Entitlements.Add(new DataEntitlement("OPRA L1", delayed, options));
            toAdd.Entitlements.Add(new DataEntitlement("S&P DJ Indices", delayed, indices));
            toAdd.Entitlements.Add(new DataEntitlement("NASDAQ Indices", delayed, indices));
            toAdd.Entitlements.Add(new DataEntitlement("MDX Indices", delayed, indices));

            EntitlementSets.Add(new EntitlementChar('I', 1), toAdd);

            //Second character is J
            toAdd = new EntitlementSet();
            toAdd.Entitlements.Add(new DataEntitlement("NASDAQ Basic L1", realTime, equities));
            toAdd.Entitlements.Add(new DataEntitlement("NASDAQ L1", realTime, equities));
            toAdd.Entitlements.Add(new DataEntitlement("S&P DJ Indices", realTime, indices));
            toAdd.Entitlements.Add(new DataEntitlement("NASDAQ Indices", realTime, indices));
            toAdd.Entitlements.Add(new DataEntitlement("MDX Indices", realTime, indices));

            EntitlementSets.Add(new EntitlementChar('J', 1), toAdd);

            //Second character is K
            toAdd = new EntitlementSet();
            toAdd.Entitlements.Add(new DataEntitlement("NASDAQ Basic L1", realTime, equities));
            toAdd.Entitlements.Add(new DataEntitlement("NASDAQ L1", realTime, equities));
            toAdd.Entitlements.Add(new DataEntitlement("NASDAQ L2 OB", realTime, equities));
            toAdd.Entitlements.Add(new DataEntitlement("S&P DJ Indices", realTime, indices));
            toAdd.Entitlements.Add(new DataEntitlement("NASDAQ Indices", realTime, indices));
            toAdd.Entitlements.Add(new DataEntitlement("MDX Indices", realTime, indices));

            EntitlementSets.Add(new EntitlementChar('K', 1), toAdd);

            //Second character is L
            toAdd = new EntitlementSet();
            toAdd.Entitlements.Add(new DataEntitlement("NASDAQ Basic L1", realTime, equities));
            toAdd.Entitlements.Add(new DataEntitlement("NASDAQ L1", realTime, equities));
            toAdd.Entitlements.Add(new DataEntitlement("NASDAQ L2 OB", realTime, equities));
            toAdd.Entitlements.Add(new DataEntitlement("OPRA L1", realTime, options));
            toAdd.Entitlements.Add(new DataEntitlement("S&P DJ Indices", realTime, indices));
            toAdd.Entitlements.Add(new DataEntitlement("NASDAQ Indices", realTime, indices));
            toAdd.Entitlements.Add(new DataEntitlement("MDX Indices", realTime, indices));

            EntitlementSets.Add(new EntitlementChar('L', 1), toAdd);
            #endregion

            #region Video player entitlement sets - third character in entitlement string

            Entitlement.SecurityType vid = Entitlement.SecurityType.NonSecurity;

            //Third character is A
            toAdd = new EntitlementSet();
            toAdd.Entitlements.Add(new DataEntitlement("Globe and Mail News", vid));

            EntitlementSets.Add(new EntitlementChar('A', 2), toAdd);

            //Third character is B
            toAdd = new EntitlementSet();
            toAdd.Entitlements.Add(new DataEntitlement("Globe and Mail News", vid));
            toAdd.Entitlements.Add(new DataEntitlement("BNN Video", vid));

            EntitlementSets.Add(new EntitlementChar('B', 2), toAdd);

            #endregion
        }

        public List<DataEntitlement> Entitlements
        {
            get;
            protected internal set;
        }

        public EntitlementSet()
        {
            Entitlements = new List<DataEntitlement>();
        }
    }
}
