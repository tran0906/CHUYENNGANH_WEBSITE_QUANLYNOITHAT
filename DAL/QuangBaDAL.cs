// FILE: DAL/QuangBaDAL.cs
// TẦNG DAL - Truy cập dữ liệu bảng QUANGBA (SP - Đợt giảm giá)
// LUỒNG: BLL → DAL → SqlConnectionHelper → Database

using System.Data;
using Microsoft.Data.SqlClient;
using DOANCHUYENNGANH_WEB_QLNOITHAT.Models;

namespace DOANCHUYENNGANH_WEB_QLNOITHAT.DAL
{
    public class QuangBaDAL
    {
        // Lấy danh sách sản phẩm theo đợt giảm giá
        public List<SanPham> GetSanPhamByDotGiamGia(string madotgiamgia)
        {
            string query = @"SELECT sp.* FROM SAN_PHAM sp
                            INNER JOIN QUANGBA qb ON sp.MASP = qb.MASP
                            WHERE qb.MADOTGIAMGIA = @Madotgiamgia";
            SqlParameter[] parameters = { new SqlParameter("@Madotgiamgia", madotgiamgia) };
            return MapSanPhamList(SqlConnectionHelper.ExecuteQuery(query, parameters));
        }

        // Lấy danh sách đợt giảm giá theo mã sản phẩm
        public List<QuanBaSp> GetDotGiamGiaByMasp(string masp)
        {
            string query = @"SELECT qbs.* FROM QUAN_BA_SP qbs
                            INNER JOIN QUANGBA qb ON qbs.MADOTGIAMGIA = qb.MADOTGIAMGIA
                            WHERE qb.MASP = @Masp";
            SqlParameter[] parameters = { new SqlParameter("@Masp", masp) };
            return MapQuanBaSpList(SqlConnectionHelper.ExecuteQuery(query, parameters));
        }

        // Kiểm tra liên kết SP-đợt giảm giá tồn tại
        public bool Exists(string masp, string madotgiamgia)
        {
            string query = "SELECT COUNT(*) FROM QUANGBA WHERE MASP = @Masp AND MADOTGIAMGIA = @Madotgiamgia";
            SqlParameter[] parameters = {
                new SqlParameter("@Masp", masp),
                new SqlParameter("@Madotgiamgia", madotgiamgia)
            };
            return Convert.ToInt32(SqlConnectionHelper.ExecuteScalar(query, parameters)) > 0;
        }

        // Thêm liên kết SP vào đợt giảm giá
        public int Insert(string masp, string madotgiamgia)
        {
            string query = "INSERT INTO QUANGBA (MASP, MADOTGIAMGIA) VALUES (@Masp, @Madotgiamgia)";
            SqlParameter[] parameters = {
                new SqlParameter("@Masp", masp),
                new SqlParameter("@Madotgiamgia", madotgiamgia)
            };
            return SqlConnectionHelper.ExecuteNonQuery(query, parameters);
        }

        // Xóa liên kết SP khỏi đợt giảm giá
        public int Delete(string masp, string madotgiamgia)
        {
            string query = "DELETE FROM QUANGBA WHERE MASP = @Masp AND MADOTGIAMGIA = @Madotgiamgia";
            SqlParameter[] parameters = {
                new SqlParameter("@Masp", masp),
                new SqlParameter("@Madotgiamgia", madotgiamgia)
            };
            return SqlConnectionHelper.ExecuteNonQuery(query, parameters);
        }

        // Xóa tất cả SP trong đợt giảm giá
        public int DeleteByDotGiamGia(string madotgiamgia)
        {
            string query = "DELETE FROM QUANGBA WHERE MADOTGIAMGIA = @Madotgiamgia";
            SqlParameter[] parameters = { new SqlParameter("@Madotgiamgia", madotgiamgia) };
            return SqlConnectionHelper.ExecuteNonQuery(query, parameters);
        }

        // Lấy % giảm giá của SP nếu đang trong đợt quảng bá
        public int? GetPhanTramGiam(string masp)
        {
            string query = @"SELECT TOP 1 qbs.PHANTRAMGIAM 
                            FROM QUAN_BA_SP qbs
                            INNER JOIN QUANGBA qb ON qbs.MADOTGIAMGIA = qb.MADOTGIAMGIA
                            WHERE qb.MASP = @Masp 
                            AND qbs.NGAYBATDAU <= GETDATE() 
                            AND qbs.NGAYKETTHUC >= GETDATE()
                            AND qbs.PHANTRAMGIAM > 0
                            ORDER BY qbs.PHANTRAMGIAM DESC";
            SqlParameter[] parameters = { new SqlParameter("@Masp", masp) };
            var result = SqlConnectionHelper.ExecuteScalar(query, parameters);
            return result != null && result != DBNull.Value ? Convert.ToInt32(result) : null;
        }

        // Lấy danh sách tất cả SP đang được quảng bá
        public Dictionary<string, int> GetAllPromotedProducts()
        {
            string query = @"SELECT qb.MASP, qbs.PHANTRAMGIAM 
                            FROM QUANGBA qb
                            INNER JOIN QUAN_BA_SP qbs ON qb.MADOTGIAMGIA = qbs.MADOTGIAMGIA
                            WHERE qbs.NGAYBATDAU <= GETDATE() 
                            AND qbs.NGAYKETTHUC >= GETDATE()
                            AND qbs.PHANTRAMGIAM > 0";
            var dt = SqlConnectionHelper.ExecuteQuery(query);
            var dict = new Dictionary<string, int>();
            foreach (DataRow row in dt.Rows)
            {
                var masp = row["MASP"].ToString() ?? "";
                var phantram = Convert.ToInt32(row["PHANTRAMGIAM"]);
                if (!dict.ContainsKey(masp) || dict[masp] < phantram)
                    dict[masp] = phantram;
            }
            return dict;
        }

        // Chuyển DataTable thành List<SanPham>
        private List<SanPham> MapSanPhamList(DataTable dt)
        {
            var list = new List<SanPham>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new SanPham
                {
                    Masp = row["MASP"].ToString() ?? "",
                    Tensp = row["TENSP"] != DBNull.Value ? row["TENSP"].ToString() : null,
                    Mota = row["MOTA"] != DBNull.Value ? row["MOTA"].ToString() : null,
                    Giaban = row["GIABAN"] != DBNull.Value ? Convert.ToDecimal(row["GIABAN"]) : null,
                    Soluongton = row["SOLUONGTON"] != DBNull.Value ? Convert.ToInt32(row["SOLUONGTON"]) : null,
                    Hinhanh = row["HINHANH"] != DBNull.Value ? row["HINHANH"].ToString() : null
                });
            }
            return list;
        }

        // Chuyển DataTable thành List<QuanBaSp>
        private List<QuanBaSp> MapQuanBaSpList(DataTable dt)
        {
            var list = new List<QuanBaSp>();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new QuanBaSp
                {
                    Madotgiamgia = row["MADOTGIAMGIA"].ToString() ?? "",
                    Ngaybatdau = row["NGAYBATDAU"] != DBNull.Value ? Convert.ToDateTime(row["NGAYBATDAU"]) : null,
                    Ngayketthuc = row["NGAYKETTHUC"] != DBNull.Value ? Convert.ToDateTime(row["NGAYKETTHUC"]) : null,
                    Userid = row["USERID"] != DBNull.Value ? row["USERID"].ToString() : null
                });
            }
            return list;
        }
    }
}
