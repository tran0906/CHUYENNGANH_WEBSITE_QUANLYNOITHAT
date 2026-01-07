// FILE: DAL/SanPhamDAL.cs - Tầng truy cập dữ liệu cho Sản phẩm (CRUD với SQL Server)

using System.Data;
using Microsoft.Data.SqlClient;
using DOANCHUYENNGANH_WEB_QLNOITHAT.Models;

namespace DOANCHUYENNGANH_WEB_QLNOITHAT.DAL
{
    public class SanPhamDAL
    {
        // Lấy tất cả sản phẩm
        public List<SanPham> GetAll()
        {
            string query = @"SELECT sp.*, nsp.TENNHOMSP as Tennhomsp, vl.TENVL as Tenvl 
                            FROM SAN_PHAM sp
                            LEFT JOIN NHOM_SAN_PHAM nsp ON sp.MANHOMSP = nsp.MANHOMSP
                            LEFT JOIN VAT_LIEU vl ON sp.MAVL = vl.MAVL
                            ORDER BY sp.MASP";
            
            DataTable dt = SqlConnectionHelper.ExecuteQuery(query);
            return MapDataTableToList(dt);
        }

        // Tìm kiếm sản phẩm theo điều kiện
        public List<SanPham> Search(string? search, string? nhomSp, string? vatLieu)
        {
            string query = @"SELECT sp.*, nsp.TENNHOMSP as Tennhomsp, vl.TENVL as Tenvl 
                            FROM SAN_PHAM sp
                            LEFT JOIN NHOM_SAN_PHAM nsp ON sp.MANHOMSP = nsp.MANHOMSP
                            LEFT JOIN VAT_LIEU vl ON sp.MAVL = vl.MAVL
                            WHERE 1=1";
            
            var parameters = new List<SqlParameter>();

            if (!string.IsNullOrEmpty(search))
            {
                query += " AND (sp.MASP LIKE @Search OR sp.TENSP LIKE @Search)";
                parameters.Add(new SqlParameter("@Search", $"%{search}%"));
            }
            if (!string.IsNullOrEmpty(nhomSp))
            {
                query += " AND sp.MANHOMSP = @NhomSp";
                parameters.Add(new SqlParameter("@NhomSp", nhomSp));
            }
            if (!string.IsNullOrEmpty(vatLieu))
            {
                query += " AND sp.MAVL = @VatLieu";
                parameters.Add(new SqlParameter("@VatLieu", vatLieu));
            }
            query += " ORDER BY sp.MASP";

            DataTable dt = SqlConnectionHelper.ExecuteQuery(query, parameters.ToArray());
            return MapDataTableToList(dt);
        }

        // Lấy sản phẩm theo mã
        public SanPham? GetById(string maSp)
        {
            string query = @"SELECT sp.*, nsp.TENNHOMSP as Tennhomsp, vl.TENVL as Tenvl 
                            FROM SAN_PHAM sp
                            LEFT JOIN NHOM_SAN_PHAM nsp ON sp.MANHOMSP = nsp.MANHOMSP
                            LEFT JOIN VAT_LIEU vl ON sp.MAVL = vl.MAVL
                            WHERE sp.MASP = @MaSp";
            
            SqlParameter[] parameters = { new SqlParameter("@MaSp", maSp) };
            DataTable dt = SqlConnectionHelper.ExecuteQuery(query, parameters);
            return MapDataTableToList(dt).FirstOrDefault();
        }

        // Thêm sản phẩm mới
        public int Insert(SanPham sp)
        {
            string query = @"INSERT INTO SAN_PHAM (MASP, TENSP, MOTA, GIABAN, SOLUONGTON, HINHANH, MANHOMSP, MAVL)
                            VALUES (@Masp, @Tensp, @Mota, @Giaban, @Soluongton, @Hinhanh, @Manhomsp, @Mavl)";
            
            SqlParameter[] parameters = {
                new SqlParameter("@Masp", sp.Masp),
                new SqlParameter("@Tensp", (object?)sp.Tensp ?? DBNull.Value),
                new SqlParameter("@Mota", (object?)sp.Mota ?? DBNull.Value),
                new SqlParameter("@Giaban", (object?)sp.Giaban ?? DBNull.Value),
                new SqlParameter("@Soluongton", (object?)sp.Soluongton ?? DBNull.Value),
                new SqlParameter("@Hinhanh", (object?)sp.Hinhanh ?? DBNull.Value),
                new SqlParameter("@Manhomsp", (object?)sp.Manhomsp ?? DBNull.Value),
                new SqlParameter("@Mavl", (object?)sp.Mavl ?? DBNull.Value)
            };
            return SqlConnectionHelper.ExecuteNonQuery(query, parameters);
        }

