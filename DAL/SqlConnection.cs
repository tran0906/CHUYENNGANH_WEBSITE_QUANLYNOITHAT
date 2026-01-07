// FILE: DAL/SqlConnection.cs - Lớp kết nối và thao tác với SQL Server

using System.Data;
using Microsoft.Data.SqlClient;

namespace DOANCHUYENNGANH_WEB_QLNOITHAT.DAL
{
    public class SqlConnectionHelper
    {
        // Chuỗi kết nối đến SQL Server (Server, Database, Authentication)
        private static string connectionString = @"Server=LAPTOP-R442T6OB\MSSQLSERVER2025;Database=WEB_NOITHAT;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true";

        // Property để lấy/gán ConnectionString
        public static string ConnectionString
        {
            get { return connectionString; }
            set { connectionString = value; }
        }

        // Tạo kết nối mới đến SQL Server
        public static SqlConnection GetConnection()
        {
            SqlConnection conn = new SqlConnection(connectionString);
            return conn;
        }

        // Thực thi SELECT - Trả về DataTable chứa kết quả
        public static DataTable ExecuteQuery(string query, SqlParameter[]? parameters = null)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = GetConnection())
            {
                conn.Open(); // Mở kết nối
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    if (parameters != null)
                        cmd.Parameters.AddRange(parameters); // Thêm tham số

                    using (SqlDataAdapter da = new SqlDataAdapter(cmd))
                    {
                        da.Fill(dt); // Đổ dữ liệu vào DataTable
                    }
                }
            } // Tự động đóng kết nối khi ra khỏi using
            return dt;
        }

        // Thực thi INSERT/UPDATE/DELETE - Trả về số dòng bị ảnh hưởng
        public static int ExecuteNonQuery(string query, SqlParameter[]? parameters = null)
        {
            using (SqlConnection conn = GetConnection())
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    if (parameters != null)
                        cmd.Parameters.AddRange(parameters);

                    return cmd.ExecuteNonQuery(); // Trả về số dòng thay đổi
                }
            }
        }

        // Thực thi COUNT/MAX/MIN/SUM - Trả về 1 giá trị đơn
        public static object? ExecuteScalar(string query, SqlParameter[]? parameters = null)
        {
            using (SqlConnection conn = GetConnection())
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    if (parameters != null)
                        cmd.Parameters.AddRange(parameters);

                    return cmd.ExecuteScalar(); // Trả về giá trị đầu tiên
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
