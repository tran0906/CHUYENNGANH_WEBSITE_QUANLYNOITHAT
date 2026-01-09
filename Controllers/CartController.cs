// FILE: CartController.cs - Xử lý giỏ hàng và thanh toán

using Microsoft.AspNetCore.Mvc;
using DOANCHUYENNGANH_WEB_QLNOITHAT.BLL;
using DOANCHUYENNGANH_WEB_QLNOITHAT.Models;
using DOANCHUYENNGANH_WEB_QLNOITHAT.Services;
using System.Text.Json;

namespace DOANCHUYENNGANH_WEB_QLNOITHAT.Controllers
{
    // Class lưu thông tin sản phẩm trong giỏ hàng
    public class CartItem
    {
        public string Masp { get; set; } = string.Empty;
        public string Tensp { get; set; } = string.Empty;
        public string? Hinhanh { get; set; }
        public decimal Giaban { get; set; }
        public int Soluong { get; set; }
        public decimal Thanhtien => Giaban * Soluong; // Tự tính thành tiền
    }

    public class CartController : Controller
    {
        // Khai báo các BLL để gọi tầng nghiệp vụ
        private readonly SanPhamBLL _sanPhamBLL = new SanPhamBLL();
        private readonly KhachHangBLL _khachHangBLL = new KhachHangBLL();
        private readonly DonHangBLL _donHangBLL = new DonHangBLL();
        private readonly CtDonhangBLL _ctDonhangBLL = new CtDonhangBLL();
        private readonly ThanhToanBLL _thanhToanBLL = new ThanhToanBLL();
        private readonly MoMoService _momoService;
        private const string CartCookieKey = "ShoppingCart";
        private const string CartSessionKey = "CartItems";

        public CartController(IConfiguration configuration)
        {
            _momoService = new MoMoService(configuration);
        }

        // Lấy giỏ hàng từ Session hoặc Cookie
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

        // Lưu giỏ hàng vào Session và Cookie
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

        // GET /gio-hang - Hiển thị trang giỏ hàng
        [Route("gio-hang")]
        public IActionResult Index()
        {
            var cartItems = GetCartItems();
            
            // Cập nhật giá mới nhất (bao gồm giá khuyến mãi) cho từng sản phẩm
            bool hasChanges = false;
            foreach (var item in cartItems)
            {
                var product = _sanPhamBLL.GetByIdWithPromotion(item.Masp);
                if (product != null && product.Giaban.HasValue && item.Giaban != product.Giaban.Value)
                {
                    item.Giaban = product.Giaban.Value;
                    hasChanges = true;
                }
            }
            if (hasChanges)
            {
                SaveCartItems(cartItems);
            }
            
            ViewBag.TotalAmount = cartItems.Sum(i => i.Thanhtien);
            return View(cartItems);
        }

        // POST /gio-hang/them - Thêm sản phẩm vào giỏ
        [HttpPost]
        [Route("gio-hang/them")]
        public IActionResult AddToCart(string productId, int quantity = 1)
        {
            // Sử dụng GetByIdWithPromotion để lấy giá đã giảm (nếu có)
            var product = _sanPhamBLL.GetByIdWithPromotion(productId);
            if (product == null)
            {
                return Json(new { success = false, message = "Sản phẩm không tồn tại" });
            }

            // Kiểm tra sản phẩm còn hàng không
            if (product.Soluongton <= 0)
            {
                return Json(new { success = false, message = "Sản phẩm đã hết hàng" });
            }

            var cartItems = GetCartItems();
            var existingItem = cartItems.FirstOrDefault(i => i.Masp == productId);

            // Kiểm tra tổng số lượng trong giỏ + số lượng thêm có vượt quá tồn kho không
            var currentQtyInCart = existingItem?.Soluong ?? 0;
            if (currentQtyInCart + quantity > product.Soluongton)
            {
                return Json(new { 
                    success = false, 
                    message = $"Không đủ hàng! Còn lại {product.Soluongton} sản phẩm, trong giỏ đã có {currentQtyInCart}" 
                });
            }

            if (existingItem != null)
            {
                existingItem.Soluong += quantity;
                // Cập nhật giá mới nhất khi thêm số lượng
                existingItem.Giaban = product.Giaban ?? 0;
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
                        if (string.IsNullOrEmpty(trimmed)) continue;
                        
                        // Nếu đã có đường dẫn đầy đủ
                        if (trimmed.StartsWith("/") || trimmed.StartsWith("http"))
                        {
                            firstImage = trimmed;
                            break;
                        }
                        
                        // Nếu chỉ có tên file, thêm đường dẫn thư mục products
                        if (!trimmed.Contains("/"))
                        {
                            firstImage = "/images/products/" + trimmed;
                            break;
                        }
                        
                        // Trường hợp khác, thêm / ở đầu
                        firstImage = "/" + trimmed;
                        break;
                    }
                }
                
