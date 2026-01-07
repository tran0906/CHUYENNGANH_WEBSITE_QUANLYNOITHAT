// FILE: DAL/DonHangDAL.cs - Truy cập dữ liệu bảng DON_HANG

using System.Data;
using Microsoft.Data.SqlClient;
using DOANCHUYENNGANH_WEB_QLNOITHAT.Models;

namespace DOANCHUYENNGANH_WEB_QLNOITHAT.DAL
{
    public class DonHangDAL
    {
        // Lấy tất cả đơn hàng kèm tên khách hàng
        public List<DonHang> GetAll()
        {
            string query = @"SELECT dh.*, kh.HOTENKH as Hotenkh
                            FROM DON_HANG dh
                            LEFT JOIN KHACH_HANG kh ON dh.MAKH = kh.MAKH
                            ORDER BY dh.NGAYDAT DESC";
            return MapDataTableToList(SqlConnectionHelper.ExecuteQuery(query));
        }

        public List<DonHang> Search(string? maDh, string? maKh, string? trangThai, DateTime? tuNgay, DateTime? denNgay)
        {
            string query = @"SELECT dh.*, kh.HOTENKH as Hotenkh
                            FROM DON_HANG dh
                            LEFT JOIN KHACH_HANG kh ON dh.MAKH = kh.MAKH
                            WHERE 1=1";
            var parameters = new List<SqlParameter>();

            if (!string.IsNullOrEmpty(maDh))
            {
                query += " AND dh.MADONHANG LIKE @MaDh";
                parameters.Add(new SqlParameter("@MaDh", $"%{maDh}%"));
            }
            if (!string.IsNullOrEmpty(maKh))
            {
                query += " AND dh.MAKH = @MaKh";
                parameters.Add(new SqlParameter("@MaKh", maKh));
            }
            if (!string.IsNullOrEmpty(trangThai))
            {
                query += " AND dh.TRANGTHAI = @TrangThai";
                parameters.Add(new SqlParameter("@TrangThai", trangThai));
            }
            if (tuNgay.HasValue)
            {
                query += " AND dh.NGAYDAT >= @TuNgay";
                parameters.Add(new SqlParameter("@TuNgay", tuNgay.Value.Date));
            }
            if (denNgay.HasValue)
            {
                query += " AND dh.NGAYDAT <= @DenNgay";
                parameters.Add(new SqlParameter("@DenNgay", denNgay.Value.Date.AddDays(1)));
            }
            query += " ORDER BY dh.NGAYDAT DESC";
            return MapDataTableToList(SqlConnectionHelper.ExecuteQuery(query, parameters.ToArray()));
        }

        public List<DonHang> GetByKhachHang(string maKh)
        {
            string query = @"SELECT dh.*, kh.HOTENKH as Hotenkh
                            FROM DON_HANG dh
                            LEFT JOIN KHACH_HANG kh ON dh.MAKH = kh.MAKH
                            WHERE dh.MAKH = @MaKh ORDER BY dh.NGAYDAT DESC";
            SqlParameter[] parameters = { new SqlParameter("@MaKh", maKh) };
            return MapDataTableToList(SqlConnectionHelper.ExecuteQuery(query, parameters));
        }

        public DonHang? GetById(string maDh)
        {
            string query = @"SELECT dh.*, kh.HOTENKH as Hotenkh, kh.SDTKH as Sdtkh, kh.DIACHIKH as Diachikh
                            FROM DON_HANG dh
                            LEFT JOIN KHACH_HANG kh ON dh.MAKH = kh.MAKH
                            WHERE dh.MADONHANG = @MaDh";
            SqlParameter[] parameters = { new SqlParameter("@MaDh", maDh) };
            return MapDataTableToList(SqlConnectionHelper.ExecuteQuery(query, parameters)).FirstOrDefault();
        }

        public int Insert(DonHang obj)
        {
            string query = @"INSERT INTO DON_HANG (MADONHANG, MAKH, NGUOIDUYETID, GHICHU, TRANGTHAI, NGAYDAT) 
                            VALUES (@Madonhang, @Makh, @Nguoiduyetid, @Ghichu, @Trangthai, @Ngaydat)";
            SqlParameter[] parameters = {
                new SqlParameter("@Madonhang", obj.Madonhang),
                new SqlParameter("@Makh", obj.Makh),
                new SqlParameter("@Nguoiduyetid", (object?)obj.Nguoiduyetid ?? DBNull.Value),
                new SqlParameter("@Ghichu", (object?)obj.Ghichu ?? DBNull.Value),
                new SqlParameter("@Trangthai", (object?)obj.Trangthai ?? DBNull.Value),
                new SqlParameter("@Ngaydat", obj.Ngaydat)
            };
            return SqlConnectionHelper.ExecuteNonQuery(query, parameters);
        }

