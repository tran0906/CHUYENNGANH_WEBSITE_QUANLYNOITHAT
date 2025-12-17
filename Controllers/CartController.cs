using Microsoft.AspNetCore.Mvc;
using DOANCHUYENNGANH_WEB_QLNOITHAT.BLL;
using DOANCHUYENNGANH_WEB_QLNOITHAT.Models;
using System.Text.Json;

namespace DOANCHUYENNGANH_WEB_QLNOITHAT.Controllers
{
    public class CartItem
    {
        public string Masp { get; set; } = string.Empty;
        public string Tensp { get; set; } = string.Empty;
        public string? Hinhanh { get; set; }
        public decimal Giaban { get; set; }
        public int Soluong { get; set; }
        public decimal Thanhtien => Giaban * Soluong;
    }

    public class CartController : Controller
    {
        private readonly SanPhamBLL _sanPhamBLL = new SanPhamBLL();
        private readonly KhachHangBLL _khachHangBLL = new KhachHangBLL();
        private readonly DonHangBLL _donHangBLL = new DonHangBLL();
        private readonly CtDonhangBLL _ctDonhangBLL = new CtDonhangBLL();
        private const string CartCookieKey = "ShoppingCart";
        private const string CartSessionKey = "CartItems";

        private List<CartItem> GetCartItems()
        {
            var cartJson = HttpContext.Session.GetString(CartSessionKey);
            if (!string.IsNullOrEmpty(cartJson))
            {
                return JsonSerializer.Deserialize<List<CartItem>>(cartJson) ?? new List<CartItem>();
            }

            if (Request.Cookies.TryGetValue(CartCookieKey, out var cookieValue))
            {
                var items = JsonSerializer.Deserialize<List<CartItem>>(cookieValue) ?? new List<CartItem>();
                HttpContext.Session.SetString(CartSessionKey, JsonSerializer.Serialize(items));
                return items;
            }

            return new List<CartItem>();
        }

        private void SaveCartItems(List<CartItem> items)
        {
            var json = JsonSerializer.Serialize(items);
            HttpContext.Session.SetString(CartSessionKey, json);
            HttpContext.Session.SetInt32("CartCount", items.Sum(i => i.Soluong));

            var cookieOptions = new CookieOptions
            {
                Expires = DateTime.Now.AddDays(30),
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax
            };
            Response.Cookies.Append(CartCookieKey, json, cookieOptions);
        }

        [Route("gio-hang")]
        public IActionResult Index()
        {
            var cartItems = GetCartItems();
            ViewBag.TotalAmount = cartItems.Sum(i => i.Thanhtien);
            return View(cartItems);
        }

        [HttpPost]
        [Route("gio-hang/them")]
        public IActionResult AddToCart(string productId, int quantity = 1)
        {
            var product = _sanPhamBLL.GetById(productId);
            if (product == null)
            {
                return Json(new { success = false, message = "Sản phẩm không tồn tại" });
            }

            var cartItems = GetCartItems();
            var existingItem = cartItems.FirstOrDefault(i => i.Masp == productId);

            if (existingItem != null)
            {
                existingItem.Soluong += quantity;
            }
            else
            {
                // Lấy hình đầu tiên từ chuỗi hình ảnh (phân cách bằng ;)
                var firstImage = "/images/no-image.jpg";
                if (!string.IsNullOrEmpty(product.Hinhanh))
                {
                    var images = product.Hinhanh.Split(';', StringSplitOptions.RemoveEmptyEntries);
                    foreach (var img in images)
                    {
                        var trimmed = img.Trim();
                        if (!string.IsNullOrEmpty(trimmed) && (trimmed.StartsWith("/") || trimmed.StartsWith("http")))
                        {
                            firstImage = trimmed;
                            break;
                        }
                    }
                }
                
                cartItems.Add(new CartItem
                {
                    Masp = product.Masp!,
                    Tensp = product.Tensp ?? "",
                    Hinhanh = firstImage,
                    Giaban = product.Giaban ?? 0,
                    Soluong = quantity
                });
            }

            SaveCartItems(cartItems);

            return Json(new { 
                success = true, 
                message = "Đã thêm sản phẩm vào giỏ hàng",
                cartCount = cartItems.Sum(i => i.Soluong)
            });
        }

