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

        // GET: Account/Login
        public IActionResult Login(string? message)
        {
            if (HttpContext.Session.GetString("CustomerId") != null)
            {
                return RedirectToAction("Index", "Home");
            }

            // Kiểm tra cookie "Ghi nhớ đăng nhập" 7 ngày
            if (Request.Cookies.TryGetValue(RememberMeCookieKey, out var cookieValue))
            {
                try
                {
                    var loginData = JsonSerializer.Deserialize<RememberMeData>(cookieValue);
                    if (loginData != null && !string.IsNullOrEmpty(loginData.CustomerId))
                    {
                        var khachHang = _khachHangBLL.GetById(loginData.CustomerId);
                        if (khachHang != null && khachHang.Sdtkh == loginData.Phone)
                        {
                            SetLoginSession(khachHang);
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

        // POST: Account/Login
        [HttpPost]
        public IActionResult Login(string phone, string password, bool rememberMe = false)
        {
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

            var (success, message, khachHang) = _khachHangBLL.Login(phone, password);
            
            if (!success || khachHang == null)
            {
                TempData["Error"] = message;
                return View();
            }

            SetLoginSession(khachHang);

            if (rememberMe)
            {
                SaveRememberMeCookie(khachHang);
            }

            return RedirectToAction("Index", "Home");
        }

        private void SetLoginSession(KhachHang khachHang)
        {
            HttpContext.Session.SetString("CustomerId", khachHang.Makh);
            HttpContext.Session.SetString("CustomerName", khachHang.Hotenkh ?? "");
            HttpContext.Session.SetString("CustomerPhone", khachHang.Sdtkh ?? "");
        }

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
                HttpOnly = true,
                Secure = true,
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

        // GET: Account/Register
        public IActionResult Register()
        {
            if (HttpContext.Session.GetString("CustomerId") != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // POST: Account/Register
        [HttpPost]
        public IActionResult Register(string hoTen, string soDienThoai, string diaChi, string matKhau, string xacNhanMatKhau)
        {
            if (string.IsNullOrWhiteSpace(hoTen))
            {
                TempData["Error"] = "Vui lòng nhập họ tên";
                return View();
            }

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

            if (string.IsNullOrWhiteSpace(matKhau) || matKhau.Length < 6)
            {
                TempData["Error"] = "Mật khẩu phải có ít nhất 6 ký tự";
                return View();
            }

            if (matKhau != xacNhanMatKhau)
            {
                TempData["Error"] = "Mật khẩu xác nhận không khớp";
                return View();
            }

            var khachHang = new KhachHang
            {
                Hotenkh = hoTen,
                Sdtkh = soDienThoai,
                Diachikh = diaChi,
                Matkhau = matKhau
            };

            var (success, message) = _khachHangBLL.Register(khachHang);
            
            if (success)
            {
                TempData["Success"] = "Đăng ký thành công! Vui lòng đăng nhập.";
                return RedirectToAction("Login");
            }

            TempData["Error"] = message;
            return View();
        }

        // GET: Account/Profile
        public IActionResult Profile()
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

            return View(khachHang);
        }

        // POST: Account/Profile
        [HttpPost]
        public IActionResult Profile(string makh, string hoTen, string diaChi)
        {
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

            var existing = _khachHangBLL.GetById(customerId);
            if (existing != null)
            {
                existing.Hotenkh = hoTen;
                existing.Diachikh = diaChi;

                var (success, message) = _khachHangBLL.Update(existing);
                if (success)
                {
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

        // GET: Account/Orders
        public IActionResult Orders()
        {
            var customerId = HttpContext.Session.GetString("CustomerId");
            if (string.IsNullOrEmpty(customerId))
            {
                return RedirectToAction("Login");
            }

            ViewBag.Customer = _khachHangBLL.GetById(customerId);
            var donHangs = _donHangBLL.GetByKhachHang(customerId);

            // Lấy chi tiết cho mỗi đơn hàng
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
            if (donHang == null || donHang.Makh != customerId)
            {
                return NotFound();
            }

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

            if (khachHang.Matkhau != currentPassword)
            {
                TempData["Error"] = "Mật khẩu hiện tại không đúng";
                return RedirectToAction("ChangePassword");
            }

            if (string.IsNullOrEmpty(newPassword) || newPassword.Length < 6)
            {
                TempData["Error"] = "Mật khẩu mới phải có ít nhất 6 ký tự";
                return RedirectToAction("ChangePassword");
            }

            if (newPassword != confirmPassword)
            {
                TempData["Error"] = "Mật khẩu xác nhận không khớp";
                return RedirectToAction("ChangePassword");
            }

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

                var allKhachHang = _khachHangBLL.GetAll();
                var khachHang = allKhachHang.FirstOrDefault(k => k.Sdtkh == sdtkh);
                
                if (khachHang == null)
                {
                    ViewBag.Error = $"Số điện thoại '{sdtkh}' chưa được đăng ký trong hệ thống. Vui lòng đăng ký tài khoản trước.";
                    ViewBag.Step = 1;
                    return View("ForgotPassword");
                }

                var otp = new Random().Next(100000, 999999).ToString();
                HttpContext.Session.SetString("ResetOTP", otp);
                HttpContext.Session.SetString("ResetPhone", sdtkh);
                HttpContext.Session.SetString("OTPExpiry", DateTime.Now.AddMinutes(5).ToString());

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

            if (DateTime.TryParse(expiryStr, out DateTime expiry) && DateTime.Now > expiry)
            {
                ViewBag.Error = "Mã OTP đã hết hạn. Vui lòng yêu cầu mã mới.";
                ViewBag.Step = 2;
                ViewBag.Phone = phone;
                ViewBag.OTPCode = savedOTP;
                return View("ForgotPassword");
            }

            if (otp != savedOTP)
            {
                ViewBag.Error = "Mã OTP không đúng. Vui lòng kiểm tra lại.";
                ViewBag.Step = 2;
                ViewBag.Phone = phone;
                ViewBag.OTPCode = savedOTP;
                return View("ForgotPassword");
            }

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
            HttpContext.Session.Clear();
            if (Request.Cookies.ContainsKey(RememberMeCookieKey))
            {
                Response.Cookies.Delete(RememberMeCookieKey);
            }
            return RedirectToAction("Index", "Home");
        }
    }
}