        public int Update(DonHang obj)
        {
            string query = @"UPDATE DON_HANG SET MAKH=@Makh, NGUOIDUYETID=@Nguoiduyetid, 
                            GHICHU=@Ghichu, TRANGTHAI=@Trangthai, NGAYDAT=@Ngaydat WHERE MADONHANG=@Madonhang";
            SqlParameter[] parameters = {
                new SqlParameter("@Madonhang", obj.Madonhang),
                new SqlParameter("@Makh", obj.Makh),
                new SqlParameter("@Nguoiduyetid", (object?)obj.Nguoiduyetid ?? DBNull.Value),
                new SqlParameter("@Ghichu", (object?)obj.Ghichu ?? DBNull.Value),
                new SqlParameter("@Trangthai", (object?)obj.Trangthai ?? DBNull.Value),
                new SqlParameter("@Ngaydat", obj.Ngaydat)
            };
            return SqlConnectionHelper.ExecuteNonQuery(query, parameters);
        }

        public int UpdateTrangThai(string maDh, string trangThai, string? nguoiDuyetId = null)
        {
            string query = "UPDATE DON_HANG SET TRANGTHAI=@TrangThai, NGUOIDUYETID=@NguoiDuyetId WHERE MADONHANG=@MaDh";
            SqlParameter[] parameters = {
                new SqlParameter("@MaDh", maDh),
                new SqlParameter("@TrangThai", trangThai),
                new SqlParameter("@NguoiDuyetId", (object?)nguoiDuyetId ?? DBNull.Value)
            };
            return SqlConnectionHelper.ExecuteNonQuery(query, parameters);
        }

        public int Delete(string maDh)
        {
            string query = "DELETE FROM DON_HANG WHERE MADONHANG=@MaDh";
            SqlParameter[] parameters = { new SqlParameter("@MaDh", maDh) };
            return SqlConnectionHelper.ExecuteNonQuery(query, parameters);
        }

        public bool Exists(string maDh)
        {
            string query = "SELECT COUNT(*) FROM DON_HANG WHERE MADONHANG=@MaDh";
            SqlParameter[] parameters = { new SqlParameter("@MaDh", maDh) };
            return Convert.ToInt32(SqlConnectionHelper.ExecuteScalar(query, parameters)) > 0;
        }

        public int Count()
        {
            return Convert.ToInt32(SqlConnectionHelper.ExecuteScalar("SELECT COUNT(*) FROM DON_HANG"));
        }

        public int CountByTrangThai(string trangThai)
        {
            string query = "SELECT COUNT(*) FROM DON_HANG WHERE TRANGTHAI=@TrangThai";
            SqlParameter[] parameters = { new SqlParameter("@TrangThai", trangThai) };
            return Convert.ToInt32(SqlConnectionHelper.ExecuteScalar(query, parameters));
        }

        /// <summary>
        /// Tính tổng doanh thu từ các đơn hàng đã hoàn thành
        /// </summary>
        public decimal GetTongDoanhThu()
        {
            string query = @"SELECT ISNULL(SUM(ct.THANHTIEN), 0) 
                            FROM DON_HANG dh
                            INNER JOIN CT_DONHANG ct ON dh.MADONHANG = ct.MADONHANG
                            WHERE dh.TRANGTHAI = 'Hoàn thành'";
            var result = SqlConnectionHelper.ExecuteScalar(query);
            return result != null && result != DBNull.Value ? Convert.ToDecimal(result) : 0;
        }

        /// <summary>
        /// Tính doanh thu theo khoảng thời gian
        /// </summary>
        public decimal GetDoanhThuTheoThoiGian(DateTime tuNgay, DateTime denNgay)
        {
            string query = @"SELECT ISNULL(SUM(ct.THANHTIEN), 0) 
                            FROM DON_HANG dh
                            INNER JOIN CT_DONHANG ct ON dh.MADONHANG = ct.MADONHANG
                            WHERE dh.TRANGTHAI = 'Hoàn thành'
                            AND dh.NGAYDAT >= @TuNgay AND dh.NGAYDAT < @DenNgay";
            SqlParameter[] parameters = {
                new SqlParameter("@TuNgay", tuNgay.Date),
                new SqlParameter("@DenNgay", denNgay.Date.AddDays(1))
            };
            var result = SqlConnectionHelper.ExecuteScalar(query, parameters);
            return result != null && result != DBNull.Value ? Convert.ToDecimal(result) : 0;
        }

        public string GenerateNewId()
        {
            // Lấy số lớn nhất từ các mã đơn hàng có dạng DHxxx
            var query = @"SELECT ISNULL(MAX(CAST(SUBSTRING(MADONHANG, 3, LEN(MADONHANG)-2) AS INT)), 0) 
                          FROM DON_HANG WHERE MADONHANG LIKE 'DH%' AND ISNUMERIC(SUBSTRING(MADONHANG, 3, LEN(MADONHANG)-2)) = 1";
            var result = SqlConnectionHelper.ExecuteScalar(query);
            int maxNum = result != null && result != DBNull.Value ? Convert.ToInt32(result) : 0;
            
            // Tạo mã mới với số tăng dần
            string newId;
            do
            {
                maxNum++;
                newId = $"DH{maxNum:D3}";
            } while (Exists(newId)); // Đảm bảo mã chưa tồn tại
            
            return newId;
        }

