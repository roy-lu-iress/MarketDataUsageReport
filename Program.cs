using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Iress.Canada.Reporting.Reports.Other.MarketDataUsageReport.Model;
using System.Data.SqlClient;
using System.IO;
using Iress.Canada.Reporting.Reports.Other.MarketDataUsageReport.Helper;
using Iress.Canada.Reporting.Reports.Other.MarketDataUsageReport.Data;
using Iress.Canada.Reporting;
using Iress.Canada.Reporting.Configuration;
using Iress.Canada.Reporting.Logging;
using Iress.Canada.Reporting.Data;
using Iress.Canada.Reporting.MarketDataUsageReport.Configuration;

namespace Iress.Canada.Reporting.Reports.Other.MarketDataUsageReport
{
    using EntitlementChar = Tuple<char, int>;
    using System.Dynamic;
    using log4net;

    class Program
    {
        public static string AppName = "MarketDataUsageReport";

        private static IniConfig ini;

        private static ILog _log;
        //private static WeeklyRollLog log = new WeeklyRollLog("MarketDataUsageReport.log");

        private static readonly string SQL_DATETIME_FORMAT = "yyyy-MM-dd HH:mm:ss.fff";

        private static SqlConnectionManager connectionManager = new SqlConnectionManager();

        private static DateTime reportStart;
        private static DateTime reportEnd;
        private static DateTime reportDate;

        private static Dictionary<string, string> connectionStrings = new Dictionary<string, string>();

        private static List<int> companyIds = new List<int>();

        private static int VIDEO_STREAM_CHAR_INDEX = 2;

        private static string OutputFilename = "IRESSUsageReport_{0}{1}.csv";

        private static string VideoStreamsOnlyCommandLineFlag = @"-VidOnly";
        private static bool VideoStreamsOnly = false;
        private static string VideoStreamsOnlyFileSuffix = "_VideoOnly";

        private static string BnnOnlyCommandLineFlag = @"-BNNOnly";
        private static bool BnnOnly = false;
        private static string BnnOnlyFileSuffix = "_BNNOnly";

        private static bool MonthEndOnly = false;

        private static string CollateEntitlementCommandLineFlag = @"-CollateByEntitlement";
        private static bool CollateEntitlementFlag = false;

        private static string CollateUserCommandLineFlag = @"-CollateByUser";

        private static bool ShowZeroLoginCountUsers = false;

        private static List<int> ExcludeGroupIds = new List<int>();
        private static List<int> ExcludeUserIds = new List<int>();

        private static HashSet<User> allUsers = new HashSet<User>();
        private static HashSet<User> usersToReport = new HashSet<User>();
        private static HashSet<int> allUserIds = new HashSet<int>();

        private static Dictionary<int, List<AuditEntry<User>>> userAuditHistories = new Dictionary<int, List<AuditEntry<User>>>();
        private static Dictionary<int, List<AuditEntry<Group>>> groupAuditHistories = new Dictionary<int, List<AuditEntry<Group>>>();

        private static Dictionary<int, List<Login>> loginsByUserId = new Dictionary<int, List<Login>>();

        private static DateTimePeriod reportPeriod;

        private static dynamic Settings;

        static void Main(string[] args)
        {
            try
            {
                _log = LogManager.GetLogger(typeof(Program));
                _log.Info("Logger opened for logging");
            }
            catch(Exception ex)
            {
                Console.WriteLine(String.Format("FATAL ERROR - failed to initialize application log with message: {0}. Press enter to exit", ex.Message));
                Console.ReadKey();
            }

            try
            {
                ParseCommandLineArgs(args);
            }
            catch(Exception ex)
            {
                string msg = String.Format("{0} occurred parsing command line arguments with message: {1}", ex.GetType(), ex.Message);
                _log.Fatal(msg);
            }



            try
            {
                var configRes = Configure();
                if (configRes.IsFailure())
                {
                    _log.Fatal(String.Format("Error occurred reading INI config with message: {0}", configRes.AsFailure().Error.Message));
                    return;
                }
            }
            catch(Exception ex)
            {
                string msg = String.Format("{0} occurred reading INI configuration: {1}", ex.GetType(), ex.Message);
                _log.Fatal(msg);
            }



            try
            {
                RunReport();
            }
            catch(Exception ex)
            {
                string msg = String.Format("{0} occurred during main report process: {1}", ex.GetType(), ex.Message);
                _log.Fatal(msg);
            }


        }

        #region Command line parsing

        private static void ParseCommandLineArgs(string[] args)
        {
            foreach(string arg in args)
            {
                /*if (arg == CollateEntitlementCommandLineFlag)
                    CollateEntitlementFlag = true;
                else if (arg == CollateUserCommandLineFlag && !VideoStreamsOnly)
                    CollateEntitlementFlag = false;
                else*/ 
                if (arg.Equals(BnnOnlyCommandLineFlag, StringComparison.InvariantCultureIgnoreCase))
                {
                    BnnOnly = true;
                    CollateEntitlementFlag = true;
                }
                else if (arg.Equals(VideoStreamsOnlyCommandLineFlag, StringComparison.InvariantCultureIgnoreCase))
                {
                    VideoStreamsOnly = true;
                    CollateEntitlementFlag = true;
                }
            }
        }

        #endregion

