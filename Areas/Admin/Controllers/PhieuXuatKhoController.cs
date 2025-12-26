using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using DOANCHUYENNGANH_WEB_QLNOITHAT.BLL;
using DOANCHUYENNGANH_WEB_QLNOITHAT.Models;
using DOANCHUYENNGANH_WEB_QLNOITHAT.Areas.Admin.Filters;

namespace DOANCHUYENNGANH_WEB_QLNOITHAT.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthFilter]
    public class PhieuXuatKhoController : Controller
    {
        private readonly PhieuXuatKhoBLL _bll = new PhieuXuatKhoBLL();
        private readonly DonHangBLL _donHangBLL = new DonHangBLL();
        private readonly UserBLL _userBLL = new UserBLL();
        private readonly CtDonhangBLL _ctDonhangBLL = new CtDonhangBLL();
        private readonly KhachHangBLL _khachHangBLL = new KhachHangBLL();

        public IActionResult Index(string? search)
        {
            var list = _bll.GetAll();

            if (!string.IsNullOrEmpty(search))
            {
                list = list.Where(p => 
                    p.Maphieuxuat.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    (p.Madonhang != null && p.Madonhang.Contains(search, StringComparison.OrdinalIgnoreCase))
                ).ToList();
            }

            ViewBag.Search = search;

            return View(list);
        }

        public IActionResult Details(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();
            var obj = _bll.GetById(id);
            if (obj == null) return NotFound();
            return View(obj);
        }

        public IActionResult Create()
        {
            ViewData["Madonhang"] = new SelectList(_donHangBLL.GetAll(), "Madonhang", "Madonhang");
            ViewData["Userid"] = new SelectList(_userBLL.GetAll(), "UserId", "HoTen");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(PhieuXuatKho obj)
        {
            if (ModelState.IsValid)
            {
                obj.Ngayxuat = DateTime.Now;
                var (success, message) = _bll.Insert(obj);
                if (success)
                {
                    TempData["Success"] = message;
                    return RedirectToAction(nameof(Index));
                }
                ViewBag.Error = message;
            }
            ViewData["Madonhang"] = new SelectList(_donHangBLL.GetAll(), "Madonhang", "Madonhang", obj.Madonhang);
            ViewData["Userid"] = new SelectList(_userBLL.GetAll(), "UserId", "HoTen", obj.Userid);
            return View(obj);
        }

        public IActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();
            var obj = _bll.GetById(id);
            if (obj == null) return NotFound();
            
            ViewData["Madonhang"] = new SelectList(_donHangBLL.GetAll(), "Madonhang", "Madonhang", obj.Madonhang);
            ViewData["Userid"] = new SelectList(_userBLL.GetAll(), "UserId", "HoTen", obj.Userid);
            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(string id, PhieuXuatKho obj)
        {
            if (id != obj.Maphieuxuat) return NotFound();
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
            ViewData["Madonhang"] = new SelectList(_donHangBLL.GetAll(), "Madonhang", "Madonhang", obj.Madonhang);
            ViewData["Userid"] = new SelectList(_userBLL.GetAll(), "UserId", "HoTen", obj.Userid);
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

        // GET: In phiếu xuất kho
        public IActionResult Print(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();
            
            var phieuXuat = _bll.GetById(id);
            if (phieuXuat == null) return NotFound();
            
            // Lấy thông tin đơn hàng
            var donHang = _donHangBLL.GetById(phieuXuat.Madonhang);
            if (donHang != null)
            {
                donHang.CtDonhangs = _ctDonhangBLL.GetByDonHang(phieuXuat.Madonhang);
                phieuXuat.MadonhangNavigation = donHang;
            }
            
            // Lấy thông tin người duyệt
            if (!string.IsNullOrEmpty(phieuXuat.Manvduyet))
            {
                ViewBag.NguoiDuyet = _userBLL.GetById(phieuXuat.Manvduyet);
            }
            
            return View(phieuXuat);
        }

        // GET: In phiếu xuất kho theo mã đơn hàng
        public IActionResult PrintByDonHang(string maDonHang)
        {
            if (string.IsNullOrEmpty(maDonHang)) return NotFound();
            
            var phieuXuat = _bll.GetByDonHang(maDonHang);
            if (phieuXuat == null)
            {
                TempData["Error"] = "Đơn hàng này chưa có phiếu xuất kho";
                return RedirectToAction("Details", "DonHang", new { id = maDonHang });
            }
            
            return RedirectToAction(nameof(Print), new { id = phieuXuat.Maphieuxuat });
        }
    }
}