        [HttpPost]
        [Route("gio-hang/cap-nhat")]
        public IActionResult UpdateCart(string productId, int quantity)
        {
            var cartItems = GetCartItems();
            var item = cartItems.FirstOrDefault(i => i.Masp == productId);

            if (item != null)
            {
                if (quantity <= 0)
                    cartItems.Remove(item);
                else
                    item.Soluong = quantity;
                SaveCartItems(cartItems);
            }

            return Json(new { 
                success = true,
                cartCount = cartItems.Sum(i => i.Soluong),
                totalAmount = cartItems.Sum(i => i.Thanhtien)
            });
        }

        [HttpPost]
        [Route("gio-hang/xoa")]
        public IActionResult RemoveFromCart(string productId)
        {
            var cartItems = GetCartItems();
            var item = cartItems.FirstOrDefault(i => i.Masp == productId);

            if (item != null)
            {
                cartItems.Remove(item);
                SaveCartItems(cartItems);
            }

            return Json(new { 
                success = true,
                message = "Đã xóa sản phẩm khỏi giỏ hàng",
                cartCount = cartItems.Sum(i => i.Soluong),
                totalAmount = cartItems.Sum(i => i.Thanhtien)
            });
        }

        [HttpPost]
        [Route("gio-hang/xoa-tat-ca")]
        public IActionResult ClearCart()
        {
            SaveCartItems(new List<CartItem>());
            return Json(new { success = true, message = "Đã xóa toàn bộ giỏ hàng" });
        }

        [Route("thanh-toan")]
        public IActionResult Checkout()
        {
            var customerId = HttpContext.Session.GetString("CustomerId");
            if (string.IsNullOrEmpty(customerId))
            {
                TempData["ReturnUrl"] = "/thanh-toan";
                return RedirectToAction("Login", "Account");
            }

            var cartItems = GetCartItems();
            if (!cartItems.Any())
            {
                TempData["Error"] = "Giỏ hàng trống";
                return RedirectToAction("Index");
            }

            ViewBag.TotalAmount = cartItems.Sum(i => i.Thanhtien);
            ViewBag.Customer = _khachHangBLL.GetById(customerId);
            return View(cartItems);
        }

        [HttpPost]
        [Route("dat-hang")]
        public IActionResult PlaceOrder(string? ghichu)
        {
            try
            {
                var customerId = HttpContext.Session.GetString("CustomerId");
                if (string.IsNullOrEmpty(customerId))
                {
                    return RedirectToAction("Login", "Account");
                }

                var cartItems = GetCartItems();
                if (!cartItems.Any())
                {
                    TempData["Error"] = "Giỏ hàng trống";
                    return RedirectToAction("Index");
                }

                // Tạo mã đơn hàng mới
                var newOrderId = _donHangBLL.GenerateNewId();

                // Tạo đơn hàng từ website
                var order = new DonHang
                {
                    Madonhang = newOrderId,
                    Makh = customerId,
                    Ngaydat = DateTime.Now,
                    Trangthai = "Chờ xác nhận",
                    Ghichu = ghichu
                };

                var (success, message) = _donHangBLL.Insert(order);
                if (!success)
                {
                    TempData["Error"] = message;
                    return RedirectToAction("Checkout");
                }

                // Tạo chi tiết đơn hàng
                foreach (var item in cartItems)
                {
                    var ctDonhang = new CtDonhang
                    {
                        Madonhang = newOrderId,
                        Masp = item.Masp,
                        Soluong = item.Soluong,
                        Dongia = item.Giaban,
                        Thanhtien = item.Thanhtien
                    };
                    _ctDonhangBLL.Insert(ctDonhang);
                }

                // Xóa giỏ hàng
                SaveCartItems(new List<CartItem>());

                TempData["Success"] = $"Đặt hàng thành công! Mã đơn hàng: {newOrderId}";
                return RedirectToAction("OrderSuccess", new { orderId = newOrderId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Có lỗi xảy ra: " + ex.Message;
                return RedirectToAction("Checkout");
            }
        }

        [Route("dat-hang-thanh-cong")]
        public IActionResult OrderSuccess(string orderId)
        {
            var order = _donHangBLL.GetById(orderId);
            if (order == null)
            {
                return NotFound();
            }

            order.CtDonhangs = _ctDonhangBLL.GetByDonHang(orderId);
            return View(order);
        }

        [HttpGet]
        [Route("gio-hang/so-luong")]
        public IActionResult GetCartCount()
        {
            var cartItems = GetCartItems();
            return Json(new { count = cartItems.Sum(i => i.Soluong) });
        }
    }
}