                // Giaban đã được tính giảm giá trong GetByIdWithPromotion
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
                cartCount = cartItems.Sum(i => i.Soluong),
                price = product.Giaban,
                originalPrice = product.GiaGoc,
                discount = product.PhanTramGiam
            });
        }

        // POST /gio-hang/cap-nhat - Cập nhật số lượng sản phẩm
        [HttpPost]
        [Route("gio-hang/cap-nhat")]
        public IActionResult UpdateCart(string productId, int quantity)
        {
            var cartItems = GetCartItems();
            var item = cartItems.FirstOrDefault(i => i.Masp == productId);

            if (item != null)
            {
                if (quantity <= 0)
                {
                    cartItems.Remove(item);
                }
                else
                {
                    // Kiểm tra tồn kho trước khi cập nhật
                    var product = _sanPhamBLL.GetById(productId);
                    if (product == null)
                    {
                        return Json(new { success = false, message = "Sản phẩm không tồn tại" });
                    }
                    
                    if (quantity > product.Soluongton)
                    {
                        return Json(new { 
                            success = false, 
                            message = $"Không đủ hàng! Chỉ còn {product.Soluongton} sản phẩm",
                            maxQuantity = product.Soluongton
                        });
                    }
                    
                    item.Soluong = quantity;
                }
                SaveCartItems(cartItems);
            }

            return Json(new { 
                success = true,
                cartCount = cartItems.Sum(i => i.Soluong),
                totalAmount = cartItems.Sum(i => i.Thanhtien)
            });
        }

        // POST /gio-hang/xoa - Xóa sản phẩm khỏi giỏ
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

        // POST /gio-hang/xoa-tat-ca - Xóa toàn bộ giỏ hàng
        [HttpPost]
        [Route("gio-hang/xoa-tat-ca")]
        public IActionResult ClearCart()
        {
            SaveCartItems(new List<CartItem>());
            return Json(new { success = true, message = "Đã xóa toàn bộ giỏ hàng" });
        }

        // GET /thanh-toan - Hiển thị trang thanh toán
        [Route("thanh-toan")]
        public IActionResult Checkout()

        {
            //kiểm tra đăng nhập 
            var customerId = HttpContext.Session.GetString("CustomerId");
            if (string.IsNullOrEmpty(customerId))
            {
                TempData["ReturnUrl"] = "/thanh-toan";
                return RedirectToAction("Login", "Account");
            }
            //kiểm tra giỏ hàng
            var cartItems = GetCartItems();
            if (!cartItems.Any())
            {
                TempData["Error"] = "Giỏ hàng trống";
                return RedirectToAction("Index");
            }

            // Cập nhật giá mới nhất (bao gồm giá khuyến mãi) cho từng sản phẩm
            bool hasChanges = false;
            foreach (var item in cartItems)
            {
                var product = _sanPhamBLL.GetByIdWithPromotion(item.Masp);
                if (product != null && product.Giaban.HasValue && item.Giaban != product.Giaban.Value)
                {
                    item.Giaban = product.Giaban.Value;
                    hasChanges = true;
                }
            }
            if (hasChanges)
            {
                SaveCartItems(cartItems);
            }
            //lấy thông tin từ database 
            ViewBag.TotalAmount = cartItems.Sum(i => i.Thanhtien);
            ViewBag.Customer = _khachHangBLL.GetById(customerId);
            //trả về view
            return View(cartItems);
        }

        // POST /dat-hang - Xử lý đặt hàng (COD hoặc MoMo)
        [HttpPost]
        [Route("dat-hang")]
        public async Task<IActionResult> PlaceOrder(string? ghichu, string? paymentMethod, string? diachigiaohang)
        {
            try
            {
                //validate kiểm tra đăng nhập giỏ hang đại chỉ 
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

                // Kiểm tra địa chỉ giao hàng
                if (string.IsNullOrWhiteSpace(diachigiaohang))
                {
                    TempData["Error"] = "Vui lòng nhập đầy đủ địa chỉ giao hàng";
                    return RedirectToAction("Checkout");
                }

                // Tính tổng tiền
                var totalAmount = cartItems.Sum(i => i.Thanhtien);
                var requireDeposit = totalAmount >= 2000000; 
                // Đơn >= 2 triệu yêu cầu đặt cọc qua MoMo
                var isMoMo = paymentMethod == "momo";
                
                // Xác định trạng thái và ghi chú thanh toán
                string trangThai;
                string paymentNote = "";
                bool shouldDeductStock = true;
                decimal payAmount = totalAmount; // Số tiền cần thanh toán
                
                // Đơn >= 2 triệu bắt buộc thanh toán MoMo
                if (requireDeposit && !isMoMo)
                {
                    TempData["Error"] = "Đơn hàng từ 2.000.000₫ trở lên yêu cầu đặt cọc 20% qua MoMo";
                    return RedirectToAction("Checkout");
                }
                
                if (isMoMo)
                {
                    // Thanh toán MoMo
                    trangThai = "Chờ thanh toán";
                    shouldDeductStock = false;
                    
                    if (requireDeposit)
                    {
                        payAmount = Math.Round(totalAmount * 0.2m, 0);
                        var remainingAmount = totalAmount - payAmount;
                        paymentNote = $"[MOMO - ĐẶT CỌC 20%: {payAmount:N0}₫ | CÒN LẠI: {remainingAmount:N0}₫]";
                    }
                    else
                    {
                        paymentNote = $"[MOMO: {totalAmount:N0}₫]";
                    }
                }
                else
                {
                    // COD - Thanh toán khi nhận hàng
                    trangThai = "Chờ xác nhận";
                    paymentNote = "[COD]";
                    shouldDeductStock = true;
                }
                
                var fullNote = $"[ĐỊA CHỈ GIAO HÀNG: {diachigiaohang}] {paymentNote}";
                if (!string.IsNullOrEmpty(ghichu))
                {
                    fullNote += $" [GHI CHÚ: {ghichu}]";
                }
                //tạo mã đơn hàng mới
                var newOrderId = _donHangBLL.GenerateNewId();

                var order = new DonHang
                {
                    Madonhang = newOrderId,
                    Makh = customerId,
                    Ngaydat = DateTime.Now,
                    Trangthai = trangThai,
                    Ghichu = fullNote
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
                    var product = _sanPhamBLL.GetById(item.Masp);
                    if (product == null || product.Soluongton < item.Soluong)
                    {
                        _donHangBLL.Delete(newOrderId);
                        TempData["Error"] = $"Sản phẩm '{item.Tensp}' không đủ số lượng tồn kho (còn {product?.Soluongton ?? 0})";
                        return RedirectToAction("Checkout");
                    }

                    var ctDonhang = new CtDonhang
                    {
                        Madonhang = newOrderId,
                        Masp = item.Masp,
                        Soluong = item.Soluong,
                        Dongia = item.Giaban,
                        Thanhtien = item.Thanhtien
                    };
                    _ctDonhangBLL.Insert(ctDonhang);

                    if (shouldDeductStock)
                    {
                        product.Soluongton = (product.Soluongton ?? 0) - item.Soluong;
                        _sanPhamBLL.Update(product);
                    }
                }

                // Nếu thanh toán MoMo, redirect sang MoMo TRƯỚC khi xóa giỏ hàng
                if (isMoMo)
                {
                    // Tạo URL động dựa trên request hiện tại
                    var baseUrl = $"{Request.Scheme}://{Request.Host}";
                    var redirectUrl = $"{baseUrl}/thanh-toan/momo-return";
                    var ipnUrl = $"{baseUrl}/thanh-toan/momo-ipn";
                    
                    var orderInfo = $"Thanh toan don hang {newOrderId}";
                    var (momoSuccess, payUrl, momoMessage) = await _momoService.CreatePaymentAsync(
                        newOrderId, 
                        payAmount, 
                        orderInfo,
                        "",
                        redirectUrl,
                        ipnUrl
                    );

                    if (momoSuccess && !string.IsNullOrEmpty(payUrl))
                    {
                        // MoMo thành công - xóa giỏ hàng và redirect
                        SaveCartItems(new List<CartItem>());
                        return Redirect(payUrl);
                    }
                    else
                    {
                        // MoMo lỗi - xóa đơn hàng, GIỮ giỏ hàng và báo lỗi
                        _donHangBLL.Delete(newOrderId);
                        TempData["Error"] = $"Không thể kết nối MoMo: {momoMessage}. Vui lòng thử lại hoặc chọn phương thức thanh toán khác.";
                        return RedirectToAction("Checkout");
                    }
                }

                // Xóa giỏ hàng (cho COD và chuyển khoản thường)
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

        // GET /dat-hang-thanh-cong - Hiển thị trang đặt hàng thành công
        [Route("dat-hang-thanh-cong")]
        public IActionResult OrderSuccess(string? orderId)
        {
            if (string.IsNullOrEmpty(orderId))
            {
                TempData["Error"] = "Không tìm thấy mã đơn hàng";
                return RedirectToAction("Index", "Home");
            }
            
            var order = _donHangBLL.GetById(orderId);
            if (order == null)
            {
                TempData["Error"] = "Đơn hàng không tồn tại";
                return RedirectToAction("Index", "Home");
            }

            order.CtDonhangs = _ctDonhangBLL.GetByDonHang(orderId);
            return View(order);
        }

        // GET /thanh-toan/momo-return - MoMo redirect về sau khi thanh toán
        [Route("thanh-toan/momo-return")]
        public IActionResult MoMoReturn(
            string? partnerCode, string? orderId, string? requestId,
            long amount, string? orderInfo, string? orderType,
            long transId, int resultCode, string? message,
            string? payType, long responseTime, string? extraData, string? signature)
        {
            // Log để debug
            Console.WriteLine($"=== MoMo Return ===");
            Console.WriteLine($"orderId: {orderId}");
            Console.WriteLine($"resultCode: {resultCode}");
            Console.WriteLine($"transId: {transId}");
            Console.WriteLine($"amount: {amount}");
            Console.WriteLine($"message: {message}");
            
            // Kiểm tra orderId có tồn tại không
            if (string.IsNullOrEmpty(orderId))
            {
                TempData["Error"] = "Không tìm thấy mã đơn hàng từ MoMo";
                return RedirectToAction("Index", "Home");
            }
            
            var order = _donHangBLL.GetById(orderId);
            if (order == null)
            {
                TempData["Error"] = "Đơn hàng không tồn tại";
                return RedirectToAction("Index", "Home");
            }
            
            // Kiểm tra kết quả thanh toán
            if (resultCode == 0)
            {
                // Thanh toán thành công - cập nhật đơn hàng nếu chưa được xử lý
                if (order.Trangthai == "Chờ thanh toán")
                {
                    // Cập nhật trạng thái đơn hàng
                    order.Trangthai = "Đã xác nhận";
                    order.Ghichu += $" [MOMO ĐÃ THANH TOÁN - MÃ GD: {transId}]";
                    _donHangBLL.Update(order);

                    // Tạo bản ghi thanh toán với phương thức MoMo
                    var thanhToan = new ThanhToan
                    {
                        Mathanhtoan = _thanhToanBLL.GenerateNewId(),
                        Madonhang = orderId,
                        Userid = order.Makh,
                        Sotien = amount,
                        Phuongthuc = "MoMo",
                        Ngaythanhtoan = DateTime.Now
                    };
                    _thanhToanBLL.Insert(thanhToan);

                    // Trừ tồn kho
                    var chiTiet = _ctDonhangBLL.GetByDonHang(orderId);
                    foreach (var ct in chiTiet)
                    {
                        var product = _sanPhamBLL.GetById(ct.Masp!);
                        if (product != null)
                        {
                            product.Soluongton = (product.Soluongton ?? 0) - (ct.Soluong ?? 0);
                            _sanPhamBLL.Update(product);
                        }
                    }
                    
                    Console.WriteLine($"Đã cập nhật đơn hàng {orderId} thành công");
                }

                TempData["Success"] = $"Thanh toán MoMo thành công! Mã giao dịch: {transId}";
            }
            else
            {
                // Thanh toán thất bại hoặc bị hủy
                string errorMsg = resultCode switch
                {
                    1001 => "Giao dịch đã bị hủy",
                    1002 => "Giao dịch thất bại do lỗi từ nhà phát hành thẻ",
                    1003 => "Giao dịch bị từ chối bởi MoMo",
                    1004 => "Giao dịch thất bại do số tiền vượt quá hạn mức",
                    1005 => "Giao dịch thất bại do URL hoặc QR code đã hết hạn",
                    1006 => "Giao dịch thất bại do người dùng từ chối xác nhận",
                    1007 => "Giao dịch bị từ chối do tài khoản không đủ số dư",
                    _ => message ?? "Giao dịch không thành công"
                };
                
                TempData["Error"] = $"Thanh toán MoMo thất bại: {errorMsg}";
                Console.WriteLine($"MoMo thất bại - resultCode: {resultCode}, message: {errorMsg}");
            }

            // Redirect đến trang OrderSuccess với orderId
            return RedirectToAction("OrderSuccess", new { orderId = orderId });
        }

        // POST /thanh-toan/momo-ipn - MoMo gọi callback khi thanh toán xong
        [HttpPost]
        [Route("thanh-toan/momo-ipn")]
        public IActionResult MoMoIpn([FromBody] MoMoIpnRequest request)
        {
            try
            {
                // Xác thực chữ ký
                if (!_momoService.VerifySignature(request))
                {
                    return BadRequest(new { resultCode = 1, message = "Invalid signature" });
                }

                if (request.resultCode == 0)
                {
                    // Thanh toán thành công
                    var order = _donHangBLL.GetById(request.orderId);
                    if (order != null && order.Trangthai == "Chờ thanh toán")
                    {
                        order.Trangthai = "Đã xác nhận";
                        order.Ghichu += $" [MOMO IPN - MÃ GD: {request.transId}]";
                        _donHangBLL.Update(order);

                        // Tạo bản ghi thanh toán với phương thức MoMo
                        var thanhToan = new ThanhToan
                        {
                            Mathanhtoan = _thanhToanBLL.GenerateNewId(),
                            Madonhang = request.orderId,
                            Userid = order.Makh,
                            Sotien = request.amount,
                            Phuongthuc = "MoMo",
                            Ngaythanhtoan = DateTime.Now
                        };
                        _thanhToanBLL.Insert(thanhToan);

                        // Trừ tồn kho
                        var chiTiet = _ctDonhangBLL.GetByDonHang(request.orderId);
                        foreach (var ct in chiTiet)
                        {
                            var product = _sanPhamBLL.GetById(ct.Masp!);
                            if (product != null)
                            {
                                product.Soluongton = (product.Soluongton ?? 0) - (ct.Soluong ?? 0);
                                _sanPhamBLL.Update(product);
                            }
                        }
                    }
                }

                return Ok(new { resultCode = 0, message = "OK" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { resultCode = 1, message = ex.Message });
            }
        }

        // GET /gio-hang/so-luong - Lấy số lượng sản phẩm trong giỏ
        [HttpGet]
        [Route("gio-hang/so-luong")]
        public IActionResult GetCartCount()
        {
            var cartItems = GetCartItems();
            return Json(new { count = cartItems.Sum(i => i.Soluong) });
        }

        // GET /gio-hang/ton-kho/{id} - Lấy số lượng tồn kho của sản phẩm
        [HttpGet]
        [Route("gio-hang/ton-kho/{productId}")]
        public IActionResult GetStock(string productId)
        {
            var product = _sanPhamBLL.GetById(productId);
            if (product == null)
            {
                return Json(new { success = false, stock = 0 });
            }
            return Json(new { success = true, stock = product.Soluongton ?? 0 });
        }
    }
}
