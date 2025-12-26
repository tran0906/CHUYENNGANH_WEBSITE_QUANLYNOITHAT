using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DOANCHUYENNGANH_WEB_QLNOITHAT.BLL;
using DOANCHUYENNGANH_WEB_QLNOITHAT.Models;

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
            // Lấy sản phẩm nổi bật (8 sản phẩm mới nhất)
            ViewBag.FeaturedProducts = _sanPhamBLL.GetAll().Take(8).ToList();

            // Lấy sản phẩm mới
            ViewBag.NewProducts = _sanPhamBLL.GetAll()
                .OrderByDescending(s => s.Masp)
                .Take(4)
                .ToList();

            // Lấy danh sách SP đang được quảng bá (MASP -> % giảm)
            ViewBag.PromotedProducts = _quangBaBLL.GetAllPromotedProducts();
            ViewBag.Categories = _nhomSanPhamBLL.GetAll();

            return View();
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
