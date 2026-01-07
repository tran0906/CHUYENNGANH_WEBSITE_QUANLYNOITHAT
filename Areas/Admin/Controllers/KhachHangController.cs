using Microsoft.AspNetCore.Mvc;
using DOANCHUYENNGANH_WEB_QLNOITHAT.BLL;
using DOANCHUYENNGANH_WEB_QLNOITHAT.Models;
using DOANCHUYENNGANH_WEB_QLNOITHAT.Areas.Admin.Filters;

namespace DOANCHUYENNGANH_WEB_QLNOITHAT.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminOnlyFilter]
    public class KhachHangController : Controller
    {
        private readonly KhachHangBLL _bll = new KhachHangBLL();
        private readonly DonHangBLL _donHangBLL = new DonHangBLL();

        public IActionResult Index(string? search)
        {
            var list = _bll.Search(search);
            ViewBag.Search = search;
            return View(list);
        }

        // GET: Admin/KhachHang/DanhSach - Cho Nhân viên xem danh sách khách hàng
        [SkipAdminOnlyFilter]
        [AdminAuthFilter]
        public IActionResult DanhSach(string? search)
        {
            var list = _bll.Search(search);
            ViewBag.Search = search;
            return View(list);
        }

        // GET: Admin/KhachHang/ChiTiet/5 - Cho nhân viên xem chi tiết khách hàng
        [SkipAdminOnlyFilter]
        [AdminAuthFilter]
        public IActionResult ChiTiet(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();
            var obj = _bll.GetById(id);
            if (obj == null) return NotFound();
            
            // Lấy danh sách đơn hàng của khách hàng kèm tổng tiền
            var donHangs = _donHangBLL.GetByKhachHangWithTotal(id);
            ViewBag.DonHangs = donHangs;
            
            // Thống kê
            ViewBag.TongDonHang = donHangs.Count;
            ViewBag.DonHoanThanh = donHangs.Count(d => d.Trangthai == "Hoàn thành" || d.Trangthai == "Đã giao");
            ViewBag.DonDangXuLy = donHangs.Count(d => d.Trangthai != "Hoàn thành" && d.Trangthai != "Đã giao" && d.Trangthai != "Đã hủy");
            ViewBag.DonDaHuy = donHangs.Count(d => d.Trangthai == "Đã hủy");
            ViewBag.TongChiTieu = _donHangBLL.GetTongChiTieuKhachHang(id);
            
            return View(obj);
        }

        public IActionResult Details(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();
            var obj = _bll.GetById(id);
            if (obj == null) return NotFound();
            
            // Lấy danh sách đơn hàng của khách hàng kèm tổng tiền
            var donHangs = _donHangBLL.GetByKhachHangWithTotal(id);
            ViewBag.DonHangs = donHangs;
            
            // Thống kê
            ViewBag.TongDonHang = donHangs.Count;
            ViewBag.DonHoanThanh = donHangs.Count(d => d.Trangthai == "Hoàn thành" || d.Trangthai == "Đã giao");
            ViewBag.DonDangXuLy = donHangs.Count(d => d.Trangthai != "Hoàn thành" && d.Trangthai != "Đã giao" && d.Trangthai != "Đã hủy");
            ViewBag.DonDaHuy = donHangs.Count(d => d.Trangthai == "Đã hủy");
            ViewBag.TongChiTieu = _donHangBLL.GetTongChiTieuKhachHang(id);
            
            return View(obj);
        }

        public IActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();
            var obj = _bll.GetById(id);
            if (obj == null) return NotFound();
            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(string id, KhachHang obj)
        {
            if (id != obj.Makh) return NotFound();
            if (ModelState.IsValid)
            {
                var (success, message) = _bll.Update(obj);
                if (success)
                {
                    TempData["Success"] = message;
                    return RedirectToAction(nameof(Index));
                }
                ViewBag.Error = message;
            }
            return View(obj);
        }

        public IActionResult Delete(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();
            var obj = _bll.GetById(id);
            if (obj == null) return NotFound();
            return View(obj);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(string id)
        {
            var (success, message) = _bll.Delete(id);
            if (success)
                TempData["Success"] = message;
            else
                TempData["Error"] = message;
            return RedirectToAction(nameof(Index));
        }
    }
}
