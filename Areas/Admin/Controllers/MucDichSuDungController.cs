using Microsoft.AspNetCore.Mvc;
using DOANCHUYENNGANH_WEB_QLNOITHAT.BLL;
using DOANCHUYENNGANH_WEB_QLNOITHAT.Models;
using DOANCHUYENNGANH_WEB_QLNOITHAT.Areas.Admin.Filters;

namespace DOANCHUYENNGANH_WEB_QLNOITHAT.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthFilter]
    public class MucDichSuDungController : Controller
    {
        private readonly MucDichSuDungBLL _bll = new MucDichSuDungBLL();

        public IActionResult Index(string? search, int page = 1)
        {
            int pageSize = 10;
            var allItems = string.IsNullOrEmpty(search) ? _bll.GetAll() : _bll.Search(search);
            
            var totalItems = allItems.Count;
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            page = Math.Max(1, Math.Min(page, Math.Max(1, totalPages)));
            
            var pagedItems = allItems.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            
            ViewBag.Search = search;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalItems = totalItems;
            
            return View(pagedItems);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(MucDichSuDung model)
        {
            if (ModelState.IsValid)
            {
                var (success, message) = _bll.Insert(model);
                if (success)
                {
                    TempData["Success"] = message;
                    return RedirectToAction(nameof(Index));
                }
                TempData["Error"] = message;
            }
            return View(model);
        }

        public IActionResult Edit(string id)
        {
            var item = _bll.GetById(id);
            if (item == null)
            {
                TempData["Error"] = "Không tìm thấy mục đích sử dụng";
                return RedirectToAction(nameof(Index));
            }
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(MucDichSuDung model)
        {
            if (ModelState.IsValid)
            {
                var (success, message) = _bll.Update(model);
                if (success)
                {
                    TempData["Success"] = message;
                    return RedirectToAction(nameof(Index));
                }
                TempData["Error"] = message;
            }
            return View(model);
        }

        public IActionResult Delete(string id)
        {
            var item = _bll.GetById(id);
            if (item == null)
            {
                TempData["Error"] = "Không tìm thấy mục đích sử dụng";
                return RedirectToAction(nameof(Index));
            }
            return View(item);
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
