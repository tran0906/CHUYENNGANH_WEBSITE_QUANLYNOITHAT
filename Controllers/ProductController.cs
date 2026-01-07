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

            // Sắp xếp - Mặc định là sản phẩm mới nhất lên đầu
            products = sort switch
            {
                "newest" => products.OrderByDescending(s => s.Masp).ToList(),
                "price-asc" => products.OrderBy(s => s.Giaban).ToList(),
                "price-desc" => products.OrderByDescending(s => s.Giaban).ToList(),
                "name-asc" => products.OrderBy(s => s.Tensp).ToList(),
                "name-desc" => products.OrderByDescending(s => s.Tensp).ToList(),
                _ => products.OrderByDescending(s => s.Masp).ToList() // Mặc định: Mới nhất
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
        public IActionResult BestSeller(int page = 1)
        {
            int pageSize = 12;
            
            // Lấy danh sách mã sản phẩm bán chạy từ database (dựa trên số lượng đã bán)
            var bestSellerIds = GetBestSellerProductIds(100); // Lấy top 100 sản phẩm bán chạy
            
            // Lấy thông tin chi tiết sản phẩm kèm giảm giá
            var allProducts = _sanPhamBLL.GetAllWithPromotion();
            var bestSellerProducts = allProducts
                .Where(p => bestSellerIds.Contains(p.Masp))
                .OrderBy(p => bestSellerIds.IndexOf(p.Masp)) // Giữ thứ tự bán chạy
                .ToList();
            
            // Nếu không có sản phẩm bán chạy, lấy sản phẩm mới nhất
            if (!bestSellerProducts.Any())
            {
                bestSellerProducts = allProducts.Take(20).ToList();
            }
            
            var totalItems = bestSellerProducts.Count;
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            
            var pagedProducts = bestSellerProducts
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            
            ViewBag.NhomSpList = _nhomSanPhamBLL.GetAll();
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalItems = totalItems;
            ViewBag.IsBestSeller = true;
            ViewBag.PageTitle = "Sản phẩm bán chạy";
            
            return View("BestSeller", pagedProducts);
        }
        
        // GET: /san-pham-giam-gia
        [Route("san-pham-giam-gia")]
        public IActionResult Sale(int page = 1)
        {
            int pageSize = 12;
            
            // Lấy sản phẩm đang giảm giá
            var allProducts = _sanPhamBLL.GetAllWithPromotion();
            var saleProducts = allProducts
                .Where(p => p.DangGiamGia)
                .OrderByDescending(p => p.PhanTramGiam) // Sắp xếp theo % giảm cao nhất
                .ToList();
            
            var totalItems = saleProducts.Count;
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);
            
            var pagedProducts = saleProducts
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            
            ViewBag.NhomSpList = _nhomSanPhamBLL.GetAll();
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;
            ViewBag.TotalItems = totalItems;
            
            return View("Sale", pagedProducts);
        }
        
        /// <summary>
        /// Lấy danh sách mã sản phẩm bán chạy (dựa trên số lượng đã bán từ đơn hàng hoàn thành)
        /// </summary>
        private List<string> GetBestSellerProductIds(int top)
        {
            try
            {
                string query = $@"SELECT TOP {top} ct.MASP
                                 FROM CT_DONHANG ct
                                 INNER JOIN DON_HANG dh ON ct.MADONHANG = dh.MADONHANG
                                 WHERE dh.TRANGTHAI = N'Hoàn thành'
                                 GROUP BY ct.MASP
                                 ORDER BY SUM(ct.SOLUONG) DESC";
                
                var dt = DAL.SqlConnectionHelper.ExecuteQuery(query);
                var result = new List<string>();
                foreach (System.Data.DataRow row in dt.Rows)
                {
                    if (row["MASP"] != DBNull.Value)
                    {
                        result.Add(row["MASP"].ToString() ?? "");
                    }
                }
                return result;
            }
            catch
            {
                return new List<string>();
            }
        }
    }
}
