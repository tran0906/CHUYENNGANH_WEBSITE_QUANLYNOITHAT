using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using DOANCHUYENNGANH_WEB_QLNOITHAT.BLL;
using DOANCHUYENNGANH_WEB_QLNOITHAT.Models;
using DOANCHUYENNGANH_WEB_QLNOITHAT.Areas.Admin.Filters;

namespace DOANCHUYENNGANH_WEB_QLNOITHAT.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminOnlyFilter]
    public class UserController : Controller
    {
        private readonly UserBLL _userBLL = new UserBLL();

        public IActionResult Index()
        {
            return View(_userBLL.GetAll());
        }

        public IActionResult Create()
        {
            ViewBag.VaiTroOptions = GetVaiTroOptions();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(User user)
        {
            if (_userBLL.Exists(user.UserId))
            {
                ViewBag.Error = "Mã người dùng đã tồn tại!";
                ViewBag.VaiTroOptions = GetVaiTroOptions();
                return View(user);
            }

            if (_userBLL.ExistsByTenUser(user.TenUser ?? ""))
            {
                ViewBag.Error = "Tên đăng nhập đã tồn tại!";
                ViewBag.VaiTroOptions = GetVaiTroOptions();
                return View(user);
            }

            if (ModelState.IsValid)
            {
                user.NgayTao = DateTime.Now;
                var result = _userBLL.Insert(user);
                if (result.Success)
                {
                    TempData["Success"] = "Thêm người dùng thành công!";
                    return RedirectToAction(nameof(Index));
                }
                ViewBag.Error = result.Message;
            }
            ViewBag.VaiTroOptions = GetVaiTroOptions();
            return View(user);
        }

        public IActionResult Edit(string id)
        {
            if (id == null) return NotFound();
            var user = _userBLL.GetById(id);
            if (user == null) return NotFound();
            ViewBag.VaiTroOptions = GetVaiTroOptions();
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(string id, User user, string? newPassword)
        {
            if (id != user.UserId) return NotFound();

            if (_userBLL.ExistsByTenUser(user.TenUser ?? "", id))
            {
                ModelState.AddModelError("TenUser", "Tên đăng nhập đã tồn tại");
                ViewBag.VaiTroOptions = GetVaiTroOptions();
                return View(user);
            }

            if (ModelState.IsValid)
            {
                var existingUser = _userBLL.GetById(id);
                if (existingUser == null) return NotFound();

                existingUser.TenUser = user.TenUser;
                existingUser.HoTen = user.HoTen;
                existingUser.VaiTro = user.VaiTro;

                if (!string.IsNullOrEmpty(newPassword))
                {
                    existingUser.MatKhau = newPassword;
                }

                var result = _userBLL.Update(existingUser);
                if (result.Success)
                {
                    TempData["Success"] = "Cập nhật người dùng thành công!";
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", result.Message);
            }
            ViewBag.VaiTroOptions = GetVaiTroOptions();
            return View(user);
        }

        public IActionResult Delete(string id)
        {
            if (id == null) return NotFound();
            var user = _userBLL.GetById(id);
            if (user == null) return NotFound();
            ViewBag.HasRelatedData = _userBLL.HasRelatedData(id);
            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(string id)
        {
            var result = _userBLL.Delete(id);
            if (result.Success)
                TempData["Success"] = "Xóa người dùng thành công!";
            else
                TempData["Error"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        private List<SelectListItem> GetVaiTroOptions()
        {
            return new List<SelectListItem>
            {
                new SelectListItem { Value = "Nhân viên", Text = "Nhân viên" },
                new SelectListItem { Value = "Quản trị hệ thống", Text = "Quản trị hệ thống" }
            };
        }
    }
}
