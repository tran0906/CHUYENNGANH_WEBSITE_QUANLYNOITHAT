using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using DOANCHUYENNGANH_WEB_QLNOITHAT.BLL;
using DOANCHUYENNGANH_WEB_QLNOITHAT.Models;
using DOANCHUYENNGANH_WEB_QLNOITHAT.Areas.Admin.Filters;

namespace DOANCHUYENNGANH_WEB_QLNOITHAT.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminOnlyFilter]
    public class QuanBaSpController : Controller
    {
        private readonly QuanBaSpBLL _bll = new QuanBaSpBLL();
        private readonly UserBLL _userBLL = new UserBLL();
        private readonly SanPhamBLL _sanPhamBLL = new SanPhamBLL();

        public IActionResult Index()
        {
            return View(_bll.GetAll());
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
            ViewData["Userid"] = new SelectList(_userBLL.GetAll(), "UserId", "HoTen");
            ViewData["SanPhams"] = _sanPhamBLL.GetAll();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(QuanBaSp obj)
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
            ViewData["Userid"] = new SelectList(_userBLL.GetAll(), "UserId", "HoTen", obj.Userid);
            ViewData["SanPhams"] = _sanPhamBLL.GetAll();
            return View(obj);
        }

        public IActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();
            var obj = _bll.GetById(id);
            if (obj == null) return NotFound();
            
            ViewData["Userid"] = new SelectList(_userBLL.GetAll(), "UserId", "HoTen", obj.Userid);
            ViewData["SanPhams"] = _sanPhamBLL.GetAll();
            return View(obj);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(string id, QuanBaSp obj)
        {
            if (id != obj.Madotgiamgia) return NotFound();
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
            ViewData["Userid"] = new SelectList(_userBLL.GetAll(), "UserId", "HoTen", obj.Userid);
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
