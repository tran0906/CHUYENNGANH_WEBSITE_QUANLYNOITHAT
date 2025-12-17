using System.Data;
using Microsoft.Data.SqlClient;
using DOANCHUYENNGANH_WEB_QLNOITHAT.Models;

namespace DOANCHUYENNGANH_WEB_QLNOITHAT.DAL
{
    /// <summary>
    /// Data Access Layer cho CtDonhang - Mô hình 3 lớp
    /// </summary>
    public class CtDonhangDAL
    {
        public List<CtDonhang> GetAll()
        {
            string query = @"SELECT ct.*, sp.TENSP as Tensp, dh.NGAYDAT as Ngaydat 
                            FROM CT_DONHANG ct
                            LEFT JOIN SAN_PHAM sp ON ct.MASP = sp.MASP
                            LEFT JOIN DON_HANG dh ON ct.MADONHANG = dh.MADONHANG
                            ORDER BY ct.MADONHANG";
            return MapDataTableToList(SqlConnectionHelper.ExecuteQuery(query));
        }

        public List<CtDonhang> GetByDonHang(string maDh)
        {
            string query = @"SELECT ct.*, sp.TENSP as Tensp, sp.HINHANH as Hinhanh 
                            FROM CT_DONHANG ct
                            LEFT JOIN SAN_PHAM sp ON ct.MASP = sp.MASP
                            WHERE ct.MADONHANG = @MaDh";
            SqlParameter[] parameters = { new SqlParameter("@MaDh", maDh) };
            return MapDataTableToList(SqlConnectionHelper.ExecuteQuery(query, parameters));
        }

        public CtDonhang? GetById(string maSp, string maDh)
        {
            string query = @"SELECT ct.*, sp.TENSP as Tensp FROM CT_DONHANG ct
                            LEFT JOIN SAN_PHAM sp ON ct.MASP = sp.MASP
                            WHERE ct.MASP = @MaSp AND ct.MADONHANG = @MaDh";
            SqlParameter[] parameters = { 
                new SqlParameter("@MaSp", maSp), 
                new SqlParameter("@MaDh", maDh) 
            };
            return MapDataTableToList(SqlConnectionHelper.ExecuteQuery(query, parameters)).FirstOrDefault();
        }

        public int Insert(CtDonhang obj)
        {
            string query = @"INSERT INTO CT_DONHANG (MASP, MADONHANG, SOLUONG, DONGIA, THANHTIEN) 
                            VALUES (@Masp, @Madonhang, @Soluong, @Dongia, @Thanhtien)";
            SqlParameter[] parameters = {
                new SqlParameter("@Masp", obj.Masp),
                new SqlParameter("@Madonhang", obj.Madonhang),
                new SqlParameter("@Soluong", (object?)obj.Soluong ?? DBNull.Value),
                new SqlParameter("@Dongia", (object?)obj.Dongia ?? DBNull.Value),
                new SqlParameter("@Thanhtien", (object?)obj.Thanhtien ?? DBNull.Value)
            };
            return SqlConnectionHelper.ExecuteNonQuery(query, parameters);
        }

        public int Update(CtDonhang obj)
        {
            string query = @"UPDATE CT_DONHANG SET SOLUONG=@Soluong, DONGIA=@Dongia, THANHTIEN=@Thanhtien 
                            WHERE MASP=@Masp AND MADONHANG=@Madonhang";
            SqlParameter[] parameters = {
                new SqlParameter("@Masp", obj.Masp),
                new SqlParameter("@Madonhang", obj.Madonhang),
                new SqlParameter("@Soluong", (object?)obj.Soluong ?? DBNull.Value),
                new SqlParameter("@Dongia", (object?)obj.Dongia ?? DBNull.Value),
                new SqlParameter("@Thanhtien", (object?)obj.Thanhtien ?? DBNull.Value)
            };
            return SqlConnectionHelper.ExecuteNonQuery(query, parameters);
        }

        public int Delete(string maSp, string maDh)
        {
            string query = "DELETE FROM CT_DONHANG WHERE MASP=@MaSp AND MADONHANG=@MaDh";
            SqlParameter[] parameters = { new SqlParameter("@MaSp", maSp), new SqlParameter("@MaDh", maDh) };
            return SqlConnectionHelper.ExecuteNonQuery(query, parameters);
        }

        public int DeleteByDonHang(string maDh)
        {
            string query = "DELETE FROM CT_DONHANG WHERE MADONHANG=@MaDh";
            SqlParameter[] parameters = { new SqlParameter("@MaDh", maDh) };
            return SqlConnectionHelper.ExecuteNonQuery(query, parameters);
        }

        public decimal GetTongTienByDonHang(string maDh)
        {
            string query = "SELECT ISNULL(SUM(THANHTIEN), 0) FROM CT_DONHANG WHERE MADONHANG=@MaDh";
            SqlParameter[] parameters = { new SqlParameter("@MaDh", maDh) };
            return Convert.ToDecimal(SqlConnectionHelper.ExecuteScalar(query, parameters));
        }

        private List<CtDonhang> MapDataTableToList(DataTable dt)
        {
            var list = new List<CtDonhang>();
            foreach (DataRow row in dt.Rows)
            {
                var ct = new CtDonhang
                {
                    Masp = row["MASP"].ToString() ?? "",
                    Madonhang = row["MADONHANG"].ToString() ?? "",
                    Soluong = row["SOLUONG"] != DBNull.Value ? Convert.ToInt32(row["SOLUONG"]) : null,
                    Dongia = row["DONGIA"] != DBNull.Value ? Convert.ToDecimal(row["DONGIA"]) : null,
                    Thanhtien = row["THANHTIEN"] != DBNull.Value ? Convert.ToDecimal(row["THANHTIEN"]) : null
                };

                if (dt.Columns.Contains("Tensp") && row["Tensp"] != DBNull.Value)
                {
                    ct.MaspNavigation = new SanPham 
                    { 
                        Masp = ct.Masp, 
                        Tensp = row["Tensp"].ToString(),
                        Hinhanh = dt.Columns.Contains("Hinhanh") && row["Hinhanh"] != DBNull.Value ? row["Hinhanh"].ToString() : null
                    };
                }
                list.Add(ct);
            }
            return list;
        }
    }
}
