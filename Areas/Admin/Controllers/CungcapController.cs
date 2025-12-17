using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using DOANCHUYENNGANH_WEB_QLNOITHAT.BLL;
using DOANCHUYENNGANH_WEB_QLNOITHAT.Models;
using DOANCHUYENNGANH_WEB_QLNOITHAT.Areas.Admin.Filters;

namespace DOANCHUYENNGANH_WEB_QLNOITHAT.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminOnlyFilter]
    public class CungcapController : Controller
    {
        private readonly CungcapBLL _bll = new CungcapBLL();
        private readonly NhaCungCapBLL _nhaCungCapBLL = new NhaCungCapBLL();
        private readonly SanPhamBLL _sanPhamBLL = new SanPhamBLL();

        public IActionResult Index(string? mancc)
        {
            var list = _bll.GetAll();
            if (!string.IsNullOrEmpty(mancc))
            {
                list = list.Where(c => c.Mancc == mancc).ToList();
                ViewBag.MaNCC = mancc;
            }
            ViewData["NhaCungCaps"] = new SelectList(_nhaCungCapBLL.GetAll(), "Mancc", "Tenncc", mancc);
            return View(list);
        }

        public IActionResult Details(string mancc, string masp)
        {
            if (string.IsNullOrEmpty(mancc) || string.IsNullOrEmpty(masp)) return NotFound();
            var obj = _bll.GetById(mancc, masp);
            if (obj == null) return NotFound();
            return View(obj);
        }

        public IActionResult Create()
        {
            ViewData["Mancc"] = new SelectList(_nhaCungCapBLL.GetAll(), "Mancc", "Tenncc");
            ViewData["Masp"] = new SelectList(_sanPhamBLL.GetAll(), "Masp", "Tensp");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Cungcap obj)
        {
            if (ModelState.IsValid)
            {
                var (success, message) = _bll.Insert(obj);
                if (success)
                {
                    TempData["Success"] = message;
                    return RedirectToAction(nameof(Index));
                }
                ViewBag.Error = message;
            }
            ViewData["Mancc"] = new SelectList(_nhaCungCapBLL.GetAll(), "Mancc", "Tenncc", obj.Mancc);
            ViewData["Masp"] = new SelectList(_sanPhamBLL.GetAll(), "Masp", "Tensp", obj.Masp);
            return View(obj);
        }

        public IActionResult Edit(string mancc, string masp)
        {
            if (string.IsNullOrEmpty(mancc) || string.IsNullOrEmpty(masp)) return NotFound();
            var obj = _bll.GetById(mancc, masp);
            if (obj == null) return NotFound();
            
            ViewData["Mancc"] = new SelectList(_nhaCungCapBLL.GetAll(), "Mancc", "Tenncc", obj.Mancc);
            ViewData["Masp"] = new SelectList(_sanPhamBLL.GetAll(), "Masp", "Tensp", obj.Masp);
            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(string mancc, string masp, Cungcap obj)
        {
            if (mancc != obj.Mancc || masp != obj.Masp) return NotFound();
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
            ViewData["Mancc"] = new SelectList(_nhaCungCapBLL.GetAll(), "Mancc", "Tenncc", obj.Mancc);
            ViewData["Masp"] = new SelectList(_sanPhamBLL.GetAll(), "Masp", "Tensp", obj.Masp);
            return View(obj);
        }

        public IActionResult Delete(string mancc, string masp)
        {
            if (string.IsNullOrEmpty(mancc) || string.IsNullOrEmpty(masp)) return NotFound();
            var obj = _bll.GetById(mancc, masp);
            if (obj == null) return NotFound();
            return View(obj);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(string mancc, string masp)
        {
            var (success, message) = _bll.Delete(mancc, masp);
            if (success)
                TempData["Success"] = message;
            else
                TempData["Error"] = message;
            return RedirectToAction(nameof(Index));
        }
    }
}
