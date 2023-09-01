using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using Iress.Canada.Reporting.Reports.Other.MarketDataUsageReport.Model;
using Iress.Canada.Reporting.Logging;
namespace Iress.Canada.Reporting.Reports.Other.MarketDataUsageReport.Data
{
    using log4net;
    public static class SqlHelper
    {
        public static readonly string SQL_DATETIME_FORMAT = "yyyy-MM-dd HH:mm:ss.fff";

        public static readonly int ViewPointLoginType = 16384;
        private static ILog _log;
        private static HashSet<Type> NumericTypes = new HashSet<Type>
        {
            typeof(byte), typeof(decimal), typeof(double), typeof(float), typeof(int), typeof(long), typeof(sbyte)
            , typeof(short), typeof(uint), typeof(ulong), typeof(ushort)
        };

        public static int? GetCurrentGroupIDFromUserAuditTable(DateTime time, SqlConnection conn, string table, string column, int userId)
        {
            string cmdStr = String.Format("SELECT TOP(1) * FROM {0} WHERE UserID = {1} AND TStamp < '{2}' ORDER BY TStamp DESC", table, userId, time.ToString(SQL_DATETIME_FORMAT));

            using(SqlCommand cmd = new SqlCommand(cmdStr, conn))
            using(SqlDataReader reader = cmd.ExecuteReader())
            {
                if (!reader.HasRows)
                    throw new Exception("Could not find a current value for given date time from the audit table supplied with query: " + cmdStr);

                reader.Read();

                if (reader.IsDBNull(reader.GetOrdinal(column)))
                    return null;
                else
                    return reader.GetInt32(reader.GetOrdinal(column));
            }
        }

        public static bool UserExistedInPeriod(User user, SqlConnection conn, DateTimePeriod period)
        {
            return UserExistedInPeriod(user.UserID, conn, period);
        }

        public static bool UserExistedInPeriod(int userId, SqlConnection conn, DateTimePeriod period)
        {
            string cmdStr = String.Format("SELECT * FROM cadb..audit_Users_Log WHERE UserID = {0} AND TStamp < '{1}' ORDER BY TStamp DESC", userId, period.End);
            using(SqlCommand cmd = new SqlCommand(cmdStr, conn))
            using(SqlDataReader reader = cmd.ExecuteReader())
            {
                if (!reader.HasRows)
                    return false;
                else
                {
                    reader.Read();
                    DateTime latestDT = reader.GetDateTime(reader.GetOrdinal("TStamp"));
                    int action = reader.GetInt32(reader.GetOrdinal("Action"));

                    if (action == 0)
                    {
                        //Deletion - check if delete occurred after period start
                        return latestDT > period.Start;
                    }
                    else
                        return true;
                }
            }
        }

        public static int[] GetAllGroupIds(int companyId, SqlConnection conn, DateTimePeriod period)
        {

            string currentGroupsQuery = String.Format("SELECT DISTINCT(GroupID) FROM clidata..access_groups WHERE CompanyID={0};", companyId);

            List<int> distinctIds = new List<int>();

            SqlCommand cmd;

            using(cmd = new SqlCommand(currentGroupsQuery, conn))
                cmd.CommandTimeout = 300;
            using(SqlDataReader reader = cmd.ExecuteReader())
            {
                while(reader.Read())
                    distinctIds.Add(reader.GetInt32(reader.GetOrdinal("GroupID")));
            }


            string rawThisPeriodQuery = @"SELECT DISTINCT(GroupID) FROM cadb..audit_Groups_log
                                            WHERE TStamp < '{0}' AND TStamp > '{1}' AND GroupID NOT IN {2}";
            string deletedThisPeriodQuery = String.Format(rawThisPeriodQuery, period.Start, period.End, BuildExcludeString(distinctIds));

            using (cmd = new SqlCommand(deletedThisPeriodQuery, conn))
                cmd.CommandTimeout = 300;
            using(SqlDataReader reader = cmd.ExecuteReader())
            {
                while(reader.Read())
                    distinctIds.Add(reader.GetInt32(reader.GetOrdinal("GroupID")));
            }

            return distinctIds.ToArray();
        }

        public static int[] GetAllGroupIds(Company company, SqlConnection conn, DateTimePeriod period)
        {
            return GetAllGroupIds(company.CompanyID, conn, period);
        }

        public static int[] GetAllUserIds(Group group, SqlConnection conn, DateTimePeriod period, IEnumerable<int> userExludeIds)
        {
            return GetAllUserIds(group.GroupID, conn, period, userExludeIds);
        }

        public static int[] GetAllUserIds(int groupId, SqlConnection conn, DateTimePeriod period, IEnumerable<int> userExcludeIds)
        {

            string currentUsersQuery = String.Format("SELECT DISTINCT(UserID) FROM clidata..access_users WHERE GroupID={0}", groupId);

            List<int> distinctIds = new List<int>();
            SqlCommand cmd;
            try
            {
                using (cmd = new SqlCommand(currentUsersQuery, conn))
                    cmd.CommandTimeout = 300;
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        distinctIds.Add(reader.GetInt32(reader.GetOrdinal("UserID")));
                }

            }
            catch (Exception ex)
            {
                string msg = String.Format("{0} GetAllUserIds 1: {1}", ex.GetType(), ex.Message);
                _log.Error(msg);
            }
            
