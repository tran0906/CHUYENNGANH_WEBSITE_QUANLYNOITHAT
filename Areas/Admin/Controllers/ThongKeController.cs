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

        public IActionResult Index()
        {
            return View();
        }

        // Thống kê doanh thu theo thời gian
        public IActionResult DoanhThu(DateTime? tuNgay, DateTime? denNgay, string? groupBy = "day")
        {
            tuNgay ??= DateTime.Now.AddMonths(-1);
            denNgay ??= DateTime.Now;

            ViewBag.TuNgay = tuNgay.Value.ToString("yyyy-MM-dd");
            ViewBag.DenNgay = denNgay.Value.ToString("yyyy-MM-dd");
            ViewBag.GroupBy = groupBy;

            var data = GetDoanhThuTheoThoiGian(tuNgay.Value, denNgay.Value, groupBy);
            ViewBag.DoanhThuData = data;
            ViewBag.TongDoanhThu = data.AsEnumerable().Sum(r => r.Field<decimal>("DoanhThu"));

            return View();
        }

        // Thống kê sản phẩm bán chạy
        public IActionResult SanPhamBanChay(DateTime? tuNgay, DateTime? denNgay, int top = 10)
        {
            tuNgay ??= DateTime.Now.AddMonths(-1);
            denNgay ??= DateTime.Now;

            ViewBag.TuNgay = tuNgay.Value.ToString("yyyy-MM-dd");
            ViewBag.DenNgay = denNgay.Value.ToString("yyyy-MM-dd");
            ViewBag.Top = top;

            var data = GetSanPhamBanChay(tuNgay.Value, denNgay.Value, top);
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
        private DataTable GetDoanhThuTheoThoiGian(DateTime tuNgay, DateTime denNgay, string groupBy)
        {
            string dateFormat = groupBy switch
            {
                "month" => "YEAR(dh.NGAYDAT), MONTH(dh.NGAYDAT)",
                "year" => "YEAR(dh.NGAYDAT)",
                _ => "CAST(dh.NGAYDAT AS DATE)"
            };

            string selectFormat = groupBy switch
            {
                "month" => "CAST(YEAR(dh.NGAYDAT) AS VARCHAR) + '/' + RIGHT('0' + CAST(MONTH(dh.NGAYDAT) AS VARCHAR), 2) as ThoiGian",
                "year" => "CAST(YEAR(dh.NGAYDAT) AS VARCHAR) as ThoiGian",
                _ => "FORMAT(CAST(dh.NGAYDAT AS DATE), 'dd/MM/yyyy') as ThoiGian"
            };

            string query = $@"SELECT {selectFormat}, 
                             ISNULL(SUM(ct.THANHTIEN), 0) as DoanhThu,
                             COUNT(DISTINCT dh.MADONHANG) as SoDonHang
                             FROM DON_HANG dh
                             LEFT JOIN CT_DONHANG ct ON dh.MADONHANG = ct.MADONHANG
                             WHERE dh.NGAYDAT >= @TuNgay AND dh.NGAYDAT <= @DenNgay
                             AND dh.TRANGTHAI NOT IN (N'Đã hủy')
                             GROUP BY {dateFormat}
                             ORDER BY MIN(dh.NGAYDAT)";

            SqlParameter[] parameters = {
                new SqlParameter("@TuNgay", tuNgay.Date),
                new SqlParameter("@DenNgay", denNgay.Date.AddDays(1))
            };
            return SqlConnectionHelper.ExecuteQuery(query, parameters);
        }

        private DataTable GetSanPhamBanChay(DateTime tuNgay, DateTime denNgay, int top)
        {
            string query = $@"SELECT TOP {top} sp.MASP, sp.TENSP, sp.HINHANH,
                             SUM(ct.SOLUONG) as TongSoLuong,
                             SUM(ct.THANHTIEN) as TongDoanhThu
                             FROM CT_DONHANG ct
                             INNER JOIN SAN_PHAM sp ON ct.MASP = sp.MASP
                             INNER JOIN DON_HANG dh ON ct.MADONHANG = dh.MADONHANG
                             WHERE dh.NGAYDAT >= @TuNgay AND dh.NGAYDAT <= @DenNgay
                             AND dh.TRANGTHAI NOT IN (N'Đã hủy')
                             GROUP BY sp.MASP, sp.TENSP, sp.HINHANH
                             ORDER BY TongSoLuong DESC";

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
            string query = $@"SELECT TOP {top} kh.MAKH, kh.HOTENKH, kh.SDTKH, kh.DIACHIKH,
                             COUNT(DISTINCT dh.MADONHANG) as SoDonHang,
                             ISNULL(SUM(ct.THANHTIEN), 0) as TongChiTieu
                             FROM KHACH_HANG kh
                             INNER JOIN DON_HANG dh ON kh.MAKH = dh.MAKH
                             LEFT JOIN CT_DONHANG ct ON dh.MADONHANG = ct.MADONHANG
                             WHERE dh.NGAYDAT >= @TuNgay AND dh.NGAYDAT <= @DenNgay
                             AND dh.TRANGTHAI NOT IN (N'Đã hủy')
                             GROUP BY kh.MAKH, kh.HOTENKH, kh.SDTKH, kh.DIACHIKH
                             ORDER BY TongChiTieu DESC";

            SqlParameter[] parameters = {
                new SqlParameter("@TuNgay", tuNgay.Date),
                new SqlParameter("@DenNgay", denNgay.Date.AddDays(1))
            };
            return SqlConnectionHelper.ExecuteQuery(query, parameters);
        }
        #endregion
    }
}
