using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using DOANCHUYENNGANH_WEB_QLNOITHAT.BLL;
using DOANCHUYENNGANH_WEB_QLNOITHAT.Models;
using DOANCHUYENNGANH_WEB_QLNOITHAT.Areas.Admin.Filters;

namespace DOANCHUYENNGANH_WEB_QLNOITHAT.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthFilter]
    public class DonHangController : Controller
    {
        private readonly DonHangBLL _donHangBLL = new DonHangBLL();
        private readonly KhachHangBLL _khachHangBLL = new KhachHangBLL();
        private readonly UserBLL _userBLL = new UserBLL();
        private readonly CtDonhangBLL _ctDonhangBLL = new CtDonhangBLL();
        private readonly StoredProcedureBLL _spBLL = new StoredProcedureBLL();
        private readonly ThanhToanBLL _thanhToanBLL = new ThanhToanBLL();
        private readonly PhieuXuatKhoBLL _phieuXuatKhoBLL = new PhieuXuatKhoBLL();
        private readonly VanChuyenBLL _vanChuyenBLL = new VanChuyenBLL();

        public IActionResult Index(string? searchMa, string? trangThai)
        {
            var donHangs = _donHangBLL.Search(searchMa, null, trangThai, null, null);
            ViewBag.SearchMa = searchMa;
            ViewBag.TrangThai = trangThai;
            return View(donHangs);
        }

        public IActionResult Details(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();
            var donHang = _donHangBLL.GetById(id);
            if (donHang == null) return NotFound();
            
            // Lấy chi tiết đơn hàng và gán vào model
            donHang.CtDonhangs = _ctDonhangBLL.GetByDonHang(id);
            
            // Lấy thông tin người duyệt đơn hàng
            if (!string.IsNullOrEmpty(donHang.Nguoiduyetid))
            {
                ViewBag.NguoiDuyet = _userBLL.GetById(donHang.Nguoiduyetid);
            }
            
            return View(donHang);
        }

        public IActionResult Create()
        {
            ViewData["Makh"] = new SelectList(_khachHangBLL.GetAll(), "Makh", "Hotenkh");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(DonHang donHang)
        {
            if (ModelState.IsValid)
            {
                donHang.Ngaydat = DateTime.Now;
                donHang.Trangthai = "Chờ xử lý";
                
                var (success, message) = _donHangBLL.Insert(donHang);
                if (success)
                {
                    TempData["Success"] = message;
                    return RedirectToAction(nameof(Index));
                }
                ViewBag.Error = message;
            }
            ViewData["Makh"] = new SelectList(_khachHangBLL.GetAll(), "Makh", "Hotenkh", donHang.Makh);
            return View(donHang);
        }

        public IActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();
            var donHang = _donHangBLL.GetById(id);
            if (donHang == null) return NotFound();
            
            ViewData["Makh"] = new SelectList(_khachHangBLL.GetAll(), "Makh", "Hotenkh", donHang.Makh);
            return View(donHang);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(string id, DonHang donHang)
        {
            if (id != donHang.Madonhang) return NotFound();
            
            if (ModelState.IsValid)
            {
                var (success, message) = _donHangBLL.Update(donHang);
                if (success)
                {
                    TempData["Success"] = message;
                    return RedirectToAction(nameof(Index));
                }
                ViewBag.Error = message;
            }
            ViewData["Makh"] = new SelectList(_khachHangBLL.GetAll(), "Makh", "Hotenkh", donHang.Makh);
            return View(donHang);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateTrangThai(string id, string trangThai)
        {
            var userId = HttpContext.Session.GetString("AdminUserId");
            var (success, message) = _donHangBLL.UpdateTrangThai(id, trangThai, userId);
            
            if (success)
                TempData["Success"] = message;
            else
                TempData["Error"] = message;
                
            return RedirectToAction(nameof(Details), new { id });
        }

        // Chỉ Admin mới có quyền xóa đơn hàng
        public IActionResult Delete(string id)
        {
            // Kiểm tra quyền Admin
            var vaiTro = HttpContext.Session.GetString("AdminVaiTro");
            var adminRoles = new[] { "Admin", "QuanLy", "Quản trị hệ thống" };
            
            if (!adminRoles.Contains(vaiTro))
            {
                TempData["Error"] = "Bạn không có quyền xóa đơn hàng. Chỉ Admin mới có quyền này!";
                return RedirectToAction(nameof(Index));
            }
            
            if (string.IsNullOrEmpty(id)) return NotFound();
            var donHang = _donHangBLL.GetById(id);
            if (donHang == null) return NotFound();
            return View(donHang);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(string id)
        {
            // Kiểm tra quyền Admin
            var vaiTro = HttpContext.Session.GetString("AdminVaiTro");
            var adminRoles = new[] { "Admin", "QuanLy", "Quản trị hệ thống" };
            
            if (!adminRoles.Contains(vaiTro))
            {
                TempData["Error"] = "Bạn không có quyền xóa đơn hàng. Chỉ Admin mới có quyền này!";
                return RedirectToAction(nameof(Index));
            }
            
            var (success, message) = _donHangBLL.Delete(id);
            if (success)
                TempData["Success"] = message;
            else
                TempData["Error"] = message;
            return RedirectToAction(nameof(Index));
        }

        #region Sử dụng Stored Procedures

        // POST: Hủy đơn hàng (dùng sp_HuyDonHang - hoàn lại tồn kho)
        // Chỉ Admin mới có quyền hủy đơn hàng
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult HuyDonHang(string id)
        {
            // Kiểm tra quyền Admin
            var vaiTro = HttpContext.Session.GetString("AdminVaiTro");
            var adminRoles = new[] { "Admin", "QuanLy", "Quản trị hệ thống" };
            
            if (!adminRoles.Contains(vaiTro))
            {
                TempData["Error"] = "Bạn không có quyền hủy đơn hàng. Chỉ Admin mới có quyền này!";
                return RedirectToAction(nameof(Details), new { id });
            }
            
            var (success, message) = _spBLL.HuyDonHang(id);
            
            if (success)
                TempData["Success"] = "Đã hủy đơn hàng và hoàn lại tồn kho thành công!";
            else
                TempData["Error"] = message;
                
            return RedirectToAction(nameof(Details), new { id });
        }

        // POST: Xác nhận thanh toán (dùng sp_NV_XacNhanThanhToan)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult XacNhanThanhToan(string id)
        {
            var userId = HttpContext.Session.GetString("AdminUserId") ?? "";
            
            var (success, message) = _spBLL.XacNhanThanhToan(id, userId);
            
            if (success)
                TempData["Success"] = "Xác nhận thanh toán thành công!";
            else
                TempData["Error"] = message;
                
            return RedirectToAction(nameof(Details), new { id });
        }

        // POST: Xuất kho (dùng sp_NV_XuatKho)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult XuatKho(string id)
        {
            var userId = HttpContext.Session.GetString("AdminUserId") ?? "";
            
            var (success, message) = _spBLL.XuatKho(id, userId);
            
            if (success)
                TempData["Success"] = "Xuất kho thành công!";
            else
                TempData["Error"] = message;
                
            return RedirectToAction(nameof(Details), new { id });
        }

        // POST: Điều phối giao hàng (dùng sp_NV_DieuPhoiGiaoHang)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DieuPhoiGiaoHang(string id, string donViVanChuyen)
        {
            var userId = HttpContext.Session.GetString("AdminUserId") ?? "";
            
            var (success, message) = _spBLL.DieuPhoiGiaoHang(id, donViVanChuyen, userId);
            
            if (success)
                TempData["Success"] = "Điều phối giao hàng thành công!";
            else
                TempData["Error"] = message;
                
            return RedirectToAction(nameof(Details), new { id });
        }

        #endregion
    }
}
