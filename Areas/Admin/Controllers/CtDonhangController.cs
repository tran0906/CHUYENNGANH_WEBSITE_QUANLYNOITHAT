using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using DOANCHUYENNGANH_WEB_QLNOITHAT.BLL;
using DOANCHUYENNGANH_WEB_QLNOITHAT.Models;
using DOANCHUYENNGANH_WEB_QLNOITHAT.Areas.Admin.Filters;

namespace DOANCHUYENNGANH_WEB_QLNOITHAT.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthFilter]
    public class CtDonhangController : Controller
    {
        private readonly CtDonhangBLL _bll = new CtDonhangBLL();
        private readonly DonHangBLL _donHangBLL = new DonHangBLL();
        private readonly SanPhamBLL _sanPhamBLL = new SanPhamBLL();

        public IActionResult Index(string? madonhang)
        {
            var list = string.IsNullOrEmpty(madonhang) 
                ? _bll.GetAll() 
                : _bll.GetByDonHang(madonhang);
            
            ViewBag.MaDonHang = madonhang;
            ViewData["DonHangs"] = new SelectList(_donHangBLL.GetAll(), "Madonhang", "Madonhang", madonhang);
            return View(list);
        }

        public IActionResult Details(string masp, string madonhang)
        {
            if (string.IsNullOrEmpty(masp) || string.IsNullOrEmpty(madonhang)) return NotFound();
            var obj = _bll.GetById(masp, madonhang);
            if (obj == null) return NotFound();
            return View(obj);
        }

        public IActionResult Create(string? madonhang)
        {
            ViewData["Madonhang"] = new SelectList(_donHangBLL.GetAll(), "Madonhang", "Madonhang", madonhang);
            ViewData["Masp"] = new SelectList(_sanPhamBLL.GetAll(), "Masp", "Tensp");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CtDonhang obj)
        {
            if (ModelState.IsValid)
            {
                // Lấy giá sản phẩm
                var sanPham = _sanPhamBLL.GetById(obj.Masp);
                if (sanPham != null)
                {
                    obj.Dongia = sanPham.Giaban;
                    obj.Thanhtien = (obj.Soluong ?? 0) * (sanPham.Giaban ?? 0);
                }

                var (success, message) = _bll.Insert(obj);
                if (success)
                {
                    TempData["Success"] = message;
                    return RedirectToAction(nameof(Index), new { madonhang = obj.Madonhang });
                }
                ViewBag.Error = message;
            }
            ViewData["Madonhang"] = new SelectList(_donHangBLL.GetAll(), "Madonhang", "Madonhang", obj.Madonhang);
            ViewData["Masp"] = new SelectList(_sanPhamBLL.GetAll(), "Masp", "Tensp", obj.Masp);
            return View(obj);
        }

        public IActionResult Edit(string masp, string madonhang)
        {
            if (string.IsNullOrEmpty(masp) || string.IsNullOrEmpty(madonhang)) return NotFound();
            var obj = _bll.GetById(masp, madonhang);
            if (obj == null) return NotFound();
            
            ViewData["Madonhang"] = new SelectList(_donHangBLL.GetAll(), "Madonhang", "Madonhang", obj.Madonhang);
            ViewData["Masp"] = new SelectList(_sanPhamBLL.GetAll(), "Masp", "Tensp", obj.Masp);
            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(string masp, string madonhang, CtDonhang obj)
        {
            if (masp != obj.Masp || madonhang != obj.Madonhang) return NotFound();
            
            if (ModelState.IsValid)
            {
                obj.Thanhtien = (obj.Soluong ?? 0) * (obj.Dongia ?? 0);
                var (success, message) = _bll.Update(obj);
                if (success)
                {
                    TempData["Success"] = message;
                    return RedirectToAction(nameof(Index), new { madonhang = obj.Madonhang });
                }
                ViewBag.Error = message;
            }
            ViewData["Madonhang"] = new SelectList(_donHangBLL.GetAll(), "Madonhang", "Madonhang", obj.Madonhang);
            ViewData["Masp"] = new SelectList(_sanPhamBLL.GetAll(), "Masp", "Tensp", obj.Masp);
            return View(obj);
        }

        public IActionResult Delete(string masp, string madonhang)
        {
            if (string.IsNullOrEmpty(masp) || string.IsNullOrEmpty(madonhang)) return NotFound();
            var obj = _bll.GetById(masp, madonhang);
            if (obj == null) return NotFound();
            return View(obj);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(string masp, string madonhang)
        {
            var (success, message) = _bll.Delete(masp, madonhang);
            if (success)
                TempData["Success"] = message;
            else
                TempData["Error"] = message;
            return RedirectToAction(nameof(Index), new { madonhang });
        }
    }
}
