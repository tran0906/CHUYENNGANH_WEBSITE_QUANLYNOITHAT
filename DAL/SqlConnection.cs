using System.Data;
using Microsoft.Data.SqlClient;

namespace DOANCHUYENNGANH_WEB_QLNOITHAT.DAL
{
    /// <summary>
    /// Lớp kết nối cơ sở dữ liệu SQL Server
    /// Mô hình 3 lớp - Data Access Layer
    /// </summary>
    public class SqlConnectionHelper
    {
        // Connection String kết nối đến SQL Server
        private static string connectionString = @"Server=LAPTOP-R442T6OB\MSSQLSERVER2025;Database=WEB_NOITHAT;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true";

        /// <summary>
        /// Lấy Connection String
        /// </summary>
        public static string ConnectionString
        {
            get { return connectionString; }
            set { connectionString = value; }
        }

        /// <summary>
        /// Tạo và mở kết nối mới
        /// </summary>
        public static SqlConnection GetConnection()
        {
            SqlConnection conn = new SqlConnection(connectionString);
            return conn;
        }

        /// <summary>
        /// Thực thi câu lệnh SELECT - Trả về DataTable
        /// </summary>
        /// <param name="query">Câu lệnh SQL</param>
        /// <param name="parameters">Tham số (nếu có)</param>
        public static DataTable ExecuteQuery(string query, SqlParameter[]? parameters = null)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = GetConnection())
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    if (parameters != null)
                        cmd.Parameters.AddRange(parameters);

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }
            return dt;
        }

        /// <summary>
        /// Thực thi câu lệnh INSERT, UPDATE, DELETE - Trả về số dòng bị ảnh hưởng
        /// </summary>
        /// <param name="query">Câu lệnh SQL</param>
        /// <param name="parameters">Tham số (nếu có)</param>
        public static int ExecuteNonQuery(string query, SqlParameter[]? parameters = null)
        {
            using (SqlConnection conn = GetConnection())
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    if (parameters != null)
                        cmd.Parameters.AddRange(parameters);

                    return cmd.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Thực thi câu lệnh trả về một giá trị đơn (COUNT, MAX, MIN, SUM...)
        /// </summary>
        /// <param name="query">Câu lệnh SQL</param>
        /// <param name="parameters">Tham số (nếu có)</param>
        public static object? ExecuteScalar(string query, SqlParameter[]? parameters = null)
        {
            using (SqlConnection conn = GetConnection())
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    if (parameters != null)
                        cmd.Parameters.AddRange(parameters);

                    return cmd.ExecuteScalar();
                }
            }
        }

        /// <summary>
        /// Thực thi Stored Procedure - Trả về DataTable
        /// </summary>
        /// <param name="spName">Tên Stored Procedure</param>
        /// <param name="parameters">Tham số (nếu có)</param>
        public static DataTable ExecuteStoredProcedure(string spName, SqlParameter[]? parameters = null)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = GetConnection())
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(spName, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (parameters != null)
                        cmd.Parameters.AddRange(parameters);

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt);
                    }
                }
            }
            return dt;
        }

        /// <summary>
        /// Thực thi Stored Procedure không trả về dữ liệu
        /// </summary>
        /// <param name="spName">Tên Stored Procedure</param>
        /// <param name="parameters">Tham số (nếu có)</param>
        public static int ExecuteStoredProcedureNonQuery(string spName, SqlParameter[]? parameters = null)
        {
            using (SqlConnection conn = GetConnection())
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(spName, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    if (parameters != null)
                        cmd.Parameters.AddRange(parameters);

                    return cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