        #region INI configuration methods

        private static IResult<bool> Configure()
        {
            Ini ini = new Ini("MarketDataUsageReport.ini");
            reportDate = DateTime.Now.Date;

            var settingsRes = ini.FillSettings(new IniSettings());
            if (settingsRes.IsSuccess())
            {
                Settings = (IniSettings)settingsRes.AsSuccess().Result;
                IniSettings settings = Settings;

                reportStart = Settings.MarketDataUsageReport.ReportStartDate;
                reportEnd = Settings.MarketDataUsageReport.ReportEndDate;

                connectionManager.AddConnection("cadb", new SqlConnection(settings.CADB.GetConnectionString()));
                connectionManager.AddConnection("clidata", new SqlConnection(settings.Clidata.GetConnectionString()));

                companyIds = new List<int>(Settings.CompanyID.CompanyIDs);

                ExcludeGroupIds = new List<int>(Settings.Excludes.ExcludeGroupIDs);
                ExcludeUserIds = new List<int>(Settings.Excludes.ExcludeUserIDs);

                ShowZeroLoginCountUsers = Settings.MarketDataUsageReport.ShowZeroLoginCountUsers;

                OutputFilename = Settings.Output.OutputFilename;

                BnnOnlyFileSuffix = Settings.Output.BnnOnlySuffix;
                VideoStreamsOnlyFileSuffix = Settings.Output.VideoOnlySuffix;

                return true.Success();
            }
            else
                return new Failure<bool>(settingsRes.AsFailure().Error);
        }

        private static bool ConfigureOld()
        {
            throw new NotImplementedException();
            /*
            ini = new IniConfig(new IniFile("MarketDataUsageReport.ini"), _log);
            _log.Info("Reading configuration from INI");
            _log.Info("Ini file: " + ini.FilePath);
            DateTime now = DateTime.Now;

            reportDate = now.Date;

            if (!ini.ReadIniReportDates(AppName, ref reportStart, ref reportEnd))
            {
                if (now.Day == 1)
                {
                    _log.Info("First day of month. Report will be run for the full preceding month.");
                    reportStart = now.Date.AddMonths(-1);
                    reportEnd = now.Date.AddMilliseconds(-1);
                }
                else
                {
                    _log.Info("Report will be run for month to date (EOD of last full day of month)");

                    reportStart = new DateTime(now.Year, now.Month, 1);
                    reportEnd = now.Date.AddMilliseconds(-1);
                }
            }
            else
                _log.Info(String.Format("Report period specified in INI. Report will run for period from {0} to {1}", reportStart, reportEnd));

            if (!ini.ReadIniDataSources(ref connectionManager))
            {
                _log.Error("Could not read data source configuration from INI");
                return false;
            }

            if (!ini.ReadIniCompanies(ref companyIds))
            {
                _log.Error("Could not read company numbers for report from INI");
                return false;
            }

            if (!ini.ReadIniGroupExcludes(ref ExcludeGroupIds))
            {
                _log.Error("Could not read group exclude IDs for report from INI");
            }

            if (!ini.ReadIniUserExcludes(ref ExcludeUserIds))
            {
                _log.Error("Could not read user exclude IDs for report from INI");
            }

            string tempZeroCountStr = "";
            if(!ini.Ini.TryGetValue<string>(AppName, "ShowZeroLoginCountUsers", ref tempZeroCountStr))
            {
                _log.Error("Could not get zero count login users inclusion flag value from INI, defaulting to include");
            }
            else
            {
                tempZeroCountStr = tempZeroCountStr.Trim().ToUpper();
                if (tempZeroCountStr == "Y" || tempZeroCountStr == "YES" || tempZeroCountStr == "T" || tempZeroCountStr == "TRUE")
                    ShowZeroLoginCountUsers = true;
                else if (tempZeroCountStr == "N" || tempZeroCountStr == "NO" || tempZeroCountStr == "F" || tempZeroCountStr == "FALSE")
                    ShowZeroLoginCountUsers = false;
            }
            
            return true;*/
        }

        

        #endregion

