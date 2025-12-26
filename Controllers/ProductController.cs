using Microsoft.AspNetCore.Mvc;
using DOANCHUYENNGANH_WEB_QLNOITHAT.BLL;
using DOANCHUYENNGANH_WEB_QLNOITHAT.Models;

namespace DOANCHUYENNGANH_WEB_QLNOITHAT.Controllers
{
    public class ProductController : Controller
    {
        private readonly SanPhamBLL _sanPhamBLL = new SanPhamBLL();
        private readonly NhomSanPhamBLL _nhomSanPhamBLL = new NhomSanPhamBLL();
        private readonly MucDichSuDungBLL _mucDichBLL = new MucDichSuDungBLL();
        private readonly SuDungBLL _suDungBLL = new SuDungBLL();

        // GET: /san-pham
        [Route("san-pham")]
        public IActionResult Index(string? nhomsp, string? search, decimal? minPrice, decimal? maxPrice, string? sort, int page = 1)
        {
            int pageSize = 12;
            List<SanPham> products;

            // Lấy sản phẩm kèm thông tin giảm giá
            var allProducts = _sanPhamBLL.GetAllWithPromotion();

            // Lọc theo nhóm sản phẩm - từ sidebar
            if (!string.IsNullOrEmpty(nhomsp))
            {
                products = allProducts.Where(s => s.Manhomsp == nhomsp).ToList();
            }
            else
            {
                products = allProducts;
            }

            // Tìm kiếm
            if (!string.IsNullOrEmpty(search))
            {
                products = products.Where(s => 
                    (s.Tensp != null && s.Tensp.Contains(search, StringComparison.OrdinalIgnoreCase)) || 
                    (s.Mota != null && s.Mota.Contains(search, StringComparison.OrdinalIgnoreCase))).ToList();
            }

            // Lọc theo giá
            if (minPrice.HasValue)
            {
                products = products.Where(s => s.Giaban >= minPrice.Value).ToList();
            }
            if (maxPrice.HasValue)
            {
                products = products.Where(s => s.Giaban <= maxPrice.Value).ToList();
            }

            // Sắp xếp
            products = sort switch
            {
                "price-asc" => products.OrderBy(s => s.Giaban).ToList(),
                "price-desc" => products.OrderByDescending(s => s.Giaban).ToList(),
                "name-asc" => products.OrderBy(s => s.Tensp).ToList(),
                "name-desc" => products.OrderByDescending(s => s.Tensp).ToList(),
                _ => products.OrderByDescending(s => s.Masp).ToList()
            };

            var totalItems = products.Count;
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var pagedProducts = products
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Lấy danh sách cho sidebar
            ViewBag.NhomSpList = _nhomSanPhamBLL.GetAll();
            ViewBag.CurrentNhomSp = nhomsp;
            ViewBag.CurrentNhomSpName = !string.IsNullOrEmpty(nhomsp) ? _nhomSanPhamBLL.GetById(nhomsp)?.Tennhomsp : null;
            ViewBag.CurrentSearch = search;
            ViewBag.CurrentSort = sort;
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalItems = totalItems;

            return View(pagedProducts);
        }

        // GET: /san-pham/{id}
        [Route("san-pham/{id}")]
        public IActionResult Detail(string id)
        {
            // Lấy sản phẩm kèm thông tin giảm giá
            var product = _sanPhamBLL.GetByIdWithPromotion(id);

            if (product == null)
            {
                return NotFound();
            }

            // Sản phẩm liên quan (cũng kèm thông tin giảm giá)
            ViewBag.RelatedProducts = _sanPhamBLL.GetAllWithPromotion()
                .Where(s => s.Manhomsp == product.Manhomsp && s.Masp != id)
                .Take(4)
                .ToList();

            return View(product);
        }

        // GET: /san-pham-ban-chay
        [Route("san-pham-ban-chay")]
        public IActionResult BestSeller()
        {
            var products = _sanPhamBLL.GetAllWithPromotion().Take(20).ToList();
            ViewBag.Categories = _nhomSanPhamBLL.GetAll();
            return View("Index", products);
        }
    }
}
