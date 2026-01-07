using System.Diagnostics;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using DOANCHUYENNGANH_WEB_QLNOITHAT.BLL;
using DOANCHUYENNGANH_WEB_QLNOITHAT.Models;
using DOANCHUYENNGANH_WEB_QLNOITHAT.DAL;

namespace DOANCHUYENNGANH_WEB_QLNOITHAT.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SanPhamBLL _sanPhamBLL = new SanPhamBLL();
        private readonly NhomSanPhamBLL _nhomSanPhamBLL = new NhomSanPhamBLL();
        private readonly QuangBaBLL _quangBaBLL = new QuangBaBLL();

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            // Lấy tất cả sản phẩm kèm thông tin giảm giá
            var allProducts = _sanPhamBLL.GetAllWithPromotion();
            
            // 1. SẢN PHẨM BÁN CHẠY = Sản phẩm bán chạy (KHÔNG đang giảm giá)
            var bestSellerIds = GetBestSellerProductIds(20); // Lấy nhiều hơn để lọc
            var featuredProducts = allProducts
                .Where(p => bestSellerIds.Contains(p.Masp) && !p.DangGiamGia) // Loại bỏ SP đang giảm giá
                .OrderBy(p => bestSellerIds.IndexOf(p.Masp))
                .Take(8)
                .ToList();
            // Nếu chưa đủ 8 sản phẩm, lấy thêm SP không giảm giá có tồn kho cao
            if (featuredProducts.Count < 8)
            {
                var existingIds = featuredProducts.Select(p => p.Masp).ToList();
                var moreProducts = allProducts
                    .Where(p => !existingIds.Contains(p.Masp) && !p.DangGiamGia)
                    .OrderByDescending(p => p.Soluongton)
                    .Take(8 - featuredProducts.Count);
                featuredProducts.AddRange(moreProducts);
            }
            ViewBag.FeaturedProducts = featuredProducts;

            // 2. SẢN PHẨM MỚI = Sản phẩm mới thêm vào (KHÔNG đang giảm giá)
            ViewBag.NewProducts = allProducts
                .Where(p => !p.DangGiamGia) // Loại bỏ sản phẩm đang giảm giá
                .OrderByDescending(s => s.Masp)
                .Take(8)
                .ToList();

            // 3. SẢN PHẨM GIẢM GIÁ = Sản phẩm đang trong đợt khuyến mãi
            ViewBag.SaleProducts = allProducts
                .Where(p => p.DangGiamGia)
                .OrderByDescending(p => p.PhanTramGiam)
                .Take(8)
                .ToList();

            // Lấy danh sách SP đang được quảng bá (MASP -> % giảm) - dùng cho các section khác
            ViewBag.PromotedProducts = _quangBaBLL.GetAllPromotedProducts();
            ViewBag.Categories = _nhomSanPhamBLL.GetAll();

            return View();
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
                
                var dt = SqlConnectionHelper.ExecuteQuery(query);
                var result = new List<string>();
                foreach (DataRow row in dt.Rows)
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

        [Route("gioi-thieu")]
        public IActionResult About()
        {
            return View();
        }

        [Route("lien-he")]
        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        [Route("lien-he")]
        public IActionResult Contact(string name, string email, string phone, string subject, string message)
        {
            TempData["Success"] = "Cảm ơn bạn đã liên hệ! Chúng tôi sẽ phản hồi sớm nhất.";
            return RedirectToAction("Contact");
        }

        [Route("tin-tuc")]
        public IActionResult News()
        {
            return View();
        }

        [Route("dich-vu")]
        public IActionResult Services()
        {
            return View();
        }

        [Route("dieu-khoan")]
        public IActionResult Terms()
        {
            return View();
        }

        [Route("chinh-sach-bao-mat")]
        public IActionResult PrivacyPolicy()
        {
            return View("Privacy");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