        static void RunReport()
        {
            reportPeriod = new DateTimePeriod(reportStart, reportEnd);

            string reportTitle;
            if(BnnOnly)
            {
                reportTitle = String.Format(OutputFilename, reportDate.ToString("yyyyMMdd"), BnnOnlyFileSuffix);
            }
            else if(VideoStreamsOnly && !BnnOnly)
            {
                reportTitle = String.Format(OutputFilename, reportDate.ToString("yyyyMMdd"), VideoStreamsOnlyFileSuffix);
            }
            else
                reportTitle = String.Format(OutputFilename, reportDate.ToString("yyyyMMdd"), "");


            foreach (int companyId in companyIds)
            {
                Company cibc;
                if (!GetCompanyByID(companyId, out cibc))
                {
                    _log.Info("Cannot find company with ID " + companyId + ". Groups for this company ID will not be included in the report");

                    if (companyIds.Count > 1)
                        continue;
                    else
                        return;
                }

                _log.Info("Gathering all user login data for Company ID: " + companyId);

                //Not sure if we will ultimately need this if using the raw data more directly but we'll leave it for now
                try
                {
                    cibc.Groups = GetGroupsForCompany(cibc);

                }
                catch (Exception ex)
                {
                    string msg = String.Format("{0} RunReport GetGroupsForCompany1: {1}", ex.GetType(), ex.Message);
                    _log.Fatal(msg);
                }
                try
                {
                    BuildGroupAuditHistories(reportPeriod, cibc);

                }
                catch (Exception ex)
                {
                    string msg = String.Format("{0} RunReport BuildGroupAuditHistories2: {1}", ex.GetType(), ex.Message);
                    _log.Fatal(msg);
                }
                
                

                bool showZeroCountUsers = BnnOnly || VideoStreamsOnly || ShowZeroLoginCountUsers;

                try
                {
                    _log.Info(reportPeriod.Start.ToString());

                    _log.Info(reportPeriod.End.ToString());

                    BuildUserAuditHistories(reportPeriod, showZeroCountUsers);

                }
                catch (Exception ex)
                {

                    string msg = String.Format("{0} RunReport BuildUserAuditHistories3: {1}", ex.GetType(), ex.Message);
                    _log.Fatal(msg);
                }

                try
                {
                    BuildEntitlementLoginCounts(MonthEndOnly, showZeroCountUsers);

                }
                catch (Exception ex)
                {
                    string msg = String.Format("{0} RunReport BuildUserAuditHistories4: {1}", ex.GetType(), ex.Message);
                    _log.Fatal(msg);
                }
                
                

                _log.Info("Login data gathered successfully");

                WriteReportToFile(reportTitle, reportDate, ref usersToReport, CollateEntitlementFlag, showZeroCountUsers);
            }
        }

        private static void BuildEntitlementLoginCounts(bool monthEndOnly = false, bool showZeroCountUsers = false)
        {
            if (!monthEndOnly)
            {
                foreach (User user in usersToReport)
                {
                    List<AuditEntry<User>> userAuditHistory = new List<AuditEntry<User>>();
                    if (!userAuditHistories.TryGetValue(user.UserID, out userAuditHistory))
                        continue;

                    List<Login> logins = new List<Login>();
                    foreach (Login login in user.Logins)
                    {
                        List<string> entCodesForLogin = GetNoChangeEntitlementCodesForLogin(user.UserID, login, userAuditHistory, ref groupAuditHistories);
                        foreach (string entCode in entCodesForLogin)
                        {
                            if (!user.EntitlementStringLoginLists.ContainsKey(entCode))
                                user.EntitlementStringLoginLists.Add(entCode, new List<Login> { login });
                            else
                                user.EntitlementStringLoginLists[entCode].Add(login);

                            if (!user.EntitlementStringLoginCounts.ContainsKey(entCode))
                                user.EntitlementStringLoginCounts.Add(entCode, 1);
                            else
                            {
                                user.EntitlementStringLoginCounts[entCode] += 1;
                            }
                        }
                    }
                }

                if (showZeroCountUsers)
                {
                    List<User> zeroCountUserList = new List<User>();

                    foreach (User user in allUsers.Where(user => !usersToReport.Contains(user)))
                    {
                        user.EntitlementStringLoginCounts.Add(user.DummyEntitlementString, 0);
                        zeroCountUserList.Add(user);
                    }

                    foreach (User add in zeroCountUserList)
                        usersToReport.Add(add);
                }

            }
        }

        #region Audit history building methods

        private static void BuildGroupAuditHistories(DateTimePeriod reportPeriod, Company company)
        {
            int[] groupIds = new int[200] ;
            try
            {
                groupIds = SqlHelper.GetAllGroupIds(company, connectionManager.GetConnection("clidata"), reportPeriod);
            }
            catch (Exception ex)
            {
                string msg = String.Format("{0} BuildGroupAuditHistories GetAllGroupIds: {1}", ex.GetType(), ex.Message);
                _log.Fatal(msg);
            }
            List<int> excludes = groupIds.Where(grp => ExcludeGroupIds.Contains<int>(grp)).ToList();
            try
            {

            
            foreach (int groupId in groupIds.Where(grp => !ExcludeGroupIds.Contains<int>(grp)))
            {
                int[] groupUserIds = SqlHelper.GetAllUserIds(groupId, connectionManager.GetConnection("clidata"), reportPeriod, ExcludeUserIds);

                foreach (int userId in groupUserIds)
                {
                    if (ExcludeUserIds.Contains(userId))
                        continue;
                    else
                    {
                        if (!allUserIds.Contains(userId))
                            allUserIds.Add(userId);
                    }
                }

                groupAuditHistories.Add(groupId, AuditHistory.GetGroupAuditHistory(groupId, connectionManager.GetConnection("clidata")));
            }
            }
            catch (Exception ex)
            {
                string msg = String.Format("{0} BuildGroupAuditHistories GetAllUserIds: {1}", ex.GetType(), ex.Message);
                _log.Fatal(msg);
            }

        }

