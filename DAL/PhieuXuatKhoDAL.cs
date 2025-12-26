using System.Data;
using Microsoft.Data.SqlClient;
using DOANCHUYENNGANH_WEB_QLNOITHAT.Models;

namespace DOANCHUYENNGANH_WEB_QLNOITHAT.DAL
{
    /// <summary>
    /// Data Access Layer cho PhieuXuatKho - Mô hình 3 lớp
    /// </summary>
    public class PhieuXuatKhoDAL
    {
        public List<PhieuXuatKho> GetAll()
        {
            string query = @"SELECT px.*, u.HoTen as TenNguoiTao, dh.NGAYDAT as Ngaydat
                            FROM PHIEU_XUAT_KHO px
                            LEFT JOIN [User] u ON px.USERID = u.UserId
                            LEFT JOIN DON_HANG dh ON px.MADONHANG = dh.MADONHANG
                            ORDER BY px.NGAYXUAT DESC";
            return MapDataTableToList(SqlConnectionHelper.ExecuteQuery(query));
        }

        public PhieuXuatKho? GetById(string ma)
        {
            string query = @"SELECT px.*, u.HoTen as TenNguoiTao
                            FROM PHIEU_XUAT_KHO px
                            LEFT JOIN [User] u ON px.USERID = u.UserId
                            WHERE px.MAPHIEUXUAT = @Ma";
            SqlParameter[] parameters = { new SqlParameter("@Ma", ma) };
            return MapDataTableToList(SqlConnectionHelper.ExecuteQuery(query, parameters)).FirstOrDefault();
        }

        public int Insert(PhieuXuatKho obj)
        {
            string query = @"INSERT INTO PHIEU_XUAT_KHO (MAPHIEUXUAT, USERID, MADONHANG, NGAYXUAT, MANVDUYET) 
                            VALUES (@Maphieuxuat, @Userid, @Madonhang, @Ngayxuat, @Manvduyet)";
            SqlParameter[] parameters = {
                new SqlParameter("@Maphieuxuat", obj.Maphieuxuat),
                new SqlParameter("@Userid", obj.Userid),
                new SqlParameter("@Madonhang", obj.Madonhang),
                new SqlParameter("@Ngayxuat", (object?)obj.Ngayxuat ?? DBNull.Value),
                new SqlParameter("@Manvduyet", (object?)obj.Manvduyet ?? DBNull.Value)
            };
            return SqlConnectionHelper.ExecuteNonQuery(query, parameters);
        }

        public int Update(PhieuXuatKho obj)
        {
            string query = @"UPDATE PHIEU_XUAT_KHO SET USERID=@Userid, MADONHANG=@Madonhang, 
                            NGAYXUAT=@Ngayxuat, MANVDUYET=@Manvduyet WHERE MAPHIEUXUAT=@Maphieuxuat";
            SqlParameter[] parameters = {
                new SqlParameter("@Maphieuxuat", obj.Maphieuxuat),
                new SqlParameter("@Userid", obj.Userid),
                new SqlParameter("@Madonhang", obj.Madonhang),
                new SqlParameter("@Ngayxuat", (object?)obj.Ngayxuat ?? DBNull.Value),
                new SqlParameter("@Manvduyet", (object?)obj.Manvduyet ?? DBNull.Value)
            };
            return SqlConnectionHelper.ExecuteNonQuery(query, parameters);
        }

        public int Delete(string ma)
        {
            string query = "DELETE FROM PHIEU_XUAT_KHO WHERE MAPHIEUXUAT=@Ma";
            SqlParameter[] parameters = { new SqlParameter("@Ma", ma) };
            return SqlConnectionHelper.ExecuteNonQuery(query, parameters);
        }

        public bool Exists(string ma)
        {
            string query = "SELECT COUNT(*) FROM PHIEU_XUAT_KHO WHERE MAPHIEUXUAT=@Ma";
            SqlParameter[] parameters = { new SqlParameter("@Ma", ma) };
            return Convert.ToInt32(SqlConnectionHelper.ExecuteScalar(query, parameters)) > 0;
        }

        public PhieuXuatKho? GetByDonHang(string maDonHang)
        {
            string query = @"SELECT px.*, u.HoTen as TenNguoiTao
                            FROM PHIEU_XUAT_KHO px
                            LEFT JOIN [User] u ON px.USERID = u.UserId
                            WHERE px.MADONHANG = @MaDonHang";
            SqlParameter[] parameters = { new SqlParameter("@MaDonHang", maDonHang) };
            return MapDataTableToList(SqlConnectionHelper.ExecuteQuery(query, parameters)).FirstOrDefault();
        }

        public string GenerateNewId()
        {
            var result = SqlConnectionHelper.ExecuteScalar("SELECT MAX(MAPHIEUXUAT) FROM PHIEU_XUAT_KHO WHERE MAPHIEUXUAT LIKE 'PX%'");
            if (result == null || result == DBNull.Value) return "PX001";
            
            string maxId = result.ToString() ?? "";
            // Chỉ lấy phần số sau "PX"
            if (maxId.Length > 2 && maxId.StartsWith("PX"))
            {
                string numPart = maxId.Substring(2);
                if (int.TryParse(numPart, out int num))
                {
                    return $"PX{(num + 1):D3}";
                }
            }
            
            // Fallback: đếm số lượng record + 1
            var countResult = SqlConnectionHelper.ExecuteScalar("SELECT COUNT(*) FROM PHIEU_XUAT_KHO");
            int count = countResult != null && countResult != DBNull.Value ? Convert.ToInt32(countResult) : 0;
            return $"PX{(count + 1):D3}";
        }

        private List<PhieuXuatKho> MapDataTableToList(DataTable dt)
        {
            var list = new List<PhieuXuatKho>();
            foreach (DataRow row in dt.Rows)
            {
                var px = new PhieuXuatKho
                {
                    Maphieuxuat = row["MAPHIEUXUAT"].ToString() ?? "",
                    Userid = row["USERID"].ToString() ?? "",
                    Madonhang = row["MADONHANG"].ToString() ?? "",
                    Ngayxuat = row["NGAYXUAT"] != DBNull.Value ? Convert.ToDateTime(row["NGAYXUAT"]) : null,
                    Manvduyet = row["MANVDUYET"] != DBNull.Value ? row["MANVDUYET"].ToString() : null
                };

                if (dt.Columns.Contains("TenNguoiTao") && row["TenNguoiTao"] != DBNull.Value)
                    px.User = new User { UserId = px.Userid, HoTen = row["TenNguoiTao"].ToString() };

                list.Add(px);
            }
            return list;
        }
    }
}
