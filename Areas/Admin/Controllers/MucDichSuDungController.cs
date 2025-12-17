using Microsoft.AspNetCore.Mvc;
using DOANCHUYENNGANH_WEB_QLNOITHAT.BLL;
using DOANCHUYENNGANH_WEB_QLNOITHAT.Models;
using DOANCHUYENNGANH_WEB_QLNOITHAT.Areas.Admin.Filters;

namespace DOANCHUYENNGANH_WEB_QLNOITHAT.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminOnlyFilter]
    public class MucDichSuDungController : Controller
    {
        private readonly MucDichSuDungBLL _bll = new MucDichSuDungBLL();
        private readonly SanPhamBLL _sanPhamBLL = new SanPhamBLL();

        public IActionResult Index(string? search)
        {
            var list = _bll.GetAll();
            if (!string.IsNullOrEmpty(search))
            {
                list = list.Where(m => m.Tenmdsd != null && m.Tenmdsd.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
                ViewBag.Search = search;
            }
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
            ViewData["SanPhams"] = _sanPhamBLL.GetAll();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(MucDichSuDung obj)
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
            ViewData["SanPhams"] = _sanPhamBLL.GetAll();
            return View(obj);
        }

        public IActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();
            var obj = _bll.GetById(id);
            if (obj == null) return NotFound();
            ViewData["SanPhams"] = _sanPhamBLL.GetAll();
            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(string id, MucDichSuDung obj)
        {
            if (id != obj.Mamdsd) return NotFound();
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
            ViewData["SanPhams"] = _sanPhamBLL.GetAll();
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
