// FILE: DAL/SqlConnection.cs
// LỚP KẾT NỐI SQL SERVER - Cung cấp các phương thức thực thi SQL chung
// Tất cả DAL đều gọi qua lớp này để thao tác với database

using System.Data;
using Microsoft.Data.SqlClient;

namespace DOANCHUYENNGANH_WEB_QLNOITHAT.DAL
{
    public class SqlConnectionHelper
    {
        // Chuỗi kết nối SQL Server
        private static string connectionString = @"Server=LAPTOP-R442T6OB\MSSQLSERVER2025;Database=WEB_NOITHAT;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true";

        public static string ConnectionString
        {
            get { return connectionString; }
            set { connectionString = value; }
        }

        // Tạo connection mới
        public static SqlConnection GetConnection()
        {
            SqlConnection conn = new SqlConnection(connectionString);
            return conn;
        }

        // Thực thi SELECT - Trả về DataTable
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
            } // Tự động đóng connection
            return dt;
        }

        // Thực thi INSERT/UPDATE/DELETE - Trả về số dòng ảnh hưởng
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

        // Thực thi COUNT/MAX/MIN - Trả về 1 giá trị
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

        // Thực thi Stored Procedure - Trả về DataTable
        public static DataTable ExecuteStoredProcedure(string spName, SqlParameter[]? parameters = null)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = GetConnection())
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(spName, conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure; // Đánh dấu là SP
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

        // Thực thi Stored Procedure không trả về dữ liệu
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
