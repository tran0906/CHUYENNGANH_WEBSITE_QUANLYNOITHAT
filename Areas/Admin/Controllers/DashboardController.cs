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
    public class DashboardController : Controller
    {
        private readonly SanPhamBLL _sanPhamBLL = new SanPhamBLL();
        private readonly DonHangBLL _donHangBLL = new DonHangBLL();
        private readonly KhachHangBLL _khachHangBLL = new KhachHangBLL();
        private readonly ThanhToanBLL _thanhToanBLL = new ThanhToanBLL();
        private readonly StoredProcedureBLL _spBLL = new StoredProcedureBLL();

        public IActionResult Index()
        {
            ViewBag.TongSanPham = _sanPhamBLL.Count();
            ViewBag.TongDonHang = _donHangBLL.Count();
            ViewBag.TongKhachHang = _khachHangBLL.Count();
            
            // Doanh thu = tổng tiền các đơn hàng đã hoàn thành
            ViewBag.TongDoanhThu = _donHangBLL.GetTongDoanhThu();
            
            // Doanh thu hôm nay - tính từ bảng THANH_TOAN
            ViewBag.DoanhThuHomNay = _thanhToanBLL.GetDoanhThuNgay(DateTime.Today);
            
            ViewBag.DonHangMoi = _donHangBLL.CountByTrangThai("Chờ xử lý") + 
                                _donHangBLL.CountByTrangThai("Chờ xác nhận") +
                                _donHangBLL.CountByTrangThai("Chờ thanh toán");
            
            ViewBag.DonHangGanDay = _donHangBLL.GetAll()
                .OrderByDescending(d => d.Ngaydat)
                .Take(5)
                .ToList();
            
            // Top sản phẩm bán chạy - query trực tiếp (chỉ tính đơn hoàn thành)
            ViewBag.TopSanPhamBanChay = GetTopSanPhamBanChay(5);

            return View();
        }

        /// <summary>
        /// Lấy top sản phẩm bán chạy (chỉ tính đơn hàng Hoàn thành)
        /// </summary>
        private DataTable GetTopSanPhamBanChay(int top)
        {
            string query = $@"SELECT TOP {top} 
                             sp.MASP, sp.TENSP,
                             ISNULL(SUM(ct.SOLUONG), 0) as TongSoLuong
                             FROM SAN_PHAM sp
                             INNER JOIN CT_DONHANG ct ON sp.MASP = ct.MASP
                             INNER JOIN DON_HANG dh ON ct.MADONHANG = dh.MADONHANG
                             WHERE dh.TRANGTHAI = N'Hoàn thành'
                             GROUP BY sp.MASP, sp.TENSP
                             ORDER BY TongSoLuong DESC";
            return SqlConnectionHelper.ExecuteQuery(query);
        }
    }
}