        private static void BuildUserAuditHistories(DateTimePeriod reportPeriod, bool showZeroCountUsers)
        {

            User toAdd;

            foreach (int userId in allUserIds)
            {

                try
                {

                   

                    try
                    {
                        
                         toAdd = SqlHelper.GetUser(userId, connectionManager.GetConnection("clidata"), reportPeriod);
                         toAdd.Logins = SqlHelper.GetTimePeriodLoginsForUser(toAdd.UserID, reportPeriod, connectionManager.GetConnection("clidata"));
                    }
                    catch (Exception ex)
                    {
                        _log.Info("userId:" + userId);
                        string msg = String.Format("{0} BuildUserAuditHistories GetUser: {1}", ex.GetType(), ex.Message);
                        _log.Fatal(msg);
                        /*break*/;
                        continue;
                    }

                    
                    loginsByUserId.Add(toAdd.UserID, toAdd.Logins);

                    if (toAdd.Logins.Count > 0 /*|| (toAdd.Logins.Count == 0 && showZeroCountUsers)*/)
                    {
                        usersToReport.Add(toAdd);
                       
                    }
                }
                catch (Exception ex)
                {

                    string msg = String.Format("{0} BuildUserAuditHistories showZeroCountUsers1: {1}", ex.GetType(), ex.Message);
                    _log.Fatal(msg);
                }
                try
                {
                    
                    userAuditHistories.Add(userId, AuditHistory.GetUserAuditHistory(userId, connectionManager.GetConnection("clidata")));
                }
                catch (Exception ex)
                {
                    _log.Info("userId:" + userId);
                    string msg = String.Format("{0} BuildUserAuditHistories showZeroCountUsers2: {1}", ex.GetType(), ex.Message);
                    _log.Fatal(msg);
                }
               
            }
        }

        #endregion

        private static void WriteReportToFile(string reportTitle, DateTime reportDate, ref HashSet<User> usersToReport, bool collateEntitlements, bool showZeroCountUsers, bool orderUsers = false)
        {
            _log.Info("Writing report to file");

            long recordCount = 0;

            Dictionary<User, List<EntitlementOutput>> outputLinesByUser = new Dictionary<User, List<EntitlementOutput>>();

            List<string> collationEntitlements = new List<string>();

            IEnumerable<User> writeUsers = usersToReport;
            if (showZeroCountUsers || orderUsers)
                writeUsers = usersToReport.OrderBy(user => user.UserID);

            foreach(User user in writeUsers)
            {
                List<EntitlementOutput> outputLines = new List<EntitlementOutput>();
                Dictionary<EntitlementSet, int> entSetLogins = new Dictionary<EntitlementSet, int>();

                foreach (KeyValuePair<string, int> kv in user.EntitlementStringLoginCounts)
                {
                    if (kv.Key.Length != 5)
                        continue;
                    for (int i = 0; i < kv.Key.Length; i++)
                    {
                        EntitlementChar key = new Tuple<char, int>(kv.Key[i], i);

                        if ((BnnOnly | VideoStreamsOnly) && i != VIDEO_STREAM_CHAR_INDEX)
                            continue;

                        EntitlementSet entSet;
                        if (EntitlementSet.EntitlementSets.TryGetValue(key, out entSet))
                        {
                            int currentLogins;
                            if (entSetLogins.TryGetValue(entSet, out currentLogins))
                            {
                                entSetLogins[entSet] = currentLogins + kv.Value;
                            }
                            else
                                entSetLogins.Add(entSet, kv.Value);
                        }
                    }
                }

                if (GenerateEntitlementOutputsForUser(user, entSetLogins, reportDate, showZeroCountUsers, out outputLines))
                {
                    outputLinesByUser.Add(user, outputLines);

                    if(collateEntitlements)
                    {
                        foreach (string description in outputLines.Select(line => line.EntitlementDescription).Distinct())
                        {
                            if (BnnOnly)
                            {
                                if (description == "BNN Video" && !collationEntitlements.Contains(description))
                                    collationEntitlements.Add(description);
                            }
                            else
                            {
                                if (!collationEntitlements.Contains(description))
                                    collationEntitlements.Add(description);
                            }
                        }
                    }
                }
            }


            try
            {
                if (File.Exists(reportTitle))
                    File.Delete(reportTitle);

                using (StreamWriter writer = new StreamWriter(reportTitle))
                {
                    

                    if(collateEntitlements)
                    {
                        foreach (string description in collationEntitlements)
                        {
                            writer.WriteLine("Service,," + OutputHelper.CsvFieldFormat(description));
                            writer.WriteLine();

                            writer.WriteLine(GetCollatedSubHeaderString());

                            int userOrdinal = 1;

                            foreach(User user in outputLinesByUser.Keys)
                            {
                                List<EntitlementOutput> userOutputLines;

                                if(outputLinesByUser.TryGetValue(user, out userOutputLines))
                                {
                                    EntitlementOutput output = userOutputLines.FirstOrDefault(line => line.EntitlementDescription == description);

                                    if(output != null)
                                    {
                                        writer.WriteLine(userOrdinal + "," + output.EntitlementString);
                                        userOrdinal++;
                                    }
                                }
                            }

                            writer.WriteLine();
                            writer.WriteLine();
                        }
                    }
                    else
                    {
                        writer.WriteLine(GetHeaderString());

                        foreach(User user in outputLinesByUser.Keys)
                        {
                            List<EntitlementOutput> userOutputs;
                            if(!outputLinesByUser.TryGetValue(user, out userOutputs))
                                continue;

                            foreach (EntitlementOutput output in userOutputs)
                            {
                                writer.WriteLine(output.EntitlementString);
                                ++recordCount;
                            }
                        }

                        writer.WriteLine(GetFooterString(recordCount));
                    }
                }
            }
            catch (Exception e)
            {
                _log.Info("Something went wrong building output strings and outputting to file:");
                _log.Info("Ex Message: " + e.Message);
                _log.Info(e.StackTrace);
                //Console.ReadKey();
            }
        }

