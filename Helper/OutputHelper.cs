using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Iress.Canada.Reporting.Reports.Other.MarketDataUsageReport.Helper
{
    public static class OutputHelper
    {
        public static string CsvFieldFormat(string str, bool isFirstField = false)
        {
            StringBuilder ret = new StringBuilder();

            if (!isFirstField)
                ret.Append(",");

            if (String.IsNullOrEmpty(str))
                return ret.ToString();
            else
            {
                if (str.Contains(','))
                    ret.AppendFormat("\"{0}\"", str);
                else
                    ret.Append(str);
            }

            return ret.ToString();
        }

        public static string CsvFieldFormat(object obj, bool isFirstField = false)
        {
            return OutputHelper.CsvFieldFormat(obj.ToString(), isFirstField);
        }
    }
}