        /// <summary>
        /// Lấy đơn hàng của khách hàng kèm tổng tiền
        /// </summary>
        public List<DonHang> GetByKhachHangWithTotal(string maKh)
        {
            string query = @"SELECT dh.*, kh.HOTENKH as Hotenkh,
                            ISNULL((SELECT SUM(ct.THANHTIEN) FROM CT_DONHANG ct WHERE ct.MADONHANG = dh.MADONHANG), 0) as TongTien
                            FROM DON_HANG dh
                            LEFT JOIN KHACH_HANG kh ON dh.MAKH = kh.MAKH
                            WHERE dh.MAKH = @MaKh ORDER BY dh.NGAYDAT DESC";
            SqlParameter[] parameters = { new SqlParameter("@MaKh", maKh) };
            return MapDataTableToListWithTotal(SqlConnectionHelper.ExecuteQuery(query, parameters));
        }

        /// <summary>
        /// Tính tổng chi tiêu của khách hàng (chỉ đơn hoàn thành)
        /// </summary>
        public decimal GetTongChiTieuKhachHang(string maKh)
        {
            string query = @"SELECT ISNULL(SUM(ct.THANHTIEN), 0) 
                            FROM DON_HANG dh
                            INNER JOIN CT_DONHANG ct ON dh.MADONHANG = ct.MADONHANG
                            WHERE dh.MAKH = @MaKh AND (dh.TRANGTHAI = N'Hoàn thành' OR dh.TRANGTHAI = N'Đã giao')";
            SqlParameter[] parameters = { new SqlParameter("@MaKh", maKh) };
            var result = SqlConnectionHelper.ExecuteScalar(query, parameters);
            return result != null && result != DBNull.Value ? Convert.ToDecimal(result) : 0;
        }

        private List<DonHang> MapDataTableToListWithTotal(DataTable dt)
        {
            var list = new List<DonHang>();
            foreach (DataRow row in dt.Rows)
            {
                var dh = new DonHang
                {
                    Madonhang = row["MADONHANG"].ToString() ?? "",
                    Makh = row["MAKH"].ToString() ?? "",
                    Nguoiduyetid = row["NGUOIDUYETID"] != DBNull.Value ? row["NGUOIDUYETID"].ToString() : null,
                    Ghichu = row["GHICHU"] != DBNull.Value ? row["GHICHU"].ToString() : null,
                    Trangthai = row["TRANGTHAI"] != DBNull.Value ? row["TRANGTHAI"].ToString() : null,
                    Ngaydat = Convert.ToDateTime(row["NGAYDAT"])
                };

                if (dt.Columns.Contains("Hotenkh") && row["Hotenkh"] != DBNull.Value)
                {
                    dh.MakhNavigation = new KhachHang { Makh = dh.Makh, Hotenkh = row["Hotenkh"].ToString() };
                }
                
                // Lưu tổng tiền vào CtDonhangs (tạm thời)
                if (dt.Columns.Contains("TongTien") && row["TongTien"] != DBNull.Value)
                {
                    var tongTien = Convert.ToDecimal(row["TongTien"]);
                    dh.CtDonhangs = new List<CtDonhang> { new CtDonhang { Thanhtien = tongTien } };
                }
                
                list.Add(dh);
            }
            return list;
        }

        private List<DonHang> MapDataTableToList(DataTable dt)
        {
            var list = new List<DonHang>();
            foreach (DataRow row in dt.Rows)
            {
                var dh = new DonHang
                {
                    Madonhang = row["MADONHANG"].ToString() ?? "",
                    Makh = row["MAKH"].ToString() ?? "",
                    Nguoiduyetid = row["NGUOIDUYETID"] != DBNull.Value ? row["NGUOIDUYETID"].ToString() : null,
                    Ghichu = row["GHICHU"] != DBNull.Value ? row["GHICHU"].ToString() : null,
                    Trangthai = row["TRANGTHAI"] != DBNull.Value ? row["TRANGTHAI"].ToString() : null,
                    Ngaydat = Convert.ToDateTime(row["NGAYDAT"])
                };

                if (dt.Columns.Contains("Hotenkh") && row["Hotenkh"] != DBNull.Value)
                {
                    dh.MakhNavigation = new KhachHang 
                    { 
                        Makh = dh.Makh, 
                        Hotenkh = row["Hotenkh"].ToString(),
                        Sdtkh = dt.Columns.Contains("Sdtkh") && row["Sdtkh"] != DBNull.Value ? row["Sdtkh"].ToString() : null,
                        Diachikh = dt.Columns.Contains("Diachikh") && row["Diachikh"] != DBNull.Value ? row["Diachikh"].ToString() : null
                    };
                }
                list.Add(dh);
            }
            return list;
        }
    }
}