        public static bool GenerateEntitlementOutputsForUser(User user, Dictionary<EntitlementSet, int> entSetLogins, DateTime reportDate, bool showZeroCountUsers, out List<EntitlementOutput> entitlementOutputs)
        {
            entitlementOutputs = new List<EntitlementOutput>();

            if (entSetLogins.Count == 0)
                return false;

            foreach (KeyValuePair<EntitlementSet, int> kv in entSetLogins)
            {
                List<EntitlementOutput> outputsForEntSet = GenerateOutputsForUserEntitlementSet(user, kv.Key, reportDate, kv.Value, showZeroCountUsers);
                foreach(EntitlementOutput entOutput in outputsForEntSet)
                {
                    if (!entitlementOutputs.Contains(entOutput))
                        entitlementOutputs.Add(entOutput);
                    else
                    {
                        int index = entitlementOutputs.IndexOf(entOutput);
                        entitlementOutputs[index].LoginCount += entOutput.LoginCount;
                    }
                }
            }
            return true;
        }

        private static List<EntitlementOutput> GenerateOutputsForUserEntitlementSet(User user, EntitlementSet entSet, DateTime reportDate, int loginCount, bool showZeroCountUsers)
        {
            List<EntitlementOutput> entitlementOutputs = new List<EntitlementOutput>();

            foreach (DataEntitlement entitlement in entSet.Entitlements)
            {
                entitlementOutputs.AddRange(entitlement.GetEntitlementAccessOutputsForUser(user, reportDate, loginCount, CollateEntitlementFlag, showZeroCountUsers));
            }

            return entitlementOutputs;
        }

        private static List<string> GetEntitlementStringsForUserNew(User user, DateTime reportDate)
        {
            List<EntitlementOutput> groupEntitlementLines = new List<EntitlementOutput>();

            foreach (string entCode in user.EntitlementStringLoginCounts.Keys)
            {

            }

            List<string> stringsToReturn = groupEntitlementLines.Select<EntitlementOutput, string>(entLine => entLine.EntitlementString).ToList();

            //List<string> groupEntitlementStrings = user.EntitlementGroup.GenerateEntitlementStringsForUser(user, reportDate);

            return stringsToReturn;
        }

        public static List<string> GetNoChangeEntitlementCodesForLogin(int userId, Login login, List<AuditEntry<User>> userAuditHistory, ref Dictionary<int, List<AuditEntry<Group>>> groupAuditHistories)
        {
            List<string> entitlementCodes = new List<string>();

            List<AuditEntry<User>> userChangesInPeriod = GetAuditChanges(login.DateTimePeriod, userAuditHistory);

            //No longer attributing single logins to multiple entitlement sets/groups
            //This used to apply if the user's entitlement group changed, or if the entitlement group's data entitlements were changed, while the user was logged in
            //The login will now be attributed only to the entitlements at the beginning of the login period because changes do not take effect until new login

            GetNoChangeEntitlementCodesForNoChangePeriod(login.DateTimePeriod, userAuditHistory, groupAuditHistories, ref entitlementCodes);

            #region Old method - attributes single logins to multiple groups if entitlements are changed mid-login
            /*//If there is no user group change in this period, we grab the immediately previous audit entry to the login to determine details
            if(userChangesInPeriod.Count == 0)
            {
                GetEntitlementCodesForNoChangePeriod(login.DateTimePeriod, userAuditHistory, groupAuditHistories, ref entitlementCodes);
            }
            else
            {
                bool groupChanged = false;
                int firstGroupId = userChangesInPeriod.First().Before.GroupID;
                List<int> allGroupIds = new List<int> { firstGroupId };


                foreach(AuditEntry<User> entry in userChangesInPeriod)
                {
                    if (entry.After.GroupID != firstGroupId)
                    {
                        groupChanged = true;
                        allGroupIds.Add(entry.After.GroupID);
                    }
                }

                if (!groupChanged)
                    GetEntitlementCodesForNoChangePeriod(login.DateTimePeriod, userAuditHistory, groupAuditHistories, ref entitlementCodes);
                else
                {
                    foreach(int id in allGroupIds)
                    {
                        List<AuditEntry<Group>> groupAuditHistory;
                        if(groupAuditHistories.TryGetValue(id, out groupAuditHistory))
                        {
                            List<AuditEntry<Group>> periodChanges = GetAuditChanges(login.DateTimePeriod, groupAuditHistory);

                            string code;
                            if (periodChanges.Count == 0)
                            {
                                code = GetLatestAuditBeforeDateTime<Group>(login.LoginDateTime, groupAuditHistory).After.Description;

                                if (!entitlementCodes.Contains(code) && code.Length == 5)
                                    entitlementCodes.Add(code);
                            }
                            else
                            {
                                if(periodChanges.First().Before != null)
                                {
                                    code = periodChanges.First().Before.Description;
                                    if (!entitlementCodes.Contains(code) && code.Length == 5)
                                        entitlementCodes.Add(code);
                                }
                            }

                            foreach(AuditEntry<Group> entry in periodChanges)
                            {
                                code = entry.After.Description;
                                if (!entitlementCodes.Contains(code))
                                    entitlementCodes.Add(code);
                            }
                        }
                    }
                }
            }*/
            #endregion

            return entitlementCodes;
        }

