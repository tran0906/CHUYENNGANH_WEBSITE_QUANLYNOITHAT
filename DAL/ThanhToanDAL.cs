using System.Data;
using Microsoft.Data.SqlClient;
using DOANCHUYENNGANH_WEB_QLNOITHAT.Models;

namespace DOANCHUYENNGANH_WEB_QLNOITHAT.DAL
{
    /// <summary>
    /// Data Access Layer cho ThanhToan - Mô hình 3 lớp
    /// </summary>
    public class ThanhToanDAL
    {
        public List<ThanhToan> GetAll()
        {
            string query = @"SELECT tt.*, u.HoTen as TenNguoiTao, dh.NGAYDAT as Ngaydat
                            FROM THANH_TOAN tt
                            LEFT JOIN [User] u ON tt.USERID = u.UserId
                            LEFT JOIN DON_HANG dh ON tt.MADONHANG = dh.MADONHANG
                            ORDER BY tt.NGAYTHANHTOAN DESC";
            return MapDataTableToList(SqlConnectionHelper.ExecuteQuery(query));
        }

        public List<ThanhToan> GetByDonHang(string maDh)
        {
            string query = @"SELECT tt.*, u.HoTen as TenNguoiTao
                            FROM THANH_TOAN tt
                            LEFT JOIN [User] u ON tt.USERID = u.UserId
                            WHERE tt.MADONHANG = @MaDh";
            SqlParameter[] parameters = { new SqlParameter("@MaDh", maDh) };
            return MapDataTableToList(SqlConnectionHelper.ExecuteQuery(query, parameters));
        }

        public ThanhToan? GetById(string ma)
        {
            string query = @"SELECT tt.*, u.HoTen as TenNguoiTao
                            FROM THANH_TOAN tt
                            LEFT JOIN [User] u ON tt.USERID = u.UserId
                            WHERE tt.MATHANHTOAN = @Ma";
            SqlParameter[] parameters = { new SqlParameter("@Ma", ma) };
            return MapDataTableToList(SqlConnectionHelper.ExecuteQuery(query, parameters)).FirstOrDefault();
        }

        public int Insert(ThanhToan obj)
        {
            string query = @"INSERT INTO THANH_TOAN (MATHANHTOAN, MADONHANG, USERID, SOTIEN, PHUONGTHUC, MANVDUYET, NGAYTHANHTOAN) 
                            VALUES (@Mathanhtoan, @Madonhang, @Userid, @Sotien, @Phuongthuc, @Manvduyet, @Ngaythanhtoan)";
            SqlParameter[] parameters = {
                new SqlParameter("@Mathanhtoan", obj.Mathanhtoan),
                new SqlParameter("@Madonhang", obj.Madonhang),
                new SqlParameter("@Userid", obj.Userid),
                new SqlParameter("@Sotien", (object?)obj.Sotien ?? DBNull.Value),
                new SqlParameter("@Phuongthuc", (object?)obj.Phuongthuc ?? DBNull.Value),
                new SqlParameter("@Manvduyet", (object?)obj.Manvduyet ?? DBNull.Value),
                new SqlParameter("@Ngaythanhtoan", (object?)obj.Ngaythanhtoan ?? DBNull.Value)
            };
            return SqlConnectionHelper.ExecuteNonQuery(query, parameters);
        }

        public int Update(ThanhToan obj)
        {
            string query = @"UPDATE THANH_TOAN SET MADONHANG=@Madonhang, USERID=@Userid, SOTIEN=@Sotien, 
                            PHUONGTHUC=@Phuongthuc, MANVDUYET=@Manvduyet, NGAYTHANHTOAN=@Ngaythanhtoan 
                            WHERE MATHANHTOAN=@Mathanhtoan";
            SqlParameter[] parameters = {
                new SqlParameter("@Mathanhtoan", obj.Mathanhtoan),
                new SqlParameter("@Madonhang", obj.Madonhang),
                new SqlParameter("@Userid", obj.Userid),
                new SqlParameter("@Sotien", (object?)obj.Sotien ?? DBNull.Value),
                new SqlParameter("@Phuongthuc", (object?)obj.Phuongthuc ?? DBNull.Value),
                new SqlParameter("@Manvduyet", (object?)obj.Manvduyet ?? DBNull.Value),
                new SqlParameter("@Ngaythanhtoan", (object?)obj.Ngaythanhtoan ?? DBNull.Value)
            };
            return SqlConnectionHelper.ExecuteNonQuery(query, parameters);
        }

        public int Delete(string ma)
        {
            string query = "DELETE FROM THANH_TOAN WHERE MATHANHTOAN=@Ma";
            SqlParameter[] parameters = { new SqlParameter("@Ma", ma) };
            return SqlConnectionHelper.ExecuteNonQuery(query, parameters);
        }

        public bool Exists(string ma)
        {
            string query = "SELECT COUNT(*) FROM THANH_TOAN WHERE MATHANHTOAN=@Ma";
            SqlParameter[] parameters = { new SqlParameter("@Ma", ma) };
            return Convert.ToInt32(SqlConnectionHelper.ExecuteScalar(query, parameters)) > 0;
        }

        public string GenerateNewId()
        {
            var result = SqlConnectionHelper.ExecuteScalar("SELECT MAX(MATHANHTOAN) FROM THANH_TOAN WHERE MATHANHTOAN LIKE 'TT%'");
            if (result == null || result == DBNull.Value) return "TT001";
            int num = int.Parse(result.ToString()!.Substring(2)) + 1;
            return $"TT{num:D3}";
        }

        private List<ThanhToan> MapDataTableToList(DataTable dt)
        {
            var list = new List<ThanhToan>();
            foreach (DataRow row in dt.Rows)
            {
                var tt = new ThanhToan
                {
                    Mathanhtoan = row["MATHANHTOAN"].ToString() ?? "",
                    Madonhang = row["MADONHANG"].ToString() ?? "",
                    Userid = row["USERID"].ToString() ?? "",
                    Sotien = row["SOTIEN"] != DBNull.Value ? Convert.ToDecimal(row["SOTIEN"]) : null,
                    Phuongthuc = row["PHUONGTHUC"] != DBNull.Value ? row["PHUONGTHUC"].ToString() : null,
                    Manvduyet = row["MANVDUYET"] != DBNull.Value ? row["MANVDUYET"].ToString() : null,
                    Ngaythanhtoan = row["NGAYTHANHTOAN"] != DBNull.Value ? Convert.ToDateTime(row["NGAYTHANHTOAN"]) : null
                };

                if (dt.Columns.Contains("TenNguoiTao") && row["TenNguoiTao"] != DBNull.Value)
                    tt.User = new User { UserId = tt.Userid, HoTen = row["TenNguoiTao"].ToString() };

                list.Add(tt);
            }
            return list;
        }
    }
}
