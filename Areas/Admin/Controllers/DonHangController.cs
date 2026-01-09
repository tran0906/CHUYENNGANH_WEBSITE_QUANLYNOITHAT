using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using DOANCHUYENNGANH_WEB_QLNOITHAT.BLL;
using DOANCHUYENNGANH_WEB_QLNOITHAT.Models;
using DOANCHUYENNGANH_WEB_QLNOITHAT.Areas.Admin.Filters;

namespace DOANCHUYENNGANH_WEB_QLNOITHAT.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AdminAuthFilter]
    public class DonHangController : Controller
    {
        private readonly DonHangBLL _donHangBLL = new DonHangBLL();
        private readonly KhachHangBLL _khachHangBLL = new KhachHangBLL();
        private readonly UserBLL _userBLL = new UserBLL();
        private readonly CtDonhangBLL _ctDonhangBLL = new CtDonhangBLL();
        private readonly StoredProcedureBLL _spBLL = new StoredProcedureBLL();
        private readonly ThanhToanBLL _thanhToanBLL = new ThanhToanBLL();
        private readonly PhieuXuatKhoBLL _phieuXuatKhoBLL = new PhieuXuatKhoBLL();
        private readonly VanChuyenBLL _vanChuyenBLL = new VanChuyenBLL();
        private readonly SanPhamBLL _sanPhamBLL = new SanPhamBLL();

        public IActionResult Index(string? searchMa, string? trangThai)
        {
            List<DonHang> donHangs;
            
            // Nếu lọc "Hoàn thành" thì lấy cả "Đã giao" và "Hoàn thành"
            if (trangThai == "Hoàn thành")
            {
                var daGiao = _donHangBLL.Search(searchMa, null, "Đã giao", null, null);
                var hoanThanh = _donHangBLL.Search(searchMa, null, "Hoàn thành", null, null);
                donHangs = daGiao.Concat(hoanThanh).OrderByDescending(x => x.Ngaydat).ToList();
            }
            else
            {
                donHangs = _donHangBLL.Search(searchMa, null, trangThai, null, null);
            }
            
            ViewBag.SearchMa = searchMa;
            ViewBag.TrangThai = trangThai;
            return View(donHangs);
        }

        public IActionResult Details(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();
            var donHang = _donHangBLL.GetById(id);
            if (donHang == null) return NotFound();
            
            // Lấy chi tiết đơn hàng và gán vào model
            donHang.CtDonhangs = _ctDonhangBLL.GetByDonHang(id);
            
            // Lấy thông tin người duyệt đơn hàng
            if (!string.IsNullOrEmpty(donHang.Nguoiduyetid))
            {
                ViewBag.NguoiDuyet = _userBLL.GetById(donHang.Nguoiduyetid);
            }
            
            return View(donHang);
        }

        public IActionResult Create()
        {
            ViewData["Makh"] = new SelectList(_khachHangBLL.GetAll(), "Makh", "Hotenkh");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(DonHang donHang)
        {
            if (ModelState.IsValid)
            {
                donHang.Ngaydat = DateTime.Now;
                donHang.Trangthai = "Chờ xử lý";
                
                var (success, message) = _donHangBLL.Insert(donHang);
                if (success)
                {
                    TempData["Success"] = message;
                    return RedirectToAction(nameof(Index));
                }
                ViewBag.Error = message;
            }
            ViewData["Makh"] = new SelectList(_khachHangBLL.GetAll(), "Makh", "Hotenkh", donHang.Makh);
            return View(donHang);
        }

        public IActionResult Edit(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();
            var donHang = _donHangBLL.GetById(id);
            if (donHang == null) return NotFound();
            
            ViewData["Makh"] = new SelectList(_khachHangBLL.GetAll(), "Makh", "Hotenkh", donHang.Makh);
            return View(donHang);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(string id, DonHang donHang)
        {
            if (id != donHang.Madonhang) return NotFound();
            
            if (ModelState.IsValid)
            {
                var (success, message) = _donHangBLL.Update(donHang);
                if (success)
                {
                    TempData["Success"] = message;
                    return RedirectToAction(nameof(Index));
                }
                ViewBag.Error = message;
            }
            ViewData["Makh"] = new SelectList(_khachHangBLL.GetAll(), "Makh", "Hotenkh", donHang.Makh);
            return View(donHang);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateTrangThai(string id, string trangThai)
        {
            var userId = HttpContext.Session.GetString("AdminUserId");
            var (success, message) = _donHangBLL.UpdateTrangThai(id, trangThai, userId);
            
            if (success)
                TempData["Success"] = message;
            else
                TempData["Error"] = message;
                
            return RedirectToAction(nameof(Details), new { id });
        }

        // Chỉ Admin mới có quyền xóa đơn hàng
        public IActionResult Delete(string id)
        {
            // Kiểm tra quyền Admin
            var vaiTro = HttpContext.Session.GetString("AdminVaiTro");
            var adminRoles = new[] { "Admin", "QuanLy", "Quản trị hệ thống" };
            
            if (!adminRoles.Contains(vaiTro))
            {
                TempData["Error"] = "Bạn không có quyền xóa đơn hàng. Chỉ Admin mới có quyền này!";
                return RedirectToAction(nameof(Index));
            }
            
            if (string.IsNullOrEmpty(id)) return NotFound();
            var donHang = _donHangBLL.GetById(id);
            if (donHang == null) return NotFound();
            return View(donHang);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(string id)
        {
            // Kiểm tra quyền Admin
            var vaiTro = HttpContext.Session.GetString("AdminVaiTro");
            var adminRoles = new[] { "Admin", "QuanLy", "Quản trị hệ thống" };
            
            if (!adminRoles.Contains(vaiTro))
            {
                TempData["Error"] = "Bạn không có quyền xóa đơn hàng. Chỉ Admin mới có quyền này!";
                return RedirectToAction(nameof(Index));
            }
            
            var (success, message) = _donHangBLL.Delete(id);
            if (success)
                TempData["Success"] = message;
            else
                TempData["Error"] = message;
            return RedirectToAction(nameof(Index));
        }

        #region Sử dụng Stored Procedures

        // POST: Xác nhận thanh toán (dùng sp_NV_XacNhanThanhToan) + Trừ tồn kho cho đơn chuyển khoản
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult XacNhanThanhToan(string id)
        {
            var userId = HttpContext.Session.GetString("AdminUserId") ?? "";
            
            // Lấy thông tin đơn hàng
            var donHang = _donHangBLL.GetById(id);
            if (donHang == null)
            {
                TempData["Error"] = "Đơn hàng không tồn tại";
                return RedirectToAction(nameof(Index));
            }
            
            // Kiểm tra nếu đơn hàng là chuyển khoản (chưa trừ tồn kho)
            bool isChuyenKhoan = donHang.Ghichu?.Contains("[CHUYỂN KHOẢN") == true;
            bool isChoThanhToan = donHang.Trangthai == "Chờ thanh toán";
            bool isDaGiao = donHang.Trangthai == "Đã giao";
            
            if (isChoThanhToan)
            {
                // Đơn hàng chuyển khoản đang chờ thanh toán → Xác nhận đã nhận tiền → chuyển sang "Đã xác nhận"
                // Trừ tồn kho khi xác nhận thanh toán cho đơn chuyển khoản
                var chiTietDonHang = _ctDonhangBLL.GetByDonHang(id);
                foreach (var ct in chiTietDonHang)
                {
                    var product = _sanPhamBLL.GetById(ct.Masp!);
                    if (product != null)
                    {
                        // Kiểm tra tồn kho còn đủ không
                        if (product.Soluongton < ct.Soluong)
                        {
                            TempData["Error"] = $"Sản phẩm '{product.Tensp}' không đủ tồn kho (còn {product.Soluongton}, cần {ct.Soluong})";
                            return RedirectToAction(nameof(Details), new { id });
                        }
                        
                        product.Soluongton = (product.Soluongton ?? 0) - (ct.Soluong ?? 0);
                        _sanPhamBLL.Update(product);
                    }
                }
                
                // Cập nhật trạng thái sang "Đã xác nhận" và gán người duyệt
                donHang.Trangthai = "Đã xác nhận";
                donHang.Nguoiduyetid = userId;
                var (updateSuccess, updateMessage) = _donHangBLL.Update(donHang);
                
                if (updateSuccess)
                    TempData["Success"] = "Xác nhận đã nhận tiền chuyển khoản thành công! Đã trừ tồn kho. Tiếp tục xử lý đơn hàng.";
                else
                    TempData["Error"] = updateMessage;
                    
                return RedirectToAction(nameof(Details), new { id });
            }
            else if (isDaGiao)
            {
                // Đơn hàng đã giao → Xác nhận đã thu tiền COD → chuyển sang "Hoàn thành"
                var (success, message) = _spBLL.XacNhanThanhToan(id, userId);
                
                if (success)
                {
                    // Cập nhật trạng thái vận chuyển thành "Hoàn thành"
                    var vanChuyen = _vanChuyenBLL.GetByDonHang(id);
                    if (vanChuyen != null && vanChuyen.Trangthaigiao != "Hoàn thành")
                    {
                        vanChuyen.Trangthaigiao = "Hoàn thành";
                        _vanChuyenBLL.Update(vanChuyen);
                    }
                    TempData["Success"] = "Xác nhận thanh toán COD thành công! Đơn hàng đã hoàn thành.";
                }
                else
                    TempData["Error"] = message;
                    
                return RedirectToAction(nameof(Details), new { id });
            }
            else
            {
                TempData["Error"] = "Không thể xác nhận thanh toán cho đơn hàng ở trạng thái này.";
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        // POST: Xuất kho (dùng sp_NV_XuatKho) + Tự động tạo phiếu xuất kho
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult XuatKho(string id)
        {
            var userId = HttpContext.Session.GetString("AdminUserId") ?? "";
            
            var (success, message) = _spBLL.XuatKho(id, userId);
            
            if (success)
            {
                // Tự động tạo phiếu xuất kho
                var phieuXuat = new PhieuXuatKho
                {
                    Maphieuxuat = _phieuXuatKhoBLL.GenerateNewId(),
                    Userid = userId,
                    Madonhang = id,
                    Ngayxuat = DateTime.Now,
                    Manvduyet = userId
                };
                
                var (pxSuccess, pxMessage) = _phieuXuatKhoBLL.Insert(phieuXuat);
                if (pxSuccess)
                    TempData["Success"] = $"Xuất kho thành công! Đã tạo phiếu xuất kho: {phieuXuat.Maphieuxuat}";
                else
                    TempData["Success"] = $"Xuất kho thành công! (Lưu ý: Không tạo được phiếu xuất kho - {pxMessage})";
            }
            else
                TempData["Error"] = message;
                
            return RedirectToAction(nameof(Details), new { id });
        }

        // POST: Điều phối giao hàng (dùng sp_NV_DieuPhoiGiaoHang)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DieuPhoiGiaoHang(string id, string donViVanChuyen)
        {
            var userId = HttpContext.Session.GetString("AdminUserId") ?? "";
            
            var (success, message) = _spBLL.DieuPhoiGiaoHang(id, donViVanChuyen, userId);
            
            if (success)
            {
                // Cập nhật trạng thái vận chuyển thành "Đang giao"
                var vanChuyen = _vanChuyenBLL.GetByDonHang(id);
                if (vanChuyen != null)
                {
                    vanChuyen.Trangthaigiao = "Đang giao";
                    _vanChuyenBLL.Update(vanChuyen);
                }
                TempData["Success"] = "Điều phối giao hàng thành công!";
            }
            else
                TempData["Error"] = message;
                
            return RedirectToAction(nameof(Details), new { id });
        }

        // POST: Xác nhận đã giao hàng
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult XacNhanDaGiao(string id)
        {
            var userId = HttpContext.Session.GetString("AdminUserId") ?? "";
            
            // Cập nhật trạng thái đơn hàng sang "Đã giao"
            var donHang = _donHangBLL.GetById(id);
            if (donHang == null)
            {
                TempData["Error"] = "Đơn hàng không tồn tại";
                return RedirectToAction(nameof(Index));
            }
            
            donHang.Trangthai = "Đã giao";
            var (success, message) = _donHangBLL.Update(donHang);
            
            if (success)
            {
                // Cập nhật trạng thái vận chuyển thành "Hoàn thành"
                var vanChuyen = _vanChuyenBLL.GetByDonHang(id);
                if (vanChuyen != null)
                {
                    vanChuyen.Trangthaigiao = "Hoàn thành";
                    _vanChuyenBLL.Update(vanChuyen);
                }
                TempData["Success"] = "Xác nhận đã giao hàng thành công!";
            }
            else
                TempData["Error"] = message;
                
            return RedirectToAction(nameof(Details), new { id });
        }

        #endregion
    }
}
