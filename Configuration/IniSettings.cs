using Iress.Canada.Reporting.Configuration;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;

namespace Iress.Canada.Reporting.MarketDataUsageReport.Configuration
{
    internal class IniSettings
    {
        internal ExpandoObject MarketDataUsageReport { get; set; }
        internal IniDbSection CADB { get; set; }
        internal IniDbSection Clidata { get; set; }
        internal ExpandoObject CompanyID { get; set; }
        internal ExpandoObject Excludes { get; set; }
        internal ExpandoObject Output { get; set; }

        internal IniSettings()
        {
            DateTime now = DateTime.Now;

            dynamic mainProto = new ExpandoObject();

            mainProto.ReportStartDate = now.Date.AddDays((-1 * now.Date.Day) + 1).AddMonths(-1);
            mainProto.ReportEndDate = now.Date.AddDays((-1 * now.Date.Day) + 1);

            //if (now.Day == 1)
            //{
            //    mainProto.ReportStartDate = now.Date.AddMonths(-1);
            //    mainProto.ReportEndDate = now.Date.AddMilliseconds(-1);
            //}
            //else
            //{
            //    mainProto.ReportStartDate = now.Date.AddDays((-1 * now.Date.Day) + 1).AddMonths(-1);
            //    mainProto.ReportEndDate = now.Date.AddMilliseconds(-1);
            //}

            mainProto.ShowZeroLoginCountUsers = false;
            MarketDataUsageReport = mainProto;

            CADB = new IniDbSection();
            CADB.MultipleActiveResultSets = true;

            Clidata = new IniDbSection();
            Clidata.MultipleActiveResultSets = true;

            dynamic companyId = new ExpandoObject();
            companyId.CompanyIDs = new int[0];
            CompanyID = companyId;

            dynamic excludes = new ExpandoObject();
            excludes.ExcludeGroupIDs = new int[0];
            excludes.ExcludeUserIDs = new int[0];
            Excludes = excludes;

            dynamic output = new ExpandoObject();
            output.OutputFilename = String.Format("IRESSUsageReport_{0}.csv", mainProto.ReportEndDate.ToString("yyyyMMdd"));
            output.VideoOnlySuffix = "VideoOnly";
            output.BnnOnlySuffix = "BNNOnly";
            Output = output;
        }
    }
}
