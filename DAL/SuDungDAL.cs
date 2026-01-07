using System.Data;
using Microsoft.Data.SqlClient;
using DOANCHUYENNGANH_WEB_QLNOITHAT.Models;

namespace DOANCHUYENNGANH_WEB_QLNOITHAT.DAL
{
    // Data Access Layer cho SuDung (bảng liên kết) - Mô hình 3 lớp
    public class SuDungDAL
    {
        // Lấy danh sách sản phẩm theo mục đích sử dụng
        public List<SanPham> GetSanPhamByMdsd(string mamdsd)
        {
            string query = @"SELECT sp.*, nsp.TENNHOMSP 
                            FROM SAN_PHAM sp
                            INNER JOIN SUDUNG sd ON sp.MASP = sd.MASP
                            LEFT JOIN NHOM_SAN_PHAM nsp ON sp.MANHOMSP = nsp.MANHOMSP
                            WHERE sd.MAMDSD = @Mamdsd";
            SqlParameter[] parameters = { new SqlParameter("@Mamdsd", mamdsd) };
            return MapSanPhamListWithNhom(SqlConnectionHelper.ExecuteQuery(query, parameters));
        }

        // Kiểm tra liên kết SP-mục đích sử dụng tồn tại
        public bool Exists(string mamdsd, string masp)
        {
            string query = "SELECT COUNT(*) FROM SUDUNG WHERE MAMDSD = @Mamdsd AND MASP = @Masp";
            SqlParameter[] parameters = {
                new SqlParameter("@Mamdsd", mamdsd),
                new SqlParameter("@Masp", masp)
            };
            return Convert.ToInt32(SqlConnectionHelper.ExecuteScalar(query, parameters)) > 0;
        }

        // Thêm liên kết SP-mục đích sử dụng
        public int Insert(string mamdsd, string masp)
        {
            string query = "INSERT INTO SUDUNG (MAMDSD, MASP) VALUES (@Mamdsd, @Masp)";
            SqlParameter[] parameters = {
                new SqlParameter("@Mamdsd", mamdsd),
                new SqlParameter("@Masp", masp)
            };
            return SqlConnectionHelper.ExecuteNonQuery(query, parameters);
        }

        // Xóa liên kết SP-mục đích sử dụng
        public int Delete(string mamdsd, string masp)
        {
            string query = "DELETE FROM SUDUNG WHERE MAMDSD = @Mamdsd AND MASP = @Masp";
            SqlParameter[] parameters = {
                new SqlParameter("@Mamdsd", mamdsd),
                new SqlParameter("@Masp", masp)
            };
            return SqlConnectionHelper.ExecuteNonQuery(query, parameters);
        }

        // Chuyển DataTable thành List<SanPham> (có thông tin nhóm SP)
        private List<SanPham> MapSanPhamListWithNhom(DataTable dt)
        {
            var list = new List<SanPham>();
            foreach (DataRow row in dt.Rows)
            {
                var sp = new SanPham
                {
                    Masp = row["MASP"].ToString() ?? "",
                    Tensp = row["TENSP"] != DBNull.Value ? row["TENSP"].ToString() : null,
                    Mota = row["MOTA"] != DBNull.Value ? row["MOTA"].ToString() : null,
                    Giaban = row["GIABAN"] != DBNull.Value ? Convert.ToDecimal(row["GIABAN"]) : null,
                    Soluongton = row["SOLUONGTON"] != DBNull.Value ? Convert.ToInt32(row["SOLUONGTON"]) : null,
                    Hinhanh = row["HINHANH"] != DBNull.Value ? row["HINHANH"].ToString() : null,
                    Manhomsp = row["MANHOMSP"] != DBNull.Value ? row["MANHOMSP"].ToString() : null
                };
                if (row.Table.Columns.Contains("TENNHOMSP") && row["TENNHOMSP"] != DBNull.Value)
                {
                    sp.ManhomspNavigation = new NhomSanPham
                    {
                        Manhomsp = sp.Manhomsp ?? "",
                        Tennhomsp = row["TENNHOMSP"].ToString()
                    };
                }
                list.Add(sp);
            }
            return list;
        }
    }
}
