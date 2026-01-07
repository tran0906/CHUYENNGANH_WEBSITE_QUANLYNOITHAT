// FILE: DAL/UserDAL.cs - Truy cập dữ liệu bảng [User] (nhân viên/admin)

using System.Data;
using Microsoft.Data.SqlClient;
using DOANCHUYENNGANH_WEB_QLNOITHAT.Models;

namespace DOANCHUYENNGANH_WEB_QLNOITHAT.DAL
{
    public class UserDAL
    {
        // Lấy tất cả user
        public List<User> GetAll()
        {
            string query = "SELECT * FROM [User] ORDER BY UserId";
            return MapDataTableToList(SqlConnectionHelper.ExecuteQuery(query));
        }

        public User? GetById(string userId)
        {
            string query = "SELECT * FROM [User] WHERE UserId = @UserId";
            SqlParameter[] parameters = { new SqlParameter("@UserId", userId) };
            return MapDataTableToList(SqlConnectionHelper.ExecuteQuery(query, parameters)).FirstOrDefault();
        }

        // Đăng nhập - kiểm tra tên đăng nhập và mật khẩu
        public User? Login(string tenUser, string matKhau)
        {
            string query = "SELECT * FROM [User] WHERE TenUser = @TenUser AND MatKhau = @MatKhau";
            SqlParameter[] parameters = {
                new SqlParameter("@TenUser", tenUser),
                new SqlParameter("@MatKhau", matKhau)
            };
            return MapDataTableToList(SqlConnectionHelper.ExecuteQuery(query, parameters)).FirstOrDefault();
        }

        public int Insert(User obj)
        {
            string query = @"INSERT INTO [User] (UserId, TenUser, MatKhau, HoTen, VaiTro) 
                            VALUES (@UserId, @TenUser, @MatKhau, @HoTen, @VaiTro)";
            SqlParameter[] parameters = {
                new SqlParameter("@UserId", obj.UserId),
                new SqlParameter("@TenUser", (object?)obj.TenUser ?? DBNull.Value),
                new SqlParameter("@MatKhau", (object?)obj.MatKhau ?? DBNull.Value),
                new SqlParameter("@HoTen", (object?)obj.HoTen ?? DBNull.Value),
                new SqlParameter("@VaiTro", (object?)obj.VaiTro ?? DBNull.Value)
            };
            return SqlConnectionHelper.ExecuteNonQuery(query, parameters);
        }

        public int Update(User obj)
        {
            string query = @"UPDATE [User] SET TenUser = @TenUser, MatKhau = @MatKhau, 
                            HoTen = @HoTen, VaiTro = @VaiTro WHERE UserId = @UserId";
            SqlParameter[] parameters = {
                new SqlParameter("@UserId", obj.UserId),
                new SqlParameter("@TenUser", (object?)obj.TenUser ?? DBNull.Value),
                new SqlParameter("@MatKhau", (object?)obj.MatKhau ?? DBNull.Value),
                new SqlParameter("@HoTen", (object?)obj.HoTen ?? DBNull.Value),
                new SqlParameter("@VaiTro", (object?)obj.VaiTro ?? DBNull.Value)
            };
            return SqlConnectionHelper.ExecuteNonQuery(query, parameters);
        }

        // Cập nhật profile (không đổi mật khẩu nếu để trống)
        public int UpdateProfile(string userId, string? hoTen, string? tenUser, string? matKhauMoi = null)
        {
            string query;
            SqlParameter[] parameters;

            if (!string.IsNullOrEmpty(matKhauMoi))
            {
                query = "UPDATE [User] SET TenUser = @TenUser, HoTen = @HoTen, MatKhau = @MatKhau WHERE UserId = @UserId";
                parameters = new SqlParameter[] {
                    new SqlParameter("@UserId", userId),
                    new SqlParameter("@TenUser", (object?)tenUser ?? DBNull.Value),
                    new SqlParameter("@HoTen", (object?)hoTen ?? DBNull.Value),
                    new SqlParameter("@MatKhau", matKhauMoi)
                };
            }
            else
            {
                query = "UPDATE [User] SET TenUser = @TenUser, HoTen = @HoTen WHERE UserId = @UserId";
                parameters = new SqlParameter[] {
                    new SqlParameter("@UserId", userId),
                    new SqlParameter("@TenUser", (object?)tenUser ?? DBNull.Value),
                    new SqlParameter("@HoTen", (object?)hoTen ?? DBNull.Value)
                };
            }
            return SqlConnectionHelper.ExecuteNonQuery(query, parameters);
        }

        public int Delete(string userId)
        {
            string query = "DELETE FROM [User] WHERE UserId = @UserId";
            SqlParameter[] parameters = { new SqlParameter("@UserId", userId) };
            return SqlConnectionHelper.ExecuteNonQuery(query, parameters);
        }