        public static List<string> GetEntitlementCodesForLogin(int userId, Login login, List<AuditEntry<User>> userAuditHistory, ref Dictionary<int, List<AuditEntry<Group>>> groupAuditHistories)
        {
            List<string> entitlementCodes = new List<string>();

            List<AuditEntry<User>> userChangesInPeriod = GetAuditChanges(login.DateTimePeriod, userAuditHistory);

            //If there is no user group change in this period, we grab the immediately previous audit entry to the login to determine details
            if(userChangesInPeriod.Count == 0)
            {
                GetEntitlementCodesForNoChangePeriod(login.DateTimePeriod, userAuditHistory, groupAuditHistories, ref entitlementCodes);
            }
            else
            {
                bool groupChanged = false;
                int firstGroupId = userChangesInPeriod.First().Before.GroupID;
                List<int> allGroupIds = new List<int> { firstGroupId };


                foreach(AuditEntry<User> entry in userChangesInPeriod)
                {
                    if (entry.After.GroupID != firstGroupId)
                    {
                        groupChanged = true;
                        allGroupIds.Add(entry.After.GroupID);
                    }
                }

                if (!groupChanged)
                    GetEntitlementCodesForNoChangePeriod(login.DateTimePeriod, userAuditHistory, groupAuditHistories, ref entitlementCodes);
                else
                {
                    foreach(int id in allGroupIds)
                    {
                        List<AuditEntry<Group>> groupAuditHistory;
                        if(groupAuditHistories.TryGetValue(id, out groupAuditHistory))
                        {
                            List<AuditEntry<Group>> periodChanges = GetAuditChanges(login.DateTimePeriod, groupAuditHistory);

                            string code;
                            if (periodChanges.Count == 0)
                            {
                                code = GetLatestAuditBeforeDateTime<Group>(login.LoginDateTime, groupAuditHistory).After.Description;

                                if (!entitlementCodes.Contains(code) && code.Length == 5)
                                    entitlementCodes.Add(code);
                            }
                            else
                            {
                                if(periodChanges.First().Before != null)
                                {
                                    code = periodChanges.First().Before.Description;
                                    if (!entitlementCodes.Contains(code) && code.Length == 5)
                                        entitlementCodes.Add(code);
                                }
                            }

                            foreach(AuditEntry<Group> entry in periodChanges)
                            {
                                code = entry.After.Description;
                                if (!entitlementCodes.Contains(code))
                                    entitlementCodes.Add(code);
                            }
                        }
                    }
                }
            }

            return entitlementCodes;
        }

        private static void GetNoChangeEntitlementCodesForNoChangePeriod(DateTimePeriod period, List<AuditEntry<User>> userAuditHistory, Dictionary<int, List<AuditEntry<Group>>> groupAuditHistories, ref List<string> entitlementCodes, bool groupIdOverride = false, int overrideId = -1)
        {
            int groupId;
            if (!groupIdOverride)
            {
                AuditEntry<User> latestBeforePeriod = GetLatestAuditBeforeDateTime(period.Start, userAuditHistory);

                groupId = latestBeforePeriod.After.GroupID;
            }
            else
            {
                groupId = overrideId;
            }

            List<AuditEntry<Group>> groupAuditHistory;
            if (!groupAuditHistories.TryGetValue(groupId, out groupAuditHistory))
                throw new Exception("Cannot find group ID for login!");
            else
            {
                List<AuditEntry<Group>> groupChangesInPeriod = GetAuditChanges(period, groupAuditHistory)
                                                                                .OrderBy(entry => entry.EntryTimestamp).ToList();

                Group beforePeriod = GetLatestAuditBeforeDateTime(period.Start, groupAuditHistory).After;
                if (!entitlementCodes.Contains(beforePeriod.Description))
                    entitlementCodes.Add(beforePeriod.Description);
            }
        }

        private static void GetEntitlementCodesForNoChangePeriod(DateTimePeriod period, List<AuditEntry<User>> userAuditHistory, Dictionary<int, List<AuditEntry<Group>>> groupAuditHistories, ref List<string> entitlementCodes)
        {
            AuditEntry<User> latestBeforePeriod = GetLatestAuditBeforeDateTime(period.Start, userAuditHistory);

            int groupId = latestBeforePeriod.After.GroupID;

            List<AuditEntry<Group>> groupAuditHistory;
            if (!groupAuditHistories.TryGetValue(groupId, out groupAuditHistory))
                throw new Exception("Cannot find group ID for login!");
            else
            {
                List<AuditEntry<Group>> groupChangesInPeriod = GetAuditChanges(period, groupAuditHistory)
                                                                                .OrderBy(entry => entry.EntryTimestamp).ToList();

                if (groupChangesInPeriod.Count == 0)
                {
                    Group beforePeriod = GetLatestAuditBeforeDateTime(period.Start, groupAuditHistory).After;
                    if (!entitlementCodes.Contains(beforePeriod.Description))
                        entitlementCodes.Add(beforePeriod.Description);
                }
                else
                {
                    entitlementCodes.Add(groupChangesInPeriod.First().Before.Description);

                    foreach (AuditEntry<Group> entry in groupChangesInPeriod)
                    {
                        if (!entitlementCodes.Contains(entry.After.Description))
                            entitlementCodes.Add(entry.After.Description);
                    }
                }
            }
        }

