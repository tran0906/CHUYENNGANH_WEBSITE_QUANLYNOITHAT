using Microsoft.AspNetCore.Mvc;
using DOANCHUYENNGANH_WEB_QLNOITHAT.BLL;
using DOANCHUYENNGANH_WEB_QLNOITHAT.Areas.Admin.Filters;

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

        public IActionResult Index()
        {
            ViewBag.TongSanPham = _sanPhamBLL.Count();
            ViewBag.TongDonHang = _donHangBLL.Count();
            ViewBag.TongKhachHang = _khachHangBLL.Count();
            ViewBag.TongDoanhThu = _thanhToanBLL.GetAll().Sum(t => t.Sotien ?? 0);
            
            ViewBag.DonHangMoi = _donHangBLL.CountByTrangThai("Chờ xử lý") + 
                                _donHangBLL.CountByTrangThai("Chờ xác nhận");
            
            ViewBag.DonHangGanDay = _donHangBLL.GetAll()
                .OrderByDescending(d => d.Ngaydat)
                .Take(5)
                .ToList();

            return View();
        }
    }
}
