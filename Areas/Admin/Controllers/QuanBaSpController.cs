using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using DOANCHUYENNGANH_WEB_QLNOITHAT.BLL;
using DOANCHUYENNGANH_WEB_QLNOITHAT.Models;
using DOANCHUYENNGANH_WEB_QLNOITHAT.Areas.Admin.Filters;

namespace DOANCHUYENNGANH_WEB_QLNOITHAT.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthFilter] // Cho phép cả Admin và Nhân viên truy cập
    public class QuanBaSpController : Controller
    {
        private readonly QuanBaSpBLL _bll = new QuanBaSpBLL();
        private readonly QuangBaBLL _quangBaBLL = new QuangBaBLL();
        private readonly UserBLL _userBLL = new UserBLL();
        private readonly SanPhamBLL _sanPhamBLL = new SanPhamBLL();

        public IActionResult Index(string? search, string? trangthai)
        {
            var list = _bll.GetAll();
            
            // Lọc theo tìm kiếm
            if (!string.IsNullOrEmpty(search))
            {
                list = list.Where(q => q.Madotgiamgia.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
            }
            
            // Lọc theo trạng thái
            if (!string.IsNullOrEmpty(trangthai))
            {
                var now = DateTime.Now;
                list = trangthai switch
                {
                    "dangdiễnra" => list.Where(q => q.Ngaybatdau <= now && q.Ngayketthuc >= now).ToList(),
                    "sapdiễnra" => list.Where(q => q.Ngaybatdau > now).ToList(),
                    "daketthuc" => list.Where(q => q.Ngayketthuc < now).ToList(),
                    _ => list
                };
            }
            
            ViewBag.Search = search;
            ViewBag.TrangThai = trangthai;
            return View(list);
        }

        public IActionResult Details(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();
            var obj = _bll.GetById(id);
            if (obj == null) return NotFound();
            
            // Load danh sách SP trong đợt giảm giá
            obj.Masps = _quangBaBLL.GetSanPhamByDotGiamGia(id);
            
            return View(obj);
        }

        public IActionResult Create()
        {
            ViewData["Userid"] = new SelectList(_userBLL.GetAll(), "UserId", "HoTen");
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
            return View(obj);
        }

        public IActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();
            var obj = _bll.GetById(id);
            if (obj == null) return NotFound();
            
            ViewData["Userid"] = new SelectList(_userBLL.GetAll(), "UserId", "HoTen", obj.Userid);
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

        // ========== QUẢN LÝ SẢN PHẨM TRONG ĐỢT GIẢM GIÁ ==========

        /// <summary>
        /// Xem danh sách SP trong đợt giảm giá
        /// </summary>
        public IActionResult QuanLySanPham(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();
            
            var dotGiamGia = _bll.GetById(id);
            if (dotGiamGia == null) return NotFound();

            var sanPhamTrongDot = _quangBaBLL.GetSanPhamByDotGiamGia(id);
            var tatCaSanPham = _sanPhamBLL.GetAll();
            
            // Lọc ra SP chưa được thêm vào đợt này
            var sanPhamChuaThem = tatCaSanPham
                .Where(sp => !sanPhamTrongDot.Any(s => s.Masp == sp.Masp))
                .ToList();

            ViewBag.DotGiamGia = dotGiamGia;
            ViewBag.SanPhamChuaThem = sanPhamChuaThem;
            
            return View(sanPhamTrongDot);
        }

        /// <summary>
        /// Thêm SP vào đợt giảm giá
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ThemSanPham(string madotgiamgia, string masp)
        {
            var (success, message) = _quangBaBLL.Insert(masp, madotgiamgia);
            if (success)
                TempData["Success"] = message;
            else
                TempData["Error"] = message;
            
            return RedirectToAction(nameof(QuanLySanPham), new { id = madotgiamgia });
        }

        /// <summary>
        /// Xóa SP khỏi đợt giảm giá
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult XoaSanPham(string madotgiamgia, string masp)
        {
            var (success, message) = _quangBaBLL.Delete(masp, madotgiamgia);
            if (success)
                TempData["Success"] = message;
            else
                TempData["Error"] = message;
            
            return RedirectToAction(nameof(QuanLySanPham), new { id = madotgiamgia });
        }
    }
}
