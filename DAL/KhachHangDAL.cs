using System.Data;
using Microsoft.Data.SqlClient;
using DOANCHUYENNGANH_WEB_QLNOITHAT.Models;

namespace DOANCHUYENNGANH_WEB_QLNOITHAT.DAL
{
    /// <summary>
    /// Data Access Layer cho KhachHang - Mô hình 3 lớp
    /// </summary>
    public class KhachHangDAL
    {
        public List<KhachHang> GetAll()
        {
            string query = "SELECT * FROM KHACH_HANG ORDER BY MAKH";
            return MapDataTableToList(SqlConnectionHelper.ExecuteQuery(query));
        }

        public List<KhachHang> Search(string? maKh, string? hoTen, string? sdt)
        {
            string query = "SELECT * FROM KHACH_HANG WHERE 1=1";
            var parameters = new List<SqlParameter>();

            if (!string.IsNullOrEmpty(maKh))
            {
                query += " AND MAKH LIKE @MaKh";
                parameters.Add(new SqlParameter("@MaKh", $"%{maKh}%"));
            }
            if (!string.IsNullOrEmpty(hoTen))
            {
                query += " AND HOTENKH LIKE @HoTen";
                parameters.Add(new SqlParameter("@HoTen", $"%{hoTen}%"));
            }
            if (!string.IsNullOrEmpty(sdt))
            {
                query += " AND SDTKH LIKE @Sdt";
                parameters.Add(new SqlParameter("@Sdt", $"%{sdt}%"));
            }
            query += " ORDER BY MAKH";
            return MapDataTableToList(SqlConnectionHelper.ExecuteQuery(query, parameters.ToArray()));
        }

        public KhachHang? GetById(string maKh)
        {
            string query = "SELECT * FROM KHACH_HANG WHERE MAKH = @MaKh";
            SqlParameter[] parameters = { new SqlParameter("@MaKh", maKh) };
            return MapDataTableToList(SqlConnectionHelper.ExecuteQuery(query, parameters)).FirstOrDefault();
        }

        public KhachHang? Login(string sdt, string matKhau)
        {
            string query = "SELECT * FROM KHACH_HANG WHERE SDTKH = @Sdt AND Matkhau = @MatKhau";
            SqlParameter[] parameters = {
                new SqlParameter("@Sdt", sdt),
                new SqlParameter("@MatKhau", matKhau)
            };
            return MapDataTableToList(SqlConnectionHelper.ExecuteQuery(query, parameters)).FirstOrDefault();
        }

        public int Insert(KhachHang obj)
        {
            string query = @"INSERT INTO KHACH_HANG (MAKH, HOTENKH, DIACHIKH, SDTKH, Matkhau) 
                            VALUES (@Makh, @Hotenkh, @Diachikh, @Sdtkh, @Matkhau)";
            SqlParameter[] parameters = {
                new SqlParameter("@Makh", obj.Makh),
                new SqlParameter("@Hotenkh", (object?)obj.Hotenkh ?? DBNull.Value),
                new SqlParameter("@Diachikh", (object?)obj.Diachikh ?? DBNull.Value),
                new SqlParameter("@Sdtkh", (object?)obj.Sdtkh ?? DBNull.Value),
                new SqlParameter("@Matkhau", (object?)obj.Matkhau ?? DBNull.Value)
            };
            return SqlConnectionHelper.ExecuteNonQuery(query, parameters);
        }

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

        public int Delete(string maKh)
        {
            string query = "DELETE FROM KHACH_HANG WHERE MAKH = @MaKh";
            SqlParameter[] parameters = { new SqlParameter("@MaKh", maKh) };
            return SqlConnectionHelper.ExecuteNonQuery(query, parameters);
        }

        public bool Exists(string maKh)
        {
            string query = "SELECT COUNT(*) FROM KHACH_HANG WHERE MAKH = @MaKh";
            SqlParameter[] parameters = { new SqlParameter("@MaKh", maKh) };
            return Convert.ToInt32(SqlConnectionHelper.ExecuteScalar(query, parameters)) > 0;
        }

        public bool ExistsBySdt(string sdt)
        {
            string query = "SELECT COUNT(*) FROM KHACH_HANG WHERE SDTKH = @Sdt";
            SqlParameter[] parameters = { new SqlParameter("@Sdt", sdt) };
            return Convert.ToInt32(SqlConnectionHelper.ExecuteScalar(query, parameters)) > 0;
        }

        public int Count()
        {
            return Convert.ToInt32(SqlConnectionHelper.ExecuteScalar("SELECT COUNT(*) FROM KHACH_HANG"));
        }

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
