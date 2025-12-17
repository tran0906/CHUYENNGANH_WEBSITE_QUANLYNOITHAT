using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using DOANCHUYENNGANH_WEB_QLNOITHAT.BLL;
using DOANCHUYENNGANH_WEB_QLNOITHAT.Areas.Admin.Filters;

namespace DOANCHUYENNGANH_WEB_QLNOITHAT.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminOnlyFilter]
    public class SuDungController : Controller
    {
        private readonly SuDungBLL _suDungBLL = new SuDungBLL();
        private readonly MucDichSuDungBLL _mdsdBLL = new MucDichSuDungBLL();
        private readonly SanPhamBLL _spBLL = new SanPhamBLL();

        public IActionResult Index(string? mamdsd)
        {
            var mucDichSuDungs = _mdsdBLL.GetAll();

            if (!string.IsNullOrEmpty(mamdsd))
            {
                var selected = _mdsdBLL.GetById(mamdsd);
                if (selected != null)
                {
                    selected.Masps = _suDungBLL.GetSanPhamByMdsd(mamdsd);
                }
                ViewBag.SelectedMdsd = selected;
                ViewBag.MaMdsd = mamdsd;
            }

            ViewData["MucDichSuDungs"] = new SelectList(mucDichSuDungs, "Mamdsd", "Tenmdsd", mamdsd);
            return View(mucDichSuDungs);
        }

        public IActionResult Details(string mamdsd, string masp)
        {
            if (mamdsd == null || masp == null) return NotFound();

            var mucDich = _mdsdBLL.GetById(mamdsd);
            var sanPham = _spBLL.GetById(masp);

            if (mucDich == null || sanPham == null) return NotFound();

            ViewBag.MucDich = mucDich;
            ViewBag.SanPham = sanPham;
            return View();
        }

        public IActionResult Create()
        {
            ViewData["Mamdsd"] = new SelectList(_mdsdBLL.GetAll(), "Mamdsd", "Tenmdsd");
            ViewData["Masp"] = new SelectList(_spBLL.GetAll(), "Masp", "Tensp");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(string mamdsd, string masp)
        {
            var result = _suDungBLL.Insert(mamdsd, masp);
            
            if (!result.Success)
            {
                ViewBag.Error = result.Message;
                ViewData["Mamdsd"] = new SelectList(_mdsdBLL.GetAll(), "Mamdsd", "Tenmdsd", mamdsd);
                ViewData["Masp"] = new SelectList(_spBLL.GetAll(), "Masp", "Tensp", masp);
                return View();
            }

            return RedirectToAction(nameof(Index), new { mamdsd });
        }

        public IActionResult Delete(string mamdsd, string masp)
        {
            if (mamdsd == null || masp == null) return NotFound();

            var mucDich = _mdsdBLL.GetById(mamdsd);
            var sanPham = _spBLL.GetById(masp);

            if (mucDich == null || sanPham == null) return NotFound();

            ViewBag.MucDich = mucDich;
            ViewBag.SanPham = sanPham;
            return View();
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(string mamdsd, string masp)
        {
            _suDungBLL.Delete(mamdsd, masp);
            return RedirectToAction(nameof(Index), new { mamdsd });
        }
    }
}