        public static AuditEntry<T> GetLatestAuditBeforeDateTime<T>(DateTime dt, List<AuditEntry<T>> entries)
        {
            return entries.Where(entry => entry.EntryTimestamp < dt)
                             .OrderByDescending(entry => entry.EntryTimestamp).First();
        }

        public static List<AuditEntry<T>> GetAuditChanges<T>(DateTimePeriod period, List<AuditEntry<T>> entries)
        {
            return entries.Where(entry => period.IsInPeriod(entry.EntryTimestamp)).ToList();
        }

        private static string GetHeaderString()
        {
            /*StringBuilder header = new StringBuilder();
            header.Append(OutputHelper.CsvFieldFormat("Quote Type (Video Data Source)", true));
            header.Append(OutputHelper.CsvFieldFormat("User ID"));
            header.Append(OutputHelper.CsvFieldFormat("Channel"));
            header.Append(OutputHelper.CsvFieldFormat("RT/D"));
            header.Append(OutputHelper.CsvFieldFormat("Exchange Data Source"));
            header.Append(OutputHelper.CsvFieldFormat("Login Count"));

            return header.ToString();*/

            StringBuilder header = new StringBuilder();

            header.Append(OutputHelper.CsvFieldFormat("HDR", true));
            header.Append(OutputHelper.CsvFieldFormat(reportStart.Date.ToString("dd:MM:yyyy")));
            header.Append(OutputHelper.CsvFieldFormat(reportEnd.Date.ToString("dd:MM:yyyy")));
            
            return header.ToString();
        }

        private static string GetFooterString(long recordCount)
        {
            StringBuilder footer = new StringBuilder();
            footer.Append(OutputHelper.CsvFieldFormat("FTR", true));
            footer.Append(",");
            footer.Append(",");
            footer.Append(",");
            footer.Append(OutputHelper.CsvFieldFormat(recordCount));

            return footer.ToString();
        }

        private static string GetCollatedSubHeaderString()
        {
            StringBuilder subHeader = new StringBuilder();
            subHeader.Append(OutputHelper.CsvFieldFormat("User #", true));
            subHeader.Append(OutputHelper.CsvFieldFormat("User ID"));
            subHeader.Append(OutputHelper.CsvFieldFormat("Type"));
            subHeader.Append(OutputHelper.CsvFieldFormat("Browser"));
            subHeader.Append(OutputHelper.CsvFieldFormat("# of Logins"));

            return subHeader.ToString();
        }

        #region Database access and object building methods

