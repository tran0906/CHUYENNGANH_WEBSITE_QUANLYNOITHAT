using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using DOANCHUYENNGANH_WEB_QLNOITHAT.BLL;
using DOANCHUYENNGANH_WEB_QLNOITHAT.Models;
using DOANCHUYENNGANH_WEB_QLNOITHAT.Areas.Admin.Filters;

namespace DOANCHUYENNGANH_WEB_QLNOITHAT.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthFilter]
    public class VanChuyenController : Controller
    {
        private readonly VanChuyenBLL _bll = new VanChuyenBLL();
        private readonly DonHangBLL _donHangBLL = new DonHangBLL();
        private readonly UserBLL _userBLL = new UserBLL();

        public IActionResult Index(string? trangThai, string? search)
        {
            var list = _bll.GetAll();

            if (!string.IsNullOrEmpty(trangThai))
            {
                list = list.Where(v => v.Trangthaigiao == trangThai).ToList();
            }

            if (!string.IsNullOrEmpty(search))
            {
                list = list.Where(v => v.Mavandon.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    (v.Madonhang != null && v.Madonhang.Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                    (v.Donvivanchuyen != null && v.Donvivanchuyen.Contains(search, StringComparison.OrdinalIgnoreCase))).ToList();
            }

            ViewBag.CurrentTrangThai = trangThai;
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
            // Chỉ lấy đơn hàng có thể tạo vận chuyển (Đã xác nhận/Đang xử lý và chưa có vận chuyển)
            var donHangCanVC = _bll.GetDonHangCanVanChuyen();
            ViewData["Madonhang"] = new SelectList(donHangCanVC, "Madonhang", "Madonhang");
            ViewData["Userid"] = new SelectList(_userBLL.GetAll(), "UserId", "HoTen");
            
            if (!donHangCanVC.Any())
            {
                TempData["Warning"] = "Không có đơn hàng nào cần tạo vận chuyển. Chỉ đơn hàng ở trạng thái 'Đã xác nhận' hoặc 'Đang xử lý' và chưa có vận chuyển mới có thể tạo.";
            }
            
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(VanChuyen obj)
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
            var donHangCanVC = _bll.GetDonHangCanVanChuyen();
            ViewData["Madonhang"] = new SelectList(donHangCanVC, "Madonhang", "Madonhang", obj.Madonhang);
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
        public IActionResult Edit(string id, VanChuyen obj)
        {
            if (id != obj.Mavandon) return NotFound();
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
