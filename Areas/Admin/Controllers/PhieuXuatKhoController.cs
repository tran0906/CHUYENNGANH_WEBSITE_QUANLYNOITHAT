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

        public IActionResult Index(string? searchMaPhieu, string? searchMaDon)
        {
            var list = _bll.GetAll();

            if (!string.IsNullOrEmpty(searchMaPhieu))
            {
                list = list.Where(p => p.Maphieuxuat.Contains(searchMaPhieu, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (!string.IsNullOrEmpty(searchMaDon))
            {
                list = list.Where(p => p.Madonhang != null && p.Madonhang.Contains(searchMaDon, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            ViewBag.SearchMaPhieu = searchMaPhieu;
            ViewBag.SearchMaDon = searchMaDon;

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
    }
}
