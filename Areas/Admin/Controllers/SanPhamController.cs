using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using DOANCHUYENNGANH_WEB_QLNOITHAT.BLL;
using DOANCHUYENNGANH_WEB_QLNOITHAT.Models;
using DOANCHUYENNGANH_WEB_QLNOITHAT.Areas.Admin.Filters;

namespace DOANCHUYENNGANH_WEB_QLNOITHAT.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminOnlyFilter]
    public class SanPhamController : Controller
    {
        private readonly SanPhamBLL _sanPhamBLL = new SanPhamBLL();
        private readonly NhomSanPhamBLL _nhomSanPhamBLL = new NhomSanPhamBLL();
        private readonly VatLieuBLL _vatLieuBLL = new VatLieuBLL();
        private readonly IWebHostEnvironment _env;

        public SanPhamController(IWebHostEnvironment env)
        {
            _env = env;
        }

        private List<string> GetExistingImages()
        {
            var imagesPath = Path.Combine(_env.WebRootPath, "images", "products");
            if (!Directory.Exists(imagesPath)) return new List<string>();
            
            return Directory.GetFiles(imagesPath)
                .Select(f => "/images/products/" + Path.GetFileName(f))
                .ToList();
        }

        // GET: Admin/SanPham/DanhSach - Cho nhân viên xem (chỉ xem, không thao tác)
        [SkipAdminOnlyFilter]
        [AdminAuthFilter]
        public IActionResult DanhSach(string? search, string? nhomSp, string? vatLieu)
        {
            var sanPhams = _sanPhamBLL.Search(search, nhomSp, vatLieu);

            ViewBag.Search = search;
            ViewBag.NhomSp = nhomSp;
            ViewBag.VatLieu = vatLieu;

            ViewData["NhomSanPhams"] = new SelectList(_nhomSanPhamBLL.GetAll(), "Manhomsp", "Tennhomsp", nhomSp);
            ViewData["VatLieus"] = new SelectList(_vatLieuBLL.GetAll(), "Mavl", "Tenvl", vatLieu);

            return View(sanPhams);
        }

        // GET: Admin/SanPham
        public IActionResult Index(string? search, string? nhomSp, string? vatLieu)
        {
            var sanPhams = _sanPhamBLL.Search(search, nhomSp, vatLieu);

            ViewBag.Search = search;
            ViewBag.NhomSp = nhomSp;
            ViewBag.VatLieu = vatLieu;

            ViewData["NhomSanPhams"] = new SelectList(_nhomSanPhamBLL.GetAll(), "Manhomsp", "Tennhomsp", nhomSp);
            ViewData["VatLieus"] = new SelectList(_vatLieuBLL.GetAll(), "Mavl", "Tenvl", vatLieu);

            return View(sanPhams);
        }

        // GET: Admin/SanPham/Details/5
        public IActionResult Details(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();
            
            var sanPham = _sanPhamBLL.GetById(id);
            if (sanPham == null) return NotFound();
            
            return View(sanPham);
        }

        // GET: Admin/SanPham/Create
        public IActionResult Create()
        {
            ViewData["Manhomsp"] = new SelectList(_nhomSanPhamBLL.GetAll(), "Manhomsp", "Tennhomsp");
            ViewData["Mavl"] = new SelectList(_vatLieuBLL.GetAll(), "Mavl", "Tenvl");
            ViewBag.ExistingImages = GetExistingImages();
            return View();
        }

        // POST: Admin/SanPham/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SanPham sanPham, List<IFormFile>? imageFiles)
        {
            if (ModelState.IsValid)
            {
                // Upload nhiều hình nếu có
                if (imageFiles != null && imageFiles.Count > 0)
                {
                    var uploadedPaths = new List<string>();
                    foreach (var file in imageFiles)
                    {
                        if (file.Length > 0)
                        {
                            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                            var uploadPath = Path.Combine(_env.WebRootPath, "images", "products", fileName);
                            using (var stream = new FileStream(uploadPath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }
                            uploadedPaths.Add("/images/products/" + fileName);
                        }
                    }
                    // Nếu có upload mới, thêm vào danh sách hình đã chọn
                    if (uploadedPaths.Count > 0)
                    {
                        var existingImages = string.IsNullOrEmpty(sanPham.Hinhanh) 
                            ? new List<string>() 
                            : sanPham.Hinhanh.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList();
                        existingImages.AddRange(uploadedPaths);
                        sanPham.Hinhanh = string.Join(";", existingImages.Where(x => !string.IsNullOrWhiteSpace(x)));
                    }
                }
                
                // Đảm bảo Hinhanh không có phần tử rỗng
                if (!string.IsNullOrEmpty(sanPham.Hinhanh))
                {
                    var cleanImages = sanPham.Hinhanh.Split(';', StringSplitOptions.RemoveEmptyEntries)
                        .Where(x => !string.IsNullOrWhiteSpace(x))
                        .ToList();
                    sanPham.Hinhanh = cleanImages.Count > 0 ? string.Join(";", cleanImages) : null;
                }

                var (success, message) = _sanPhamBLL.Insert(sanPham);
                if (success)
                {
                    TempData["Success"] = message;
                    return RedirectToAction(nameof(Index));
                }
                ViewBag.Error = message;
            }

            ViewData["Manhomsp"] = new SelectList(_nhomSanPhamBLL.GetAll(), "Manhomsp", "Tennhomsp", sanPham.Manhomsp);
            ViewData["Mavl"] = new SelectList(_vatLieuBLL.GetAll(), "Mavl", "Tenvl", sanPham.Mavl);
            ViewBag.ExistingImages = GetExistingImages();
            return View(sanPham);
        }

        // GET: Admin/SanPham/Edit/5
        public IActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();
            
            var sanPham = _sanPhamBLL.GetById(id);
            if (sanPham == null) return NotFound();

            ViewData["Manhomsp"] = new SelectList(_nhomSanPhamBLL.GetAll(), "Manhomsp", "Tennhomsp", sanPham.Manhomsp);
            ViewData["Mavl"] = new SelectList(_vatLieuBLL.GetAll(), "Mavl", "Tenvl", sanPham.Mavl);
            ViewBag.ExistingImages = GetExistingImages();
            return View(sanPham);
        }

        // POST: Admin/SanPham/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, SanPham sanPham, List<IFormFile>? imageFiles)
        {
            if (id != sanPham.Masp) return NotFound();

            if (ModelState.IsValid)
            {
                // Upload nhiều hình mới nếu có
                if (imageFiles != null && imageFiles.Count > 0)
                {
                    var uploadedPaths = new List<string>();
                    foreach (var file in imageFiles)
                    {
                        if (file.Length > 0)
                        {
                            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                            var uploadPath = Path.Combine(_env.WebRootPath, "images", "products", fileName);
                            using (var stream = new FileStream(uploadPath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }
                            uploadedPaths.Add("/images/products/" + fileName);
                        }
                    }
                    // Thêm hình upload mới vào danh sách
                    if (uploadedPaths.Count > 0)
                    {
                        var existingImages = string.IsNullOrEmpty(sanPham.Hinhanh) 
                            ? new List<string>() 
                            : sanPham.Hinhanh.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList();
                        existingImages.AddRange(uploadedPaths);
                        sanPham.Hinhanh = string.Join(";", existingImages.Where(x => !string.IsNullOrWhiteSpace(x)));
                    }
                }
                
                // Đảm bảo Hinhanh không có phần tử rỗng
                if (!string.IsNullOrEmpty(sanPham.Hinhanh))
                {
                    var cleanImages = sanPham.Hinhanh.Split(';', StringSplitOptions.RemoveEmptyEntries)
                        .Where(x => !string.IsNullOrWhiteSpace(x))
                        .ToList();
                    sanPham.Hinhanh = cleanImages.Count > 0 ? string.Join(";", cleanImages) : null;
                }

                var (success, message) = _sanPhamBLL.Update(sanPham);
                if (success)
                {
                    TempData["Success"] = message;
                    return RedirectToAction(nameof(Index));
                }
                ViewBag.Error = message;
            }

            ViewData["Manhomsp"] = new SelectList(_nhomSanPhamBLL.GetAll(), "Manhomsp", "Tennhomsp", sanPham.Manhomsp);
            ViewData["Mavl"] = new SelectList(_vatLieuBLL.GetAll(), "Mavl", "Tenvl", sanPham.Mavl);
            ViewBag.ExistingImages = GetExistingImages();
            return View(sanPham);
        }

        // GET: Admin/SanPham/Delete/5
        public IActionResult Delete(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();
            
            var sanPham = _sanPhamBLL.GetById(id);
            if (sanPham == null) return NotFound();
            
            return View(sanPham);
        }

        // POST: Admin/SanPham/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(string id)
        {
            var (success, message) = _sanPhamBLL.Delete(id);
            if (success)
            {
                TempData["Success"] = message;
            }
            else
            {
                TempData["Error"] = message;
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
