using Microsoft.AspNetCore.Mvc;
using DOANCHUYENNGANH_WEB_QLNOITHAT.BLL;
using DOANCHUYENNGANH_WEB_QLNOITHAT.Areas.Admin.Filters;
using DOANCHUYENNGANH_WEB_QLNOITHAT.DAL;
using System.Data;
using Microsoft.Data.SqlClient;

namespace DOANCHUYENNGANH_WEB_QLNOITHAT.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthFilter]
    public class ThongKeController : Controller
    {
        private readonly DonHangBLL _donHangBLL = new DonHangBLL();
        private readonly SanPhamBLL _sanPhamBLL = new SanPhamBLL();
        private readonly KhachHangBLL _khachHangBLL = new KhachHangBLL();
        private readonly StoredProcedureBLL _spBLL = new StoredProcedureBLL();

        public IActionResult Index()
        {
            return View();
        }

        // Thống kê doanh thu theo thời gian - sử dụng Stored Procedures
        public IActionResult DoanhThu(DateTime? tuNgay, DateTime? denNgay, string? groupBy = "day")
        {
            tuNgay ??= DateTime.Now.AddMonths(-1);
            denNgay ??= DateTime.Now;

            ViewBag.TuNgay = tuNgay.Value.ToString("yyyy-MM-dd");
            ViewBag.DenNgay = denNgay.Value.ToString("yyyy-MM-dd");
            ViewBag.GroupBy = groupBy;

            DataTable data;
            
            // Sử dụng stored procedure theo loại thống kê
            if (groupBy == "month")
            {
                // Thống kê theo tháng - dùng sp_ThongKeDoanhThu_Thang
                var thang = tuNgay.Value.Month;
                var nam = tuNgay.Value.Year;
                data = _spBLL.ThongKeDoanhThuThang(thang, nam);
            }
            else if (groupBy == "year")
            {
                // Thống kê theo năm - dùng sp_ThongKeDoanhThu_Nam
                var nam = tuNgay.Value.Year;
                data = _spBLL.ThongKeDoanhThuNam(nam);
            }
            else
            {
                // Thống kê theo ngày - dùng query tùy chỉnh với khoảng thời gian
                data = GetDoanhThuTheoKhoangThoiGian(tuNgay.Value, denNgay.Value);
            }
            
            ViewBag.DoanhThuData = data;
            
            // Tính tổng doanh thu
            decimal tongDoanhThu = 0;
            foreach (DataRow row in data.Rows)
            {
                // Tìm cột doanh thu (có thể là DoanhThu, TongDoanhThu, TONGTIEN...)
                if (data.Columns.Contains("DoanhThu"))
                    tongDoanhThu += row["DoanhThu"] != DBNull.Value ? Convert.ToDecimal(row["DoanhThu"]) : 0;
                else if (data.Columns.Contains("TongDoanhThu"))
                    tongDoanhThu += row["TongDoanhThu"] != DBNull.Value ? Convert.ToDecimal(row["TongDoanhThu"]) : 0;
                else if (data.Columns.Contains("TONGTIEN"))
                    tongDoanhThu += row["TONGTIEN"] != DBNull.Value ? Convert.ToDecimal(row["TONGTIEN"]) : 0;
            }
            ViewBag.TongDoanhThu = tongDoanhThu;

            return View();
        }

        // Thống kê sản phẩm bán chạy - query với filter thời gian và doanh thu
        public IActionResult SanPhamBanChay(DateTime? tuNgay, DateTime? denNgay, int top = 10)
        {
            tuNgay ??= DateTime.Now.AddMonths(-1);
            denNgay ??= DateTime.Now;

            ViewBag.TuNgay = tuNgay.Value.ToString("yyyy-MM-dd");
            ViewBag.DenNgay = denNgay.Value.ToString("yyyy-MM-dd");
            ViewBag.Top = top;

            // Query lấy top sản phẩm bán chạy với doanh thu (chỉ tính đơn hoàn thành)
            var data = GetTopSanPhamBanChay(tuNgay.Value, denNgay.Value, top);
            return View(data);
        }

        // Thống kê đơn hàng theo trạng thái
        public IActionResult DonHangTheoTrangThai(DateTime? tuNgay, DateTime? denNgay)
        {
            tuNgay ??= DateTime.Now.AddMonths(-1);
            denNgay ??= DateTime.Now;

            ViewBag.TuNgay = tuNgay.Value.ToString("yyyy-MM-dd");
            ViewBag.DenNgay = denNgay.Value.ToString("yyyy-MM-dd");

            var data = GetDonHangTheoTrangThai(tuNgay.Value, denNgay.Value);
            ViewBag.TongDonHang = data.AsEnumerable().Sum(r => r.Field<int>("SoLuong"));
            return View(data);
        }

        // Thống kê khách hàng mua nhiều
        public IActionResult KhachHangVIP(DateTime? tuNgay, DateTime? denNgay, int top = 10)
        {
            tuNgay ??= DateTime.Now.AddMonths(-1);
            denNgay ??= DateTime.Now;

            ViewBag.TuNgay = tuNgay.Value.ToString("yyyy-MM-dd");
            ViewBag.DenNgay = denNgay.Value.ToString("yyyy-MM-dd");
            ViewBag.Top = top;

            var data = GetKhachHangMuaNhieu(tuNgay.Value, denNgay.Value, top);
            return View(data);
        }

        #region Private Methods
        
        /// <summary>
        /// Lấy doanh thu theo khoảng thời gian (dùng cho thống kê theo ngày)
        /// Chỉ tính đơn hàng đã hoàn thành
        /// </summary>
        private DataTable GetDoanhThuTheoKhoangThoiGian(DateTime tuNgay, DateTime denNgay)
        {
            string query = @"SELECT FORMAT(CAST(dh.NGAYDAT AS DATE), 'dd/MM/yyyy') as ThoiGian, 
                             ISNULL(SUM(ct.THANHTIEN), 0) as DoanhThu,
                             COUNT(DISTINCT dh.MADONHANG) as SoDonHang
                             FROM DON_HANG dh
                             LEFT JOIN CT_DONHANG ct ON dh.MADONHANG = ct.MADONHANG
                             WHERE dh.NGAYDAT >= @TuNgay AND dh.NGAYDAT <= @DenNgay
                             AND dh.TRANGTHAI = N'Hoàn thành'
                             GROUP BY CAST(dh.NGAYDAT AS DATE)
                             ORDER BY MIN(dh.NGAYDAT)";

            SqlParameter[] parameters = {
                new SqlParameter("@TuNgay", tuNgay.Date),
                new SqlParameter("@DenNgay", denNgay.Date.AddDays(1))
            };
            return SqlConnectionHelper.ExecuteQuery(query, parameters);
        }

        private DataTable GetDonHangTheoTrangThai(DateTime tuNgay, DateTime denNgay)
        {
            string query = @"SELECT ISNULL(TRANGTHAI, N'Chưa xác định') as TrangThai, 
                            COUNT(*) as SoLuong
                            FROM DON_HANG
                            WHERE NGAYDAT >= @TuNgay AND NGAYDAT <= @DenNgay
                            GROUP BY TRANGTHAI";

            SqlParameter[] parameters = {
                new SqlParameter("@TuNgay", tuNgay.Date),
                new SqlParameter("@DenNgay", denNgay.Date.AddDays(1))
            };
            return SqlConnectionHelper.ExecuteQuery(query, parameters);
        }

        private DataTable GetKhachHangMuaNhieu(DateTime tuNgay, DateTime denNgay, int top)
        {
            // Khách hàng VIP chỉ tính từ đơn hàng đã hoàn thành
            string query = $@"SELECT TOP {top} kh.MAKH, kh.HOTENKH, kh.SDTKH, kh.DIACHIKH,
                             COUNT(DISTINCT dh.MADONHANG) as SoDonHang,
                             ISNULL(SUM(ct.THANHTIEN), 0) as TongChiTieu
                             FROM KHACH_HANG kh
                             INNER JOIN DON_HANG dh ON kh.MAKH = dh.MAKH
                             LEFT JOIN CT_DONHANG ct ON dh.MADONHANG = ct.MADONHANG
                             WHERE dh.NGAYDAT >= @TuNgay AND dh.NGAYDAT <= @DenNgay
                             AND dh.TRANGTHAI = N'Hoàn thành'
                             GROUP BY kh.MAKH, kh.HOTENKH, kh.SDTKH, kh.DIACHIKH
                             ORDER BY TongChiTieu DESC";

            SqlParameter[] parameters = {
                new SqlParameter("@TuNgay", tuNgay.Date),
                new SqlParameter("@DenNgay", denNgay.Date.AddDays(1))
            };
            return SqlConnectionHelper.ExecuteQuery(query, parameters);
        }

        /// <summary>
        /// Lấy top sản phẩm bán chạy với doanh thu (chỉ tính đơn hoàn thành)
        /// </summary>
        private DataTable GetTopSanPhamBanChay(DateTime tuNgay, DateTime denNgay, int top)
        {
            string query = $@"SELECT TOP {top} 
                             sp.MASP, sp.TENSP, sp.HINHANH,
                             ISNULL(SUM(ct.SOLUONG), 0) as TongSoLuong,
                             ISNULL(SUM(ct.THANHTIEN), 0) as TongDoanhThu
                             FROM SAN_PHAM sp
                             INNER JOIN CT_DONHANG ct ON sp.MASP = ct.MASP
                             INNER JOIN DON_HANG dh ON ct.MADONHANG = dh.MADONHANG
                             WHERE dh.NGAYDAT >= @TuNgay AND dh.NGAYDAT <= @DenNgay
                             AND dh.TRANGTHAI = N'Hoàn thành'
                             GROUP BY sp.MASP, sp.TENSP, sp.HINHANH
                             ORDER BY TongSoLuong DESC";

            SqlParameter[] parameters = {
                new SqlParameter("@TuNgay", tuNgay.Date),
                new SqlParameter("@DenNgay", denNgay.Date.AddDays(1))
            };
            return SqlConnectionHelper.ExecuteQuery(query, parameters);
        }
        #endregion
    }
}