        private static void GetTimePeriodLoginsForUser(User user, DateTimePeriod period)
        {
            try
            {
                SqlConnection conn = connectionManager.GetConnection("clidata");
                string cmdStr = String.Format(@"SELECT Time, Duration, RequestCount from access_log WHERE UserID = {0}", user.UserID);

                using (SqlCommand cmd = new SqlCommand(cmdStr, conn))                  
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if(reader.HasRows)
                    {
                        while(reader.Read())
                        {
                            if (reader.IsDBNull(reader.GetOrdinal("Time")) || reader.IsDBNull(reader.GetOrdinal("Duration")))
                                continue;

                            DateTime loginTime = reader.GetDateTime(reader.GetOrdinal("Time"));
                            TimeSpan dur = new TimeSpan(0, 0, reader.GetInt32(reader.GetOrdinal("Duration")));

                            int rqCount;
                            if (reader.IsDBNull(reader.GetOrdinal("RequestCount")))
                                rqCount = 0;
                            else
                                rqCount = reader.GetInt32(reader.GetOrdinal("RequestCount"));

                            Login login = new Login(loginTime, dur, rqCount);

                            if(period.Overlaps(login.DateTimePeriod))
                                user.Logins.Add(login);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string msg = String.Format("{0} GetTimePeriodLoginsForUser: {1}", ex.GetType(), ex.Message);
                _log.Fatal(msg);
            }
            
            
        }

        private static List<User> GetUsersByGroup(Group group, DateTimePeriod period, ref HashSet<User> allUsers)
        {
            return GetUsersByGroup(group.GroupID, period, ref allUsers);
        }

        private static List<User> GetUsersByGroup(int groupId, DateTimePeriod reportPeriod, ref HashSet<User> allUsers)
        {
            List<User> users = new List<User>();
            try
            {
 SqlConnection conn = connectionManager.GetConnection("clidata");
                string cmdStr = String.Format(@"SELECT UserID, Name, Description from access_users WHERE GroupID = {0}", groupId);

                using (SqlCommand cmd = new SqlCommand(cmdStr,conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if(reader.HasRows)
                    {
                        while(reader.Read())
                        {
                            int id = reader.GetInt32(reader.GetOrdinal("UserID"));
                            string name = reader.GetString(reader.GetOrdinal("Name"));
                            string description = reader.GetString(reader.GetOrdinal("Description"));

                            User newUser = new User(id, name, description);

                            if (SqlHelper.UserExistedInPeriod(newUser, conn, reportPeriod))
                            {
                                if (allUsers.Contains(newUser))
                                {
                                    User existingUser = allUsers.Single(x => x.UserID == newUser.UserID);
                                    //existingUser.EntitlementGroup = group;
                                    users.Add(existingUser);
                                }
                                else
                                {
                                    //newUser.EntitlementGroup = group;
                                    users.Add(newUser);
                                    //allUsers.Add(newUser);
                                }
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                string msg = String.Format("{0} GetUsersByGroup: {1}", ex.GetType(), ex.Message);
                _log.Fatal(msg);
            }
           
                return users;
            
        }

        private static int[] GetUserIDsByGroup(Group group, DateTimePeriod period, ref HashSet<int> allUserIds, ref HashSet<User> allUsers)
        {
            return GetUserIDsByGroup(group.GroupID, period, ref allUserIds, ref allUsers);
        }

        private static int[] GetUserIDsByGroup(int groupId, DateTimePeriod reportPeriod, ref HashSet<int> allUserIds, ref HashSet<User> allUsers)
        {
            List<int> userIds = new List<int>();
            try
            {

           
            SqlConnection conn = connectionManager.GetConnection("clidata");
                string cmdStr = String.Format(@"SELECT UserID, Name, Description from access_users WHERE GroupID = {0}", groupId);

                using (SqlCommand cmd = new SqlCommand(cmdStr, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            int id = reader.GetInt32(reader.GetOrdinal("UserID"));
                            string name = reader.GetString(reader.GetOrdinal("Name"));
                            string description = reader.GetString(reader.GetOrdinal("Description"));

                            User newUser = new User(id, name, description);

                            if (SqlHelper.UserExistedInPeriod(newUser, conn, reportPeriod))
                            {
                                if (allUserIds.Contains(newUser.UserID))
                                {
                                    User existingUser = allUsers.Single(x => x.UserID == newUser.UserID);
                                    //existingUser.EntitlementGroup = group;

                                    userIds.Add(newUser.UserID);
                                }
                                else
                                {
                                    userIds.Add(id);
                                    allUserIds.Add(id);
                                }
                            }
                        }
                    }
                }

                string deletedRawQuery = @"SELECT * FROM cadb..audit_users_log 
                                            WHERE TStamp < '{0}' AND TStamp > '{1}' AND GroupID = '{2}' AND Action = 0";
                string deletedQuery = String.Format(deletedRawQuery, reportPeriod.End, reportPeriod.Start, groupId);
                //List<int> retList = new List<int>();
                //foreach
            }
            catch (Exception ex)
            {
                string msg = String.Format("{0} GetUserIDsByGroup: {1}", ex.GetType(), ex.Message);
                _log.Fatal(msg);
            }
                return userIds.ToArray();
           
        }

        private static List<Group> GetGroupsForCompany(Company company)
        {
            List<Group> groups = new List<Group>();
            try
            {

           
            SqlConnection conn = connectionManager.GetConnection("clidata");
                string testCmdStr = String.Format(@"SELECT GroupID, Description, PermissionID FROM access_groups WHERE CompanyID = {0}", company.CompanyID);
                string cmdStr = String.Format(@"SELECT GroupID, Description, PermissionID FROM access_groups WHERE LEN(Description) = 5 AND CompanyID = {0}", company.CompanyID);

                using (SqlCommand cmd = new SqlCommand(testCmdStr, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if(reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            groups.Add(GetGroupFromReaderRow(reader));
                        }
                    }
                }
            

            
            }
            catch (Exception ex)
            {
                string msg = String.Format("{0} GetGroupsForCompany: {1}", ex.GetType(), ex.Message);
                _log.Fatal(msg);
            }
            return groups;
        }

        private static Group GetGroupFromReaderRow(SqlDataReader reader)
        {
            int id = reader.GetInt32(reader.GetOrdinal("GroupID"));
            string description = reader.GetString(reader.GetOrdinal("Description"));

            int permissionId = -1;
            if (!reader.IsDBNull(reader.GetOrdinal("PermissionID")))
                permissionId = reader.GetInt32(reader.GetOrdinal("PermissionID"));

            Group toAdd = new Group(id, description);
            toAdd.PermissionID = permissionId;

            return toAdd;
        }
        
        private static bool GetCompanyByID(int companyId, out Company company)
        {
            try
            {
                  SqlConnection conn = connectionManager.GetConnection("clidata");
            string cmdStr = String.Format(@"SELECT CompanyID, Description, Name FROM access_companies WHERE CompanyID = {0}", companyId);

            using (SqlCommand cmd = new SqlCommand(cmdStr,conn))
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    reader.Read();

                    string description = reader.GetString(reader.GetOrdinal("Description"));
                    string name = reader.GetString(reader.GetOrdinal("Name"));

                    company = new Company(companyId, name, description);
                    return true;
                }
                else
                {
                    company = new Company(-1, "", "");
                    return false;
                }
            }
            }
            catch (Exception ex)
            {
                string msg = String.Format("{0} GetCompanyByID: {1}", ex.GetType(), ex.Message);
                _log.Fatal(msg);
            }
            company = new Company(-1, "", "");
            return false;
        }

        

        #endregion

 
    }
}
