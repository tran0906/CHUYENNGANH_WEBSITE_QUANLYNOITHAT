// FILE: DAL/KhachHangDAL.cs - Truy cập dữ liệu bảng KHACH_HANG

using System.Data;
using Microsoft.Data.SqlClient;
using DOANCHUYENNGANH_WEB_QLNOITHAT.Models;

namespace DOANCHUYENNGANH_WEB_QLNOITHAT.DAL
{
    public class KhachHangDAL
    {
        // Lấy tất cả khách hàng
        public List<KhachHang> GetAll()
        {
            string query = "SELECT * FROM KHACH_HANG ORDER BY MAKH";
            return MapDataTableToList(SqlConnectionHelper.ExecuteQuery(query));
        }

        // Tìm kiếm khách hàng theo mã, tên hoặc SĐT
        public List<KhachHang> Search(string? search)
        {
            string query = "SELECT * FROM KHACH_HANG WHERE 1=1";
            var parameters = new List<SqlParameter>();

            if (!string.IsNullOrEmpty(search))
            {
                query += " AND (MAKH LIKE @Search OR HOTENKH LIKE @Search OR SDTKH LIKE @Search)";
                parameters.Add(new SqlParameter("@Search", $"%{search}%")); // Tìm kiếm gần đúng
            }
            query += " ORDER BY MAKH";
            return MapDataTableToList(SqlConnectionHelper.ExecuteQuery(query, parameters.ToArray()));
        }

        // Lấy 1 khách hàng theo mã
        public KhachHang? GetById(string maKh)
        {
            string query = "SELECT * FROM KHACH_HANG WHERE MAKH = @MaKh";
            SqlParameter[] parameters = { new SqlParameter("@MaKh", maKh) };
            return MapDataTableToList(SqlConnectionHelper.ExecuteQuery(query, parameters)).FirstOrDefault();
        }

        // Kiểm tra đăng nhập bằng SĐT và mật khẩu
        public KhachHang? Login(string sdt, string matKhau)
        {
            string query = "SELECT * FROM KHACH_HANG WHERE SDTKH = @Sdt AND Matkhau = @MatKhau";
            SqlParameter[] parameters = {
                new SqlParameter("@Sdt", sdt),
                new SqlParameter("@MatKhau", matKhau)
            };
            return MapDataTableToList(SqlConnectionHelper.ExecuteQuery(query, parameters)).FirstOrDefault();
        }

        // Thêm khách hàng mới (đăng ký tài khoản)
        public int Insert(KhachHang obj)
        {
            string query = @"INSERT INTO KHACH_HANG (MAKH, HOTENKH, DIACHIKH, SDTKH, Matkhau) 
                            VALUES (@Makh, @Hotenkh, @Diachikh, @Sdtkh, @Matkhau)";
            SqlParameter[] parameters = {
                new SqlParameter("@Makh", obj.Makh),
                new SqlParameter("@Hotenkh", (object?)obj.Hotenkh ?? DBNull.Value), // Nếu null thì gán DBNull
                new SqlParameter("@Diachikh", (object?)obj.Diachikh ?? DBNull.Value),
                new SqlParameter("@Sdtkh", (object?)obj.Sdtkh ?? DBNull.Value),
                new SqlParameter("@Matkhau", (object?)obj.Matkhau ?? DBNull.Value)
            };
            return SqlConnectionHelper.ExecuteNonQuery(query, parameters);
        }

        // Cập nhật thông tin khách hàng
        public int Update(KhachHang obj)
        {
            string query = @"UPDATE KHACH_HANG SET HOTENKH = @Hotenkh, DIACHIKH = @Diachikh, 
                            SDTKH = @Sdtkh, Matkhau = @Matkhau WHERE MAKH = @Makh";
            SqlParameter[] parameters = {
                new SqlParameter("@Makh", obj.Makh),
                new SqlParameter("@Hotenkh", (object?)obj.Hotenkh ?? DBNull.Value),
                new SqlParameter("@Diachikh", (object?)obj.Diachikh ?? DBNull.Value),
                new SqlParameter("@Sdtkh", (object?)obj.Sdtkh ?? DBNull.Value),
                new SqlParameter("@Matkhau", (object?)obj.Matkhau ?? DBNull.Value)
            };
            return SqlConnectionHelper.ExecuteNonQuery(query, parameters);
        }

        // Xóa khách hàng theo mã
        public int Delete(string maKh)
        {
            string query = "DELETE FROM KHACH_HANG WHERE MAKH = @MaKh";
            SqlParameter[] parameters = { new SqlParameter("@MaKh", maKh) };
            return SqlConnectionHelper.ExecuteNonQuery(query, parameters);
        }

        // Kiểm tra mã khách hàng đã tồn tại chưa
        public bool Exists(string maKh)
        {
            string query = "SELECT COUNT(*) FROM KHACH_HANG WHERE MAKH = @MaKh";
            SqlParameter[] parameters = { new SqlParameter("@MaKh", maKh) };
            return Convert.ToInt32(SqlConnectionHelper.ExecuteScalar(query, parameters)) > 0;
        }

        // Kiểm tra SĐT đã đăng ký chưa (dùng khi đăng ký)
        public bool ExistsBySdt(string sdt)
        {
            string query = "SELECT COUNT(*) FROM KHACH_HANG WHERE SDTKH = @Sdt";
            SqlParameter[] parameters = { new SqlParameter("@Sdt", sdt) };
            return Convert.ToInt32(SqlConnectionHelper.ExecuteScalar(query, parameters)) > 0;
        }

        // Đếm tổng số khách hàng
        public int Count()
        {
            return Convert.ToInt32(SqlConnectionHelper.ExecuteScalar("SELECT COUNT(*) FROM KHACH_HANG"));
        }

        // Chuyển DataTable thành List<KhachHang>
        private List<KhachHang> MapDataTableToList(DataTable dt)
        {
            var list = new List<KhachHang>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new KhachHang
                {
                    Makh = row["MAKH"].ToString() ?? "",
                    Hotenkh = row["HOTENKH"] != DBNull.Value ? row["HOTENKH"].ToString() : null,
                    Diachikh = row["DIACHIKH"] != DBNull.Value ? row["DIACHIKH"].ToString() : null,
                    Sdtkh = row["SDTKH"] != DBNull.Value ? row["SDTKH"].ToString() : null,
                    Matkhau = row["Matkhau"] != DBNull.Value ? row["Matkhau"].ToString() : null
                });
            }
            return list;
        }
    }
}