        public bool Exists(string userId)
        {
            string query = "SELECT COUNT(*) FROM [User] WHERE UserId = @UserId";
            SqlParameter[] parameters = { new SqlParameter("@UserId", userId) };
            return Convert.ToInt32(SqlConnectionHelper.ExecuteScalar(query, parameters)) > 0;
        }

        // Kiểm tra TenUser đã tồn tại chưa
        public bool ExistsByTenUser(string tenUser, string? excludeUserId = null)
        {
            string query;
            SqlParameter[] parameters;

            if (string.IsNullOrEmpty(excludeUserId))
            {
                query = "SELECT COUNT(*) FROM [User] WHERE TenUser = @TenUser";
                parameters = new SqlParameter[] { new SqlParameter("@TenUser", tenUser) };
            }
            else
            {
                query = "SELECT COUNT(*) FROM [User] WHERE TenUser = @TenUser AND UserId != @UserId";
                parameters = new SqlParameter[] {
                    new SqlParameter("@TenUser", tenUser),
                    new SqlParameter("@UserId", excludeUserId)
                };
            }
            return Convert.ToInt32(SqlConnectionHelper.ExecuteScalar(query, parameters)) > 0;
        }

        // Tìm kiếm và lọc User
        public List<User> Search(string? searchName, string? searchUser, string? vaiTro)
        {
            var conditions = new List<string>();
            var paramList = new List<SqlParameter>();

            if (!string.IsNullOrEmpty(searchName))
            {
                conditions.Add("HoTen LIKE @SearchName");
                paramList.Add(new SqlParameter("@SearchName", $"%{searchName}%"));
            }
            if (!string.IsNullOrEmpty(searchUser))
            {
                conditions.Add("TenUser LIKE @SearchUser");
                paramList.Add(new SqlParameter("@SearchUser", $"%{searchUser}%"));
            }
            if (!string.IsNullOrEmpty(vaiTro))
            {
                conditions.Add("VaiTro = @VaiTro");
                paramList.Add(new SqlParameter("@VaiTro", vaiTro));
            }

            string query = "SELECT * FROM [User]";
            if (conditions.Count > 0)
                query += " WHERE " + string.Join(" AND ", conditions);
            query += " ORDER BY NgayTao DESC";

            return MapDataTableToList(SqlConnectionHelper.ExecuteQuery(query, paramList.ToArray()));
        }

        // Lấy danh sách vai trò distinct
        public List<string> GetDistinctVaiTro()
        {
            string query = "SELECT DISTINCT VaiTro FROM [User] WHERE VaiTro IS NOT NULL ORDER BY VaiTro";
            DataTable dt = SqlConnectionHelper.ExecuteQuery(query);
            var list = new List<string>();
            foreach (DataRow row in dt.Rows)
            {
                if (row["VaiTro"] != DBNull.Value)
                    list.Add(row["VaiTro"].ToString()!);
            }
            return list;
        }

        public int CountDonHang(string userId)
        {
            string query = "SELECT COUNT(*) FROM DON_HANG WHERE USERID = @UserId";
            SqlParameter[] parameters = { new SqlParameter("@UserId", userId) };
            return Convert.ToInt32(SqlConnectionHelper.ExecuteScalar(query, parameters));
        }

        public int CountThanhToan(string userId)
        {
            string query = "SELECT COUNT(*) FROM THANH_TOAN WHERE USERID = @UserId";
            SqlParameter[] parameters = { new SqlParameter("@UserId", userId) };
            return Convert.ToInt32(SqlConnectionHelper.ExecuteScalar(query, parameters));
        }

        public bool HasRelatedData(string userId)
        {
            return CountDonHang(userId) > 0 || CountThanhToan(userId) > 0;
        }

        public int ResetPassword(string userId, string defaultPassword = "123456")
        {
            string query = "UPDATE [User] SET MatKhau = @MatKhau WHERE UserId = @UserId";
            SqlParameter[] parameters = {
                new SqlParameter("@UserId", userId),
                new SqlParameter("@MatKhau", defaultPassword)
            };
            return SqlConnectionHelper.ExecuteNonQuery(query, parameters);
        }

        private List<User> MapDataTableToList(DataTable dt)
        {
            var list = new List<User>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new User
                {
                    UserId = row["UserId"].ToString() ?? "",
                    TenUser = row["TenUser"] != DBNull.Value ? row["TenUser"].ToString() : null,
                    MatKhau = row["MatKhau"] != DBNull.Value ? row["MatKhau"].ToString() : null,
                    HoTen = row["HoTen"] != DBNull.Value ? row["HoTen"].ToString() : null,
                    VaiTro = row["VaiTro"] != DBNull.Value ? row["VaiTro"].ToString() : null
                });
            }
            return list;
        }
    }
}