            string rawDeletedThisPeriodQuery = "";
            string deletedThisPeriodQuery = "";

            if (distinctIds.Count <= 0)
            {
                rawDeletedThisPeriodQuery = @"SELECT DISTINCT(UserID) FROM cadb..audit_Users_log WHERE TStamp < '{0}' AND TStamp > '{1}' AND GroupID = {2}";

                deletedThisPeriodQuery = String.Format(rawDeletedThisPeriodQuery, period.Start, period.End, groupId);
            }
            else
            {
                rawDeletedThisPeriodQuery = @"SELECT DISTINCT(UserID) FROM cadb..audit_Users_log WHERE TStamp < '{0}' AND TStamp > '{1}' AND UserID NOT IN {2} AND GroupID = {3}";

                deletedThisPeriodQuery = String.Format(rawDeletedThisPeriodQuery, period.Start, period.End, BuildExcludeString(distinctIds.Concat<int>(userExcludeIds)), groupId);
            }

            //deletedThisPeriodQuery = String.Format(rawDeletedThisPeriodQuery, period.Start, period.End, BuildExcludeString(distinctIds));
            try
            {
                using (cmd = new SqlCommand(deletedThisPeriodQuery, conn))
                    cmd.CommandTimeout = 300;
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                        distinctIds.Add(reader.GetInt32(reader.GetOrdinal("GroupID")));
                }
            }
            catch (Exception ex)
            {
                string msg = String.Format("{0} GetAllUserIds 2: {1}", ex.GetType(), ex.Message);
                _log.Error(msg);
            }
            

            return distinctIds.ToArray();
        }

        public static string BuildExcludeString<T>(IEnumerable<T> itemsToExclude)
        {
            if(itemsToExclude.Count() <= 0)
                return "()";

            StringBuilder excludeBuilder = new StringBuilder();
            excludeBuilder.Append("(");

            if(IsNumericType(typeof(T)))
            {
                excludeBuilder.Append(itemsToExclude.First().ToString());
                
                foreach(T item in itemsToExclude.Skip(1))
                {
                    excludeBuilder.Append("," + item.ToString());
                }

                excludeBuilder.Append(")");
            }
            else
            {
                excludeBuilder.Append("'" + itemsToExclude.First().ToString() + "'");

                foreach (T item in itemsToExclude.Skip(1))
                {
                    excludeBuilder.Append(",'" + item.ToString() + "'");
                }

                excludeBuilder.Append(")");
            }

            return excludeBuilder.ToString();
        }

        public static bool IsNumericType(Type type)
        {
            return NumericTypes.Contains(type);
        }

        public static User GetUser(int id, SqlConnection conn, DateTimePeriod period)
        {
            string query = String.Format(@"SELECT users.UserID, users.Name as Name, users.Description as Description, users.GroupID, groups.Description as EntString FROM clidata..access_users as users join clidata..access_groups as groups on users.GroupID = groups.GroupID WHERE UserID={0}", id);

            using(SqlCommand cmd = new SqlCommand(query, conn))
            using(SqlDataReader reader = cmd.ExecuteReader())
            {
                if (!reader.HasRows)
                {
                    string auditQueryRaw = @"SELECT * FROM cadb..audit_users_log WHERE UserID={0} AND TStamp < '{1}' ORDER BY TStamp DESC";
                    string auditQuery = String.Format(auditQueryRaw, id, period.End);

                    using (SqlCommand auditCmd = new SqlCommand(auditQuery, conn))
                    using (SqlDataReader auditReader = auditCmd.ExecuteReader())
                    {
                        auditReader.Read();

                        if (!auditReader.HasRows)
                            throw new Exception("No records in database for User ID: " + id);
                        else
                        {
                            string name = reader.GetString(reader.GetOrdinal("Name"));
                            string description = reader.GetString(reader.GetOrdinal("Description"));

                            return new User(id, name, description);
                        }
                    }
                }
                else
                {
                    reader.Read();

                    string name = reader.GetString(reader.GetOrdinal("Name"));
                    string description = reader.GetString(reader.GetOrdinal("Description"));

                    string dummyEntString = reader.GetString(reader.GetOrdinal("EntString"));

                    return new User(id, name, description, dummyEntString);
                }
            }
        }

        public static List<Login> GetTimePeriodLoginsForUser(int userId, DateTimePeriod period, SqlConnection conn)
        {
            List<Login> toReturn = new List<Login>();

                string cmdStr = String.Format(@"SELECT Time, Duration, RequestCount, LoginType from clidata..access_log WHERE UserID = {0} AND LoginType = {1}", userId, ViewPointLoginType);

                using (SqlCommand cmd = new SqlCommand(cmdStr, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
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

                            if (period.Overlaps(login.DateTimePeriod))
                                toReturn.Add(login);
                        }
                    }
                }
            

            return toReturn;
        }

        public static List<Login> GetTimePeriodLoginsForUser(User user, DateTimePeriod period, SqlConnection conn)
        {
            return GetTimePeriodLoginsForUser(user.UserID, period, conn);
        }
    }
}
