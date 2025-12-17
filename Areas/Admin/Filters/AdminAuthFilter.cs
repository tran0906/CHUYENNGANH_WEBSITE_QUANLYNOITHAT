using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DOANCHUYENNGANH_WEB_QLNOITHAT.Areas.Admin.Filters
{
    /// <summary>
    /// Filter kiểm tra đăng nhập Admin
    /// </summary>
    public class AdminAuthFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var session = context.HttpContext.Session;
            var adminUserId = session.GetString("AdminUserId");

            if (string.IsNullOrEmpty(adminUserId))
            {
                context.Result = new RedirectToActionResult("Login", "Account", new { area = "Admin" });
                return;
            }

            base.OnActionExecuting(context);
        }
    }

    /// <summary>
    /// Filter kiểm tra quyền Admin (chỉ Admin/Quản trị hệ thống mới được truy cập)
    /// </summary>
    public class AdminOnlyFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var session = context.HttpContext.Session;
            var adminUserId = session.GetString("AdminUserId");
            var vaiTro = session.GetString("AdminVaiTro");

            if (string.IsNullOrEmpty(adminUserId))
            {
                context.Result = new RedirectToActionResult("Login", "Account", new { area = "Admin" });
                return;
            }

            // Chỉ Admin hoặc Quản trị hệ thống mới được truy cập
            var adminRoles = new[] { "Admin", "QuanLy", "Quản trị hệ thống" };
            if (!adminRoles.Contains(vaiTro))
            {
                context.Result = new RedirectToActionResult("AccessDenied", "Account", new { area = "Admin" });
                return;
            }

            base.OnActionExecuting(context);
        }
    }

    /// <summary>
    /// Helper class để kiểm tra quyền trong View
    /// </summary>
    public static class RoleHelper
    {
        // Các vai trò Admin (toàn quyền)
        public static readonly string[] AdminRoles = { "Admin", "QuanLy", "Quản trị hệ thống" };
        
        // Các vai trò Nhân viên
        public static readonly string[] StaffRoles = { "Nhân viên" };

        /// <summary>
        /// Kiểm tra có phải Admin không
        /// </summary>
        public static bool IsAdmin(string? vaiTro)
        {
            return !string.IsNullOrEmpty(vaiTro) && AdminRoles.Contains(vaiTro);
        }

        /// <summary>
        /// Kiểm tra có phải Nhân viên không
        /// </summary>
        public static bool IsStaff(string? vaiTro)
        {
            return !string.IsNullOrEmpty(vaiTro) && StaffRoles.Contains(vaiTro);
        }

        /// <summary>
        /// Kiểm tra có quyền truy cập module không
        /// </summary>
        public static bool CanAccess(string? vaiTro, string module)
        {
            if (IsAdmin(vaiTro)) return true; // Admin có toàn quyền

            // Nhân viên chỉ được truy cập các module sau
            var staffModules = new[] { 
                "Dashboard", "DonHang", "CtDonhang", "ThanhToan", 
                "VanChuyen", "PhieuXuatKho", "ThongKe", "Account" 
            };

            return IsStaff(vaiTro) && staffModules.Contains(module);
        }

        /// <summary>
        /// Kiểm tra có quyền thêm/sửa/xóa không
        /// </summary>
        public static bool CanModify(string? vaiTro, string module)
        {
            if (IsAdmin(vaiTro)) return true; // Admin có toàn quyền

            // Nhân viên chỉ được sửa các module sau
            var staffModifyModules = new[] { 
                "DonHang", "ThanhToan", "VanChuyen", "PhieuXuatKho" 
            };

            return IsStaff(vaiTro) && staffModifyModules.Contains(module);
        }
    }
}
