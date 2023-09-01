using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Iress.Canada.Reporting.Reports.Other.MarketDataUsageReport.Model;
using System.Data;
using System.Data.SqlClient;

namespace Iress.Canada.Reporting.Reports.Other.MarketDataUsageReport.Data
{
    public static class AuditHistory
    {
        public static List<AuditEntry<User>> GetUserAuditHistory(int userId, SqlConnection conn)
        {

            List<AuditEntry<User>> auditEntries = new List<AuditEntry<User>>();

            string getQuery = String.Format("SELECT * from cadb..audit_Users_Log WHERE UserID={0} ORDER BY TStamp ASC", userId);

            using(SqlCommand cmd = new SqlCommand(getQuery, conn))
            using(SqlDataReader reader = cmd.ExecuteReader())
            {
                if (!reader.HasRows)
                    throw new Exception("No audit history for user ID");

                User previous = null;
                while(reader.Read())
                {
                    int id = reader.GetInt32(reader.GetOrdinal("UserID"));
                    string name = reader.GetString(reader.GetOrdinal("Name"));
                    int groupId = reader.GetInt32(reader.GetOrdinal("GroupID"));

                    string description = "";

                    if(!reader.IsDBNull(reader.GetOrdinal("Description")))
                        description = reader.GetString(reader.GetOrdinal("Description"));

                    DateTime timestamp = reader.GetDateTime(reader.GetOrdinal("TStamp"));

                    User usr = new User(id, name, description);
                    usr.GroupID = groupId;
                    AuditEntry<User> toAdd = new AuditEntry<User>(timestamp);
                    toAdd.After = usr;

                    if (previous != null)
                        toAdd.Before = previous;

                    auditEntries.Add(toAdd);

                    previous = usr;
                }
            }

            return auditEntries;
        }

        public static List<AuditEntry<Group>> GetGroupAuditHistory(int groupId, SqlConnection conn)
        {

            List<AuditEntry<Group>> auditEntries = new List<AuditEntry<Group>>();

            string getQuery = String.Format("SELECT * from cadb..audit_Groups_Log WHERE GroupID={0} ORDER BY TStamp ASC", groupId);

            using (SqlCommand cmd = new SqlCommand(getQuery, conn))
            using (SqlDataReader reader = cmd.ExecuteReader())
            {
                if (!reader.HasRows)
                    throw new Exception("No audit history for user ID");

                Group previous = null;
                while (reader.Read())
                {
                    int id = reader.GetInt32(reader.GetOrdinal("GroupID"));

                    string description = reader.GetString(reader.GetOrdinal("Description"));

                    DateTime timestamp = reader.GetDateTime(reader.GetOrdinal("TStamp"));

                    Group grp = new Group(id, description);
                    AuditEntry<Group> toAdd = new AuditEntry<Group>(timestamp);

                    if (!reader.IsDBNull(reader.GetOrdinal("PermissionID")))
                        grp.PermissionID = reader.GetInt32(reader.GetOrdinal("PermissionID"));

                    toAdd.After = grp;

                    if (previous != null)
                        toAdd.Before = previous;

                    auditEntries.Add(toAdd);
                    previous = grp;
                }
            }

            return auditEntries;
        }
    }
}
