// FILE: Controllers/AccountController.cs
// TẦNG CONTROLLER - Xử lý đăng nhập, đăng ký, quản lý tài khoản khách hàng
// LUỒNG: User → Controller → BLL → DAL → Database

using Microsoft.AspNetCore.Mvc;
using DOANCHUYENNGANH_WEB_QLNOITHAT.BLL;
using DOANCHUYENNGANH_WEB_QLNOITHAT.Models;
using System.Text.Json;

namespace DOANCHUYENNGANH_WEB_QLNOITHAT.Controllers
{
    public class AccountController : Controller
    {
        private readonly KhachHangBLL _khachHangBLL = new KhachHangBLL();
        private readonly DonHangBLL _donHangBLL = new DonHangBLL();
        private readonly CtDonhangBLL _ctDonhangBLL = new CtDonhangBLL();
        private const string RememberMeCookieKey = "RememberMe";

        // GET: Account/Login Hiển thị trang đăng nhập
        public IActionResult Login(string? message)
        {
             // Nếu đã đăng nhập rồi → chuyển về trang chủ
            if (HttpContext.Session.GetString("CustomerId") != null)
            {
                return RedirectToAction("Index", "Home");
            }

            // Kiểm tra cookie "Ghi nhớ đăng nhập" 7 ngày
            if (Request.Cookies.TryGetValue(RememberMeCookieKey, out var cookieValue))
            {
                try
                {
                     // Đọc thông tin từ cookie
                    var loginData = JsonSerializer.Deserialize<RememberMeData>(cookieValue);
                    // Kiểm tra thông tin còn hợp lệ không
                    if (loginData != null && !string.IsNullOrEmpty(loginData.CustomerId))
                    {
                        var khachHang = _khachHangBLL.GetById(loginData.CustomerId);
                        if (khachHang != null && khachHang.Sdtkh == loginData.Phone)
                        {
                            SetLoginSession(khachHang); // Tự động đăng nhập
                            return RedirectToAction("Index", "Home");
                        }
                    }
                }
                catch
                {
                    Response.Cookies.Delete(RememberMeCookieKey);
                }
            }

            if (!string.IsNullOrEmpty(message))
            {
                ViewBag.Success = message;
            }
            return View();
        }

        // POST: Account/Login Xử lý đăng nhập
        [HttpPost]
        public IActionResult Login(string phone, string password, bool rememberMe = false)
        {
             // Validate đầu vào
            if (string.IsNullOrEmpty(phone))
            {
                TempData["Error"] = "Vui lòng nhập số điện thoại";
                return View();
            }
             
            if (string.IsNullOrEmpty(password))
            {
                 
                TempData["Error"] = "Vui lòng nhập mật khẩu";
                return View();
            }
            // Gọi BLL kiểm tra đăng nhập
            var (success, message, khachHang) = _khachHangBLL.Login(phone, password);
            
            if (!success || khachHang == null)
            {
                // "Sai mật khẩu" hoặc "Tài khoản không tồn tại"
                TempData["Error"] = message;
                return View();
            }
            // Đăng nhập thành công → Lưu Session
            SetLoginSession(khachHang);
            // Nếu chọn "Ghi nhớ đăng nhập" → Lưu Cookie 7 ngày
            if (rememberMe)
            {
                SaveRememberMeCookie(khachHang);
            }

            return RedirectToAction("Index", "Home");
        }

        // Lưu thông tin đăng nhập vào Session
        private void SetLoginSession(KhachHang khachHang)
        {
            HttpContext.Session.SetString("CustomerId", khachHang.Makh);
            HttpContext.Session.SetString("CustomerName", khachHang.Hotenkh ?? "");
            HttpContext.Session.SetString("CustomerPhone", khachHang.Sdtkh ?? "");
        }
        // Lưu cookie "Ghi nhớ đăng nhập" 7 ngày
        private void SaveRememberMeCookie(KhachHang khachHang)
        {
            var loginData = new RememberMeData
            {
                CustomerId = khachHang.Makh,
                Phone = khachHang.Sdtkh ?? "",
                ExpireDate = DateTime.Now.AddDays(7)
            };

            var cookieOptions = new CookieOptions
            {
                Expires = DateTime.Now.AddDays(7),
                HttpOnly = true, // Không cho JavaScript đọc (bảo mật)
                Secure = true,   // Chỉ gửi qua HTTPS
                SameSite = SameSiteMode.Lax,
                IsEssential = true
            };

            Response.Cookies.Append(RememberMeCookieKey, JsonSerializer.Serialize(loginData), cookieOptions);
        }