        // Cập nhật sản phẩm
        public int Update(SanPham sp)
        {
            string query = @"UPDATE SAN_PHAM SET 
                            TENSP = @Tensp, MOTA = @Mota, GIABAN = @Giaban, 
                            SOLUONGTON = @Soluongton, HINHANH = @Hinhanh, 
                            MANHOMSP = @Manhomsp, MAVL = @Mavl
                            WHERE MASP = @Masp";
            
            SqlParameter[] parameters = {
                new SqlParameter("@Masp", sp.Masp),
                new SqlParameter("@Tensp", (object?)sp.Tensp ?? DBNull.Value),
                new SqlParameter("@Mota", (object?)sp.Mota ?? DBNull.Value),
                new SqlParameter("@Giaban", (object?)sp.Giaban ?? DBNull.Value),
                new SqlParameter("@Soluongton", (object?)sp.Soluongton ?? DBNull.Value),
                new SqlParameter("@Hinhanh", (object?)sp.Hinhanh ?? DBNull.Value),
                new SqlParameter("@Manhomsp", (object?)sp.Manhomsp ?? DBNull.Value),
                new SqlParameter("@Mavl", (object?)sp.Mavl ?? DBNull.Value)
            };
            return SqlConnectionHelper.ExecuteNonQuery(query, parameters);
        }

        // Xóa sản phẩm
        public int Delete(string maSp)
        {
            string query = "DELETE FROM SAN_PHAM WHERE MASP = @MaSp";
            SqlParameter[] parameters = { new SqlParameter("@MaSp", maSp) };
            return SqlConnectionHelper.ExecuteNonQuery(query, parameters);
        }

        // Kiểm tra sản phẩm tồn tại
        public bool Exists(string maSp)
        {
            string query = "SELECT COUNT(*) FROM SAN_PHAM WHERE MASP = @MaSp";
            SqlParameter[] parameters = { new SqlParameter("@MaSp", maSp) };
            return Convert.ToInt32(SqlConnectionHelper.ExecuteScalar(query, parameters)) > 0;
        }

        // Đếm tổng số sản phẩm
        public int Count()
        {
            string query = "SELECT COUNT(*) FROM SAN_PHAM";
            return Convert.ToInt32(SqlConnectionHelper.ExecuteScalar(query));
        }

        // Lấy danh sách SP đang khuyến mãi kèm % giảm
        public Dictionary<string, int> GetProductsInPromotionWithDiscount()
        {
            try
            {
                string query = @"SELECT qb.MASP, qbs.PHANTRAMGIAM 
                                FROM QUANGBA qb
                                INNER JOIN QUAN_BA_SP qbs ON qb.MADOTGIAMGIA = qbs.MADOTGIAMGIA
                                WHERE GETDATE() BETWEEN qbs.NGAYBATDAU AND qbs.NGAYKETTHUC
                                AND qbs.PHANTRAMGIAM > 0";
                
                DataTable dt = SqlConnectionHelper.ExecuteQuery(query);
                var result = new Dictionary<string, int>();
                foreach (DataRow row in dt.Rows)
                {
                    if (row["MASP"] != DBNull.Value)
                    {
                        var masp = row["MASP"].ToString() ?? "";
                        var phantram = Convert.ToInt32(row["PHANTRAMGIAM"]);
                        if (!result.ContainsKey(masp) || result[masp] < phantram)
                            result[masp] = phantram;
                    }
                }
                return result;
            }
            catch
            {
                return new Dictionary<string, int>();
            }
        }

        // Chuyển DataTable thành List<SanPham>
        private List<SanPham> MapDataTableToList(DataTable dt)
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
                    Manhomsp = row["MANHOMSP"] != DBNull.Value ? row["MANHOMSP"].ToString() : null,
                    Mavl = row["MAVL"] != DBNull.Value ? row["MAVL"].ToString() : null
                };

                if (dt.Columns.Contains("Tennhomsp") && row["Tennhomsp"] != DBNull.Value)
                {
                    sp.ManhomspNavigation = new NhomSanPham 
                    { 
                        Manhomsp = sp.Manhomsp ?? "",
                        Tennhomsp = row["Tennhomsp"].ToString() 
                    };
                }
                if (dt.Columns.Contains("Tenvl") && row["Tenvl"] != DBNull.Value)
                {
                    sp.MavlNavigation = new VatLieu 
                    { 
                        Mavl = sp.Mavl ?? "",
                        Tenvl = row["Tenvl"].ToString() 
                    };
                }
                list.Add(sp);
            }
            return list;
        }
    }
}
