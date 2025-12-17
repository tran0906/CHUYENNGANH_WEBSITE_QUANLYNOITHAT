using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using DOANCHUYENNGANH_WEB_QLNOITHAT.BLL;
using DOANCHUYENNGANH_WEB_QLNOITHAT.Models;
using DOANCHUYENNGANH_WEB_QLNOITHAT.Areas.Admin.Filters;

namespace DOANCHUYENNGANH_WEB_QLNOITHAT.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthFilter]
    public class ThanhToanController : Controller
    {
        private readonly ThanhToanBLL _bll = new ThanhToanBLL();
        private readonly DonHangBLL _donHangBLL = new DonHangBLL();
        private readonly UserBLL _userBLL = new UserBLL();

        public IActionResult Index(string? phuongThuc)
        {
            var list = _bll.GetAll();
            
            if (!string.IsNullOrEmpty(phuongThuc))
            {
                list = list.Where(t => t.Phuongthuc == phuongThuc).ToList();
            }

            var phuongThucList = _bll.GetAll()
                .Where(t => t.Phuongthuc != null)
                .Select(t => t.Phuongthuc)
                .Distinct()
                .ToList();

            ViewBag.PhuongThucList = phuongThucList;
            ViewBag.CurrentPhuongThuc = phuongThuc;

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
        public IActionResult Create(ThanhToan obj)
        {
            if (ModelState.IsValid)
            {
                obj.Ngaythanhtoan = DateTime.Now;
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
        public IActionResult Edit(string id, ThanhToan obj)
        {
            if (id != obj.Mathanhtoan) return NotFound();
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