        private class RememberMeData
        {
            public string CustomerId { get; set; } = string.Empty;
            public string Phone { get; set; } = string.Empty;
            public DateTime ExpireDate { get; set; }
        }

        // GET: Account/Register Hiển thị form đăng ký
        public IActionResult Register()
        {
             // Nếu đã đăng nhập → về trang chủ
            if (HttpContext.Session.GetString("CustomerId") != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // POST: Account/Register Xử lý đăng ký
        [HttpPost]
        public IActionResult Register(string hoTen, string soDienThoai, string diaChi, string matKhau, string xacNhanMatKhau)
        {
            // Validate họ tên
            if (string.IsNullOrWhiteSpace(hoTen))
            {
                TempData["Error"] = "Vui lòng nhập họ tên";
                return View();
            }
             // Validate số điện thoại ( 10 số)
            if (string.IsNullOrWhiteSpace(soDienThoai) || soDienThoai.Length < 10)
            {
                TempData["Error"] = "Số điện thoại không hợp lệ";
                return View();
            }

            if (string.IsNullOrWhiteSpace(diaChi))
            {
                TempData["Error"] = "Vui lòng nhập địa chỉ";
                return View();
            }
            // Validate mật khẩu (ít nhất 6 ký tự)
            if (string.IsNullOrWhiteSpace(matKhau) || matKhau.Length < 6)
            {
                TempData["Error"] = "Mật khẩu phải có ít nhất 6 ký tự";
                return View();
            }
             // Kiểm tra mật khẩu xác nhận
            if (matKhau != xacNhanMatKhau)
            {
                TempData["Error"] = "Mật khẩu xác nhận không khớp";
                return View();
            }
            // Tạo đối tượng khách hàng
            var khachHang = new KhachHang
            {
                Hotenkh = hoTen,
                Sdtkh = soDienThoai,
                Diachikh = diaChi,
                Matkhau = matKhau
            };
            // Gọi BLL đăng ký (BLL sẽ kiểm tra SĐT đã tồn tại chưa)
            var (success, message) = _khachHangBLL.Register(khachHang);
            
            if (success)
            {
                TempData["Success"] = "Đăng ký thành công! Vui lòng đăng nhập.";
                return RedirectToAction("Login");
            }

            TempData["Error"] = message; // "Số điện thoại đã được đăng ký"
            return View();
        }

        // GET: Account/Profile Xem hồ sơ
        public IActionResult Profile()
        {
            // Kiểm tra đăng nhập
            var customerId = HttpContext.Session.GetString("CustomerId");
            if (string.IsNullOrEmpty(customerId))
            {
                return RedirectToAction("Login");
            }
             // Lấy thông tin khách hàng từ DB
            var khachHang = _khachHangBLL.GetById(customerId);
            if (khachHang == null)
            {
                return RedirectToAction("Login");
            }

            return View(khachHang);
        }

        // POST: Account/Profile Cập nhật hồ sơ
        [HttpPost]
        public IActionResult Profile(string makh, string hoTen, string diaChi)
        {
            // Kiểm tra đăng nhập + quyền sở hữu
            var customerId = HttpContext.Session.GetString("CustomerId");
            if (string.IsNullOrEmpty(customerId) || customerId != makh)
            {
                return RedirectToAction("Login");
            }

            if (string.IsNullOrWhiteSpace(hoTen))
            {
                TempData["Error"] = "Vui lòng nhập họ tên";
                return View(_khachHangBLL.GetById(customerId));
            }
             // Lấy thông tin hiện tại
            var existing = _khachHangBLL.GetById(customerId);
            if (existing != null)
            {
                // Cập nhật thông tin mới
                existing.Hotenkh = hoTen;
                existing.Diachikh = diaChi;

                var (success, message) = _khachHangBLL.Update(existing);
                if (success)
                {
                    // Cập nhật Session với tên mới
                    HttpContext.Session.SetString("CustomerName", existing.Hotenkh ?? "");
                    TempData["Success"] = "Cập nhật thông tin thành công";
                }
                else
                {
                    TempData["Error"] = message;
                }
                return View(existing);
            }

            return View(_khachHangBLL.GetById(customerId));
        }

        // GET: Account/Orders Danh sách đơn hàng
        public IActionResult Orders()
        {
            var customerId = HttpContext.Session.GetString("CustomerId");
            if (string.IsNullOrEmpty(customerId))
            {
                return RedirectToAction("Login");
            }

            ViewBag.Customer = _khachHangBLL.GetById(customerId);
            // Lấy tất cả đơn hàng của khách
            var donHangs = _donHangBLL.GetByKhachHang(customerId);

            // Lấy chi tiết cho mỗi đơn hàng (để hiển thị số SP, tổng tiền)
            foreach (var dh in donHangs)
            {
                dh.CtDonhangs = _ctDonhangBLL.GetByDonHang(dh.Madonhang);
            }

            return View(donHangs);
        }

        // GET: Account/OrderDetail/id
        public IActionResult OrderDetail(string id)
        {
            var customerId = HttpContext.Session.GetString("CustomerId");
            if (string.IsNullOrEmpty(customerId))
            {
                return RedirectToAction("Login");
            }

            var donHang = _donHangBLL.GetById(id);
              // Kiểm tra đơn hàng có thuộc về khách này không (bảo mật)
            if (donHang == null || donHang.Makh != customerId)
            {
                return NotFound();
            }
            // Lấy chi tiết sản phẩm trong đơn
            donHang.CtDonhangs = _ctDonhangBLL.GetByDonHang(id);
            return View(donHang);
        }

        // GET: Account/ChangePassword
        public IActionResult ChangePassword()
        {
            var customerId = HttpContext.Session.GetString("CustomerId");
            if (string.IsNullOrEmpty(customerId))
            {
                return RedirectToAction("Login");
            }

            ViewBag.Customer = _khachHangBLL.GetById(customerId);
            return View();
        }

        // POST: Account/ChangePasswordInProfile
        [HttpPost]
        public IActionResult ChangePasswordInProfile(string currentPassword, string newPassword, string confirmPassword)
        {
            var customerId = HttpContext.Session.GetString("CustomerId");
            if (string.IsNullOrEmpty(customerId))
            {
                return RedirectToAction("Login");
            }

            var khachHang = _khachHangBLL.GetById(customerId);
            if (khachHang == null)
            {
                return RedirectToAction("Login");
            }
            // Kiểm tra mật khẩu hiện tại
            if (khachHang.Matkhau != currentPassword)
            {
                TempData["Error"] = "Mật khẩu hiện tại không đúng";
                return RedirectToAction("ChangePassword");
            }
             // Validate mật khẩu mới
            if (string.IsNullOrEmpty(newPassword) || newPassword.Length < 6)
            {
                TempData["Error"] = "Mật khẩu mới phải có ít nhất 6 ký tự";
                return RedirectToAction("ChangePassword");
            }
            // Kiểm tra xác nhận
            if (newPassword != confirmPassword)
            {
                TempData["Error"] = "Mật khẩu xác nhận không khớp";
                return RedirectToAction("ChangePassword");
            }
            // Cập nhật mật khẩu mới
            khachHang.Matkhau = newPassword;
            var (success, message) = _khachHangBLL.Update(khachHang);

            if (success)
                TempData["Success"] = "Đổi mật khẩu thành công!";
            else
                TempData["Error"] = message;

            return RedirectToAction("ChangePassword");
        }

        // GET: Account/ForgotPassword
        public IActionResult ForgotPassword()
        {
            HttpContext.Session.Remove("ResetOTP");
            HttpContext.Session.Remove("ResetPhone");
            HttpContext.Session.Remove("OTPExpiry");
            HttpContext.Session.Remove("OTPVerified");
            ViewBag.Step = 1;
            return View();
        }

        // POST: Account/SendOTP
        [HttpPost]
        public IActionResult SendOTP(string sdtkh)
        {
            try
            {
                if (string.IsNullOrEmpty(sdtkh))
                {
                    ViewBag.Error = "Vui lòng nhập số điện thoại";
                    ViewBag.Step = 1;
                    return View("ForgotPassword");
                }

                // Chuẩn hóa số điện thoại (loại bỏ khoảng trắng, dấu gạch)
                sdtkh = sdtkh.Trim().Replace(" ", "").Replace("-", "").Replace(".", "");
                // Kiểm tra SĐT có trong hệ thống không
                var allKhachHang = _khachHangBLL.GetAll();
                var khachHang = allKhachHang.FirstOrDefault(k => k.Sdtkh == sdtkh);
                
                if (khachHang == null)
                {
                    ViewBag.Error = $"Số điện thoại '{sdtkh}' chưa được đăng ký trong hệ thống. Vui lòng đăng ký tài khoản trước.";
                    ViewBag.Step = 1;
                    return View("ForgotPassword");
                }
                // Tạo mã OTP 6 số ngẫu nhiên
                var otp = new Random().Next(100000, 999999).ToString();
                // Lưu OTP vào Session (hết hạn sau 5 phút)
                HttpContext.Session.SetString("ResetOTP", otp);
                HttpContext.Session.SetString("ResetPhone", sdtkh);
                HttpContext.Session.SetString("OTPExpiry", DateTime.Now.AddMinutes(5).ToString());
                // Hiển thị OTP cho user (demo - thực tế sẽ gửi SMS)
                ViewBag.Step = 2;
                ViewBag.OTPCode = otp;
                ViewBag.Phone = sdtkh;

                return View("ForgotPassword");
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Có lỗi xảy ra: {ex.Message}";
                ViewBag.Step = 1;
                return View("ForgotPassword");
            }
        }

        // POST: Account/VerifyOTPInPage
        [HttpPost]
        public IActionResult VerifyOTPInPage(string otp)
        {
            var savedOTP = HttpContext.Session.GetString("ResetOTP");
            var phone = HttpContext.Session.GetString("ResetPhone");
            var expiryStr = HttpContext.Session.GetString("OTPExpiry");

            if (string.IsNullOrEmpty(savedOTP) || string.IsNullOrEmpty(phone))
            {
                return RedirectToAction("ForgotPassword");
            }
            // Kiểm tra OTP hết hạn chưa
            if (DateTime.TryParse(expiryStr, out DateTime expiry) && DateTime.Now > expiry)
            {
                ViewBag.Error = "Mã OTP đã hết hạn. Vui lòng yêu cầu mã mới.";
                ViewBag.Step = 2;
                ViewBag.Phone = phone;
                ViewBag.OTPCode = savedOTP;
                return View("ForgotPassword");
            }
            // Kiểm tra OTP đúng không
            if (otp != savedOTP)
            {
                ViewBag.Error = "Mã OTP không đúng. Vui lòng kiểm tra lại.";
                ViewBag.Step = 2;
                ViewBag.Phone = phone;
                ViewBag.OTPCode = savedOTP;
                return View("ForgotPassword");
            }
             // OTP đúng → Cho phép đặt mật khẩu mới
            HttpContext.Session.SetString("OTPVerified", "true");
            ViewBag.Step = 3;
            return View("ForgotPassword");
        }

        // POST: Account/ResetPassword
        [HttpPost]
        public IActionResult ResetPassword(string newPassword, string confirmPassword)
        {
            var phone = HttpContext.Session.GetString("ResetPhone");
            var otpVerified = HttpContext.Session.GetString("OTPVerified");

            if (string.IsNullOrEmpty(phone) || otpVerified != "true")
            {
                return RedirectToAction("ForgotPassword");
            }

            if (string.IsNullOrEmpty(newPassword) || newPassword.Length < 6)
            {
                ViewBag.Error = "Mật khẩu phải có ít nhất 6 ký tự";
                ViewBag.Step = 3;
                return View("ForgotPassword");
            }

            if (newPassword != confirmPassword)
            {
                ViewBag.Error = "Mật khẩu xác nhận không khớp";
                ViewBag.Step = 3;
                return View("ForgotPassword");
            }

            var khachHang = _khachHangBLL.GetAll().FirstOrDefault(k => k.Sdtkh == phone);
            if (khachHang != null)
            {
                khachHang.Matkhau = newPassword;
                _khachHangBLL.Update(khachHang);

                HttpContext.Session.Remove("ResetOTP");
                HttpContext.Session.Remove("ResetPhone");
                HttpContext.Session.Remove("OTPExpiry");
                HttpContext.Session.Remove("OTPVerified");

                TempData["Success"] = "Đổi mật khẩu thành công! Vui lòng đăng nhập với mật khẩu mới.";
                return RedirectToAction("Login");
            }

            ViewBag.Error = "Có lỗi xảy ra. Vui lòng thử lại.";
            ViewBag.Step = 3;
            return View("ForgotPassword");
        }

        public IActionResult VerifyOTP() => RedirectToAction("ForgotPassword");

        // POST: Account/CancelOrder
        [HttpPost]
        public IActionResult CancelOrder(string orderId)
        {
            var customerId = HttpContext.Session.GetString("CustomerId");
            if (string.IsNullOrEmpty(customerId))
            {
                return Json(new { success = false, message = "Vui lòng đăng nhập" });
            }

            var donHang = _donHangBLL.GetById(orderId);
            if (donHang == null)
            {
                return Json(new { success = false, message = "Đơn hàng không tồn tại" });
            }

            // Kiểm tra đơn hàng có thuộc về khách hàng này không
            if (donHang.Makh != customerId)
            {
                return Json(new { success = false, message = "Bạn không có quyền hủy đơn hàng này" });
            }

            // Chỉ cho phép hủy đơn hàng ở trạng thái "Chờ xác nhận" hoặc "Chờ xử lý"
            if (donHang.Trangthai != "Chờ xác nhận" && donHang.Trangthai != "Chờ xử lý")
            {
                return Json(new { success = false, message = "Không thể hủy đơn hàng ở trạng thái này" });
            }

            // Kiểm tra thời gian đặt hàng - chỉ cho phép hủy trong vòng 30 phút
            var thoiGianDat = donHang.Ngaydat;
            var thoiGianHienTai = DateTime.Now;
            var khoangThoiGian = thoiGianHienTai - thoiGianDat;
            
            if (khoangThoiGian.TotalMinutes > 30)
            {
                var phutDaQua = (int)khoangThoiGian.TotalMinutes;
                return Json(new { 
                    success = false, 
                    message = $"Đã quá thời gian cho phép hủy đơn hàng! Bạn chỉ có thể hủy đơn hàng trong vòng 30 phút kể từ khi đặt hàng. Đơn hàng này đã được đặt cách đây {phutDaQua} phút. Vui lòng liên hệ hotline để được hỗ trợ." 
                });
            }

            // Hoàn lại số lượng tồn kho trước khi hủy
            var chiTietDonHang = _ctDonhangBLL.GetByDonHang(orderId);
            var sanPhamBLL = new SanPhamBLL();
            foreach (var ct in chiTietDonHang)
            {
                var sanPham = sanPhamBLL.GetById(ct.Masp);
                if (sanPham != null)
                {
                    sanPham.Soluongton = (sanPham.Soluongton ?? 0) + ct.Soluong;
                    sanPhamBLL.Update(sanPham);
                }
            }

            // Cập nhật trạng thái đơn hàng thành "Đã hủy"
            var (success, message) = _donHangBLL.UpdateTrangThai(orderId, "Đã hủy");
            
            if (success)
            {
                return Json(new { success = true, message = "Hủy đơn hàng thành công. Số lượng sản phẩm đã được hoàn lại kho." });
            }

            return Json(new { success = false, message = message });
        }

        // GET: Account/Logout
        public IActionResult Logout()
        {
            // Xóa toàn bộ Session
            HttpContext.Session.Clear();
            // Xóa cookie "Ghi nhớ đăng nhập"
            if (Request.Cookies.ContainsKey(RememberMeCookieKey))
            {
                Response.Cookies.Delete(RememberMeCookieKey);
            }
            return RedirectToAction("Index", "Home");
        }
    }
}
