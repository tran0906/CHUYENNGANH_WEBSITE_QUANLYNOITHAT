using Microsoft.AspNetCore.Mvc;
using DOANCHUYENNGANH_WEB_QLNOITHAT.BLL;
using DOANCHUYENNGANH_WEB_QLNOITHAT.Models;

namespace DOANCHUYENNGANH_WEB_QLNOITHAT.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AccountController : Controller
    {
        private readonly UserBLL _userBLL = new UserBLL();

        // GET: Admin/Account/Login
        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("AdminUserId") != null)
            {
                return RedirectToAction("Index", "Dashboard");
            }
            return View();
        }

        // POST: Admin/Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(string tenUser, string matKhau)
        {
            var (success, message, user) = _userBLL.Login(tenUser, matKhau);

            if (!success || user == null)
            {
                ViewBag.Error = message;
                return View();
            }

            // Lưu thông tin vào Session
            HttpContext.Session.SetString("AdminUserId", user.UserId);
            HttpContext.Session.SetString("AdminUserName", user.TenUser ?? "");
            HttpContext.Session.SetString("AdminHoTen", user.HoTen ?? "");
            HttpContext.Session.SetString("AdminVaiTro", user.VaiTro ?? "");

            return RedirectToAction("Index", "Dashboard");
        }

        // GET: Admin/Account/Profile
        public IActionResult Profile()
        {
            var userId = HttpContext.Session.GetString("AdminUserId");
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login");
            }

            var user = _userBLL.GetById(userId);
            if (user == null)
            {
                return RedirectToAction("Login");
            }

            return View(user);
        }

        // POST: Admin/Account/Profile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Profile(User user)
        {
            var userId = HttpContext.Session.GetString("AdminUserId");
            if (string.IsNullOrEmpty(userId) || userId != user.UserId)
            {
                return RedirectToAction("Login");
            }

            var (success, message) = _userBLL.UpdateProfile(
                userId, 
                user.HoTen, 
                user.TenUser, 
                !string.IsNullOrEmpty(user.MatKhau) ? user.MatKhau : null
            );

            if (success)
            {
                HttpContext.Session.SetString("AdminUserName", user.TenUser ?? "");
                HttpContext.Session.SetString("AdminHoTen", user.HoTen ?? "");
                ViewBag.Success = message;
            }
            else
            {
                ViewBag.Error = message;
            }

            // Lấy lại thông tin user để hiển thị
            var updatedUser = _userBLL.GetById(userId);
            return View(updatedUser ?? user);
        }

        // GET: Admin/Account/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("AdminUserId");
            HttpContext.Session.Remove("AdminUserName");
            HttpContext.Session.Remove("AdminHoTen");
            HttpContext.Session.Remove("AdminVaiTro");
            return RedirectToAction("Login");
        }

        // GET: Admin/Account/AccessDenied
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
