using System.Data;
using Microsoft.Data.SqlClient;
using DOANCHUYENNGANH_WEB_QLNOITHAT.Models;

namespace DOANCHUYENNGANH_WEB_QLNOITHAT.DAL
{
    public class SystemConfigDAL
    {
        public List<SystemConfig> GetAll()
        {
            string query = "SELECT * FROM SYSTEM_CONFIG ORDER BY ConfigKey";
            return MapDataTableToList(SqlConnectionHelper.ExecuteQuery(query));
        }

        public SystemConfig? GetByKey(string key)
        {
            string query = "SELECT * FROM SYSTEM_CONFIG WHERE ConfigKey = @Key";
            SqlParameter[] parameters = { new SqlParameter("@Key", key) };
            return MapDataTableToList(SqlConnectionHelper.ExecuteQuery(query, parameters)).FirstOrDefault();
        }

        public string GetValue(string key, string defaultValue = "")
        {
            var config = GetByKey(key);
            return config?.ConfigValue ?? defaultValue;
        }

        public int GetIntValue(string key, int defaultValue = 0)
        {
            var value = GetValue(key);
            return int.TryParse(value, out int result) ? result : defaultValue;
        }

        public int Update(string key, string value, string? updatedBy = null)
        {
            string query = @"UPDATE SYSTEM_CONFIG SET ConfigValue = @Value, UpdatedAt = GETDATE(), UpdatedBy = @UpdatedBy 
                            WHERE ConfigKey = @Key";
            SqlParameter[] parameters = {
                new SqlParameter("@Key", key),
                new SqlParameter("@Value", value),
                new SqlParameter("@UpdatedBy", (object?)updatedBy ?? DBNull.Value)
            };
            return SqlConnectionHelper.ExecuteNonQuery(query, parameters);
        }

        public int InsertOrUpdate(string key, string value, string? description = null, string? updatedBy = null)
        {
            var existing = GetByKey(key);
            if (existing != null)
            {
                return Update(key, value, updatedBy);
            }
            
            string query = @"INSERT INTO SYSTEM_CONFIG (ConfigKey, ConfigValue, Description, UpdatedBy) 
                            VALUES (@Key, @Value, @Description, @UpdatedBy)";
            SqlParameter[] parameters = {
                new SqlParameter("@Key", key),
                new SqlParameter("@Value", value),
                new SqlParameter("@Description", (object?)description ?? DBNull.Value),
                new SqlParameter("@UpdatedBy", (object?)updatedBy ?? DBNull.Value)
            };
            return SqlConnectionHelper.ExecuteNonQuery(query, parameters);
        }

        private List<SystemConfig> MapDataTableToList(DataTable dt)
        {
            var list = new List<SystemConfig>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new SystemConfig
                {
                    ConfigKey = row["ConfigKey"].ToString() ?? "",
                    ConfigValue = row["ConfigValue"].ToString() ?? "",
                    Description = row["Description"] != DBNull.Value ? row["Description"].ToString() : null,
                    UpdatedAt = Convert.ToDateTime(row["UpdatedAt"]),
                    UpdatedBy = row["UpdatedBy"] != DBNull.Value ? row["UpdatedBy"].ToString() : null
                });
            }
            return list;
        }
    }
}
