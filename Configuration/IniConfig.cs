using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iress.Canada.Reporting.Configuration;
using Iress.Canada.Reporting.Logging;
using Iress.Canada.Reporting.Data;
using System.Data.SqlClient;

namespace Iress.Canada.Reporting.MarketDataUsageReport.Configuration
{
    internal class IniConfig
    {
        internal IniFile Ini
        {
            get; set;
        }

        private WeeklyRollLog Log
        {
            get; set;
        }

        internal string FilePath
        {
            get
            {
                return Ini.Path;
            }
        }

        internal IniConfig(IniFile ini, WeeklyRollLog log)
        {
            Ini = ini;
            Log = log;
        }

        internal bool ReadIniCompanies(ref List<int> companyIds)
        {
            string companyString = "";

            if (Ini.TryGetValue<string>("CompanyID", "CompanyIDs", ref companyString))
            {
                try
                {
                    string[] idStrings = companyString.Split(',');

                    foreach (string id in idStrings)
                        companyIds.Add(Int32.Parse(id));

                    return true;
                }
                catch (Exception e)
                {
                    Log.Fatal("Error reading company IDs from INI file");
                    return false;
                }
            }
            else
                return false;
        }

        internal bool ReadIniGroupExcludes(ref List<int> groupsToExclude)
        {
            string groupExcludesString = "";

            if (Ini.TryGetValue<string>("Excludes", "ExcludeGroupIds", ref groupExcludesString))
            {
                try
                {
                    string[] groupExcludeStrings = groupExcludesString.Split(',');

                    foreach (string id in groupExcludeStrings)
                    {
                        groupsToExclude.Add(Int32.Parse(id));
                    }

                    return true;
                }
                catch (Exception e)
                {
                    Log.Fatal("Error reading group exclusion IDs from INI file");
                    Log.Fatal("Exception:" + Environment.NewLine + e.Message);
                    Log.Fatal("Stack Trace:" + Environment.NewLine + e.StackTrace);
                    return false;
                }
            }
            else
                return false;
        }

        internal bool ReadIniUserExcludes(ref List<int> usersToExclude)
        {
            string userExcludesString = "";

            if (Ini.TryGetValue<string>("Excludes", "ExcludeUserIds", ref userExcludesString))
            {
                try
                {
                    string[] userExcludeStrings = userExcludesString.Split(',');

                    foreach (string id in userExcludeStrings)
                    {
                        usersToExclude.Add(Int32.Parse(id));
                    }

                    return true;
                }
                catch (Exception e)
                {
                    Log.Fatal("Error reading user exclusion IDs from INI file");
                    Log.Fatal("Exception:" + Environment.NewLine + e.Message);
                    Log.Fatal("Stack Trace:" + Environment.NewLine + e.StackTrace);
                    return false;
                }
            }
            else
                return false;
        }

        internal bool ReadIniReportDates(string AppName, ref DateTime reportStart, ref DateTime reportEnd)
        {
            bool configured = false;
            DateTime start = DateTime.Now, end = DateTime.Now, now = DateTime.Now;

            if (Ini.TryGetValue<DateTime>(AppName, "StartDate", ref start) && Ini.TryGetValue<DateTime>(AppName, "EndDate", ref end))
            {
                reportStart = start;
                reportEnd = end.AddDays(1).AddMilliseconds(-1);

                return true;
            }

            return configured;
        }

        internal bool ReadIniDataSources(ref SqlConnectionManager connectionManager)
        {
            try
            {
                string cadbStr = "", clidataStr = "";
                if (Ini.TryGetConnectionStringFromSection("CADB", out cadbStr))
                    connectionManager.AddConnection("cadb", new SqlConnection(cadbStr));
                if (Ini.TryGetConnectionStringFromSection("Clidata", out clidataStr))
                    connectionManager.AddConnection("clidata", new SqlConnection(clidataStr));
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}
