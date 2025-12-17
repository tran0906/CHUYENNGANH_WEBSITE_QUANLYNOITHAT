using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using DOANCHUYENNGANH_WEB_QLNOITHAT.BLL;
using DOANCHUYENNGANH_WEB_QLNOITHAT.Models;
using DOANCHUYENNGANH_WEB_QLNOITHAT.Areas.Admin.Filters;

namespace DOANCHUYENNGANH_WEB_QLNOITHAT.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminOnlyFilter]
    public class NhanVienController : Controller
    {
        private readonly UserBLL _userBLL = new UserBLL();

        // GET: Admin/NhanVien
        public IActionResult Index(string? searchName, string? searchUser, string? vaiTro)
        {
            var users = _userBLL.Search(searchName, searchUser, vaiTro);

            ViewBag.SearchName = searchName;
            ViewBag.SearchUser = searchUser;
            ViewBag.VaiTro = vaiTro;
            ViewBag.VaiTroList = _userBLL.GetDistinctVaiTro();

            return View(users);
        }

        // GET: Admin/NhanVien/Details/5
        public IActionResult Details(string id)
        {
            if (id == null) return NotFound();

            var user = _userBLL.GetById(id);
            if (user == null) return NotFound();

            var stats = _userBLL.GetUserStats(id);
            ViewBag.SoDonHang = stats.SoDonHang;
            ViewBag.SoThanhToan = stats.SoThanhToan;
            ViewBag.SoVanChuyen = stats.SoVanChuyen;
            ViewBag.SoPhieuXuat = stats.SoPhieuXuat;

            return View(user);
        }

        // GET: Admin/NhanVien/Create
        public IActionResult Create()
        {
            ViewBag.VaiTroOptions = GetVaiTroOptions();
            return View();
        }

        // POST: Admin/NhanVien/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(User user)
        {
            // Kiểm tra UserId đã tồn tại chưa
            if (_userBLL.Exists(user.UserId))
            {
                ViewBag.Error = "Mã nhân viên đã tồn tại!";
                ViewBag.VaiTroOptions = GetVaiTroOptions();
                return View(user);
            }

            // Kiểm tra TenUser đã tồn tại chưa
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
                    TempData["Success"] = "Thêm nhân viên thành công!";
                    return RedirectToAction(nameof(Index));
                }
                ViewBag.Error = result.Message;
            }

            ViewBag.VaiTroOptions = GetVaiTroOptions();
            return View(user);
        }

        // GET: Admin/NhanVien/Edit/5
        public IActionResult Edit(string id)
        {
            if (id == null) return NotFound();

            var user = _userBLL.GetById(id);
            if (user == null) return NotFound();

            ViewBag.VaiTroOptions = GetVaiTroOptions();
            return View(user);
        }

        // POST: Admin/NhanVien/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(string id, User user, string? newPassword)
        {
            if (id != user.UserId) return NotFound();

            // Kiểm tra TenUser đã tồn tại chưa (trừ user hiện tại)
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

                // Chỉ cập nhật mật khẩu nếu có nhập mới
                if (!string.IsNullOrEmpty(newPassword))
                {
                    existingUser.MatKhau = newPassword;
                }

                var result = _userBLL.Update(existingUser);
                if (result.Success)
                {
                    TempData["Success"] = "Cập nhật thông tin nhân viên thành công!";
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", result.Message);
            }

            ViewBag.VaiTroOptions = GetVaiTroOptions();
            return View(user);
        }

        // GET: Admin/NhanVien/Delete/5
        public IActionResult Delete(string id)
        {
            if (id == null) return NotFound();

            var user = _userBLL.GetById(id);
            if (user == null) return NotFound();

            ViewBag.HasRelatedData = _userBLL.HasRelatedData(id);
            return View(user);
        }

        // POST: Admin/NhanVien/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(string id)
        {
            var result = _userBLL.Delete(id);
            if (result.Success)
                TempData["Success"] = "Xóa nhân viên thành công!";
            else
                TempData["Error"] = result.Message;

            return RedirectToAction(nameof(Index));
        }

        // POST: Admin/NhanVien/ResetPassword/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ResetPassword(string id)
        {
            var result = _userBLL.ResetPassword(id);
            if (result.Success)
                TempData["Success"] = result.Message;
            else
                TempData["Error"] = result.Message;

            return RedirectToAction(nameof(Index));
        }

        private List<SelectListItem> GetVaiTroOptions()
        {
            return new List<SelectListItem>
            {
                new SelectListItem { Value = "Quản trị hệ thống", Text = "Quản trị hệ thống" },
                new SelectListItem { Value = "Nhân viên", Text = "Nhân viên" },
                new SelectListItem { Value = "Kế toán", Text = "Kế toán" },
                new SelectListItem { Value = "Kho", Text = "Nhân viên kho" },
                new SelectListItem { Value = "Bán hàng", Text = "Nhân viên bán hàng" }
            };
        }
    }
}
