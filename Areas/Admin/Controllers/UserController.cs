using Microsoft.AspNetCore.Mvc;
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

        public IActionResult Details(string id)
        {
            if (id == null) return NotFound();
            var user = _userBLL.GetById(id);
            if (user == null) return NotFound();
            return View(user);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(User user)
        {
            if (ModelState.IsValid)
            {
                var result = _userBLL.Insert(user);
                if (result.Success)
                {
                    TempData["Success"] = "Thêm người dùng thành công!";
                    return RedirectToAction(nameof(Index));
                }
                ViewBag.Error = result.Message;
            }
            return View(user);
        }

        public IActionResult Edit(string id)
        {
            if (id == null) return NotFound();
            var user = _userBLL.GetById(id);
            if (user == null) return NotFound();
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(string id, User user)
        {
            if (id != user.UserId) return NotFound();
            if (ModelState.IsValid)
            {
                var result = _userBLL.Update(user);
                if (result.Success)
                    return RedirectToAction(nameof(Index));
                ModelState.AddModelError("", result.Message);
            }
            return View(user);
        }

        public IActionResult Delete(string id)
        {
            if (id == null) return NotFound();
            var user = _userBLL.GetById(id);
            if (user == null) return NotFound();
            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(string id)
        {
            var result = _userBLL.Delete(id);
            if (!result.Success)
                TempData["Error"] = result.Message;
            return RedirectToAction(nameof(Index));
        }
    }
}
