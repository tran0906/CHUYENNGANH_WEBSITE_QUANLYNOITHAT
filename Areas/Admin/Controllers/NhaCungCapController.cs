using Microsoft.AspNetCore.Mvc;
using DOANCHUYENNGANH_WEB_QLNOITHAT.BLL;
using DOANCHUYENNGANH_WEB_QLNOITHAT.Models;
using DOANCHUYENNGANH_WEB_QLNOITHAT.Areas.Admin.Filters;

namespace DOANCHUYENNGANH_WEB_QLNOITHAT.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminOnlyFilter]
    public class NhaCungCapController : Controller
    {
        private readonly NhaCungCapBLL _bll = new NhaCungCapBLL();

        public IActionResult Index(string? searchName, string? searchPhone)
        {
            var list = _bll.GetAll();
            if (!string.IsNullOrEmpty(searchName))
            {
                list = list.Where(n => n.Tenncc != null && n.Tenncc.Contains(searchName, StringComparison.OrdinalIgnoreCase)).ToList();
                ViewBag.SearchName = searchName;
            }
            if (!string.IsNullOrEmpty(searchPhone))
            {
                list = list.Where(n => n.Sdtncc != null && n.Sdtncc.Contains(searchPhone)).ToList();
                ViewBag.SearchPhone = searchPhone;
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
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(NhaCungCap obj)
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
        public IActionResult Edit(string id, NhaCungCap obj)
        {
            if (id != obj.Mancc) return NotFound();
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
