// FILE: BLL/UserBLL.cs
// TẦNG BLL - Xử lý nghiệp vụ User/Nhân viên (đăng nhập admin, phân quyền)
// LUỒNG: Controller → BLL → DAL → Database

using DOANCHUYENNGANH_WEB_QLNOITHAT.DAL;
using DOANCHUYENNGANH_WEB_QLNOITHAT.Models;

namespace DOANCHUYENNGANH_WEB_QLNOITHAT.BLL
{
    public class UserBLL
    {
        private readonly UserDAL _dal = new UserDAL();

        private static readonly string[] AllowedAdminRoles = { 
            "admin", "quanly", "quản trị hệ thống", "nhân viên", 
            "nhanvien", "quan ly", "quản lý", "manager", "staff",
            "quản trị", "quantri", "administrator"
        };

        public static readonly string[] AdminRoles = { "Admin", "QuanLy", "Quản trị hệ thống", "Quản lý" };

        public List<User> GetAll() => _dal.GetAll();

        public User? GetById(string userId)
        {
            if (string.IsNullOrEmpty(userId)) return null;
            return _dal.GetById(userId);
        }

        public (bool Success, string Message, User? User) Login(string tenUser, string matKhau)
        {
            if (string.IsNullOrEmpty(tenUser) || string.IsNullOrEmpty(matKhau))
                return (false, "Vui lòng nhập đầy đủ thông tin đăng nhập", null);

            var user = _dal.Login(tenUser, matKhau);
            if (user == null)
                return (false, "Tên đăng nhập hoặc mật khẩu không đúng", null);

            var userRole = user.VaiTro?.ToLower().Trim() ?? "";
            if (string.IsNullOrEmpty(userRole) || 
                !AllowedAdminRoles.Any(r => userRole.Contains(r) || r.Contains(userRole)))
                return (false, $"Bạn không có quyền truy cập. Vai trò: {user.VaiTro ?? "Không xác định"}", null);

            return (true, "Đăng nhập thành công", user);
        }

        public static bool IsAdmin(string? vaiTro)
        {
            if (string.IsNullOrEmpty(vaiTro)) return false;
            return AdminRoles.Any(r => r.Equals(vaiTro, StringComparison.OrdinalIgnoreCase));
        }

        public static bool IsStaff(string? vaiTro)
        {
            if (string.IsNullOrEmpty(vaiTro)) return false;
            var staffRoles = new[] { "Nhân viên", "nhanvien", "staff" };
            return staffRoles.Any(r => r.Equals(vaiTro, StringComparison.OrdinalIgnoreCase));
        }

        public (bool Success, string Message) Insert(User obj)
        {
            if (string.IsNullOrEmpty(obj.UserId))
                return (false, "Mã người dùng không được để trống");
            if (_dal.Exists(obj.UserId))
                return (false, "Mã người dùng đã tồn tại");
            if (string.IsNullOrEmpty(obj.TenUser))
                return (false, "Tên đăng nhập không được để trống");
            if (string.IsNullOrEmpty(obj.MatKhau))
                return (false, "Mật khẩu không được để trống");

            var result = _dal.Insert(obj);
            return result > 0 ? (true, "Thêm thành công") : (false, "Có lỗi xảy ra");
        }

        public (bool Success, string Message) Update(User obj)
        {
            if (string.IsNullOrEmpty(obj.UserId))
                return (false, "Mã người dùng không được để trống");
            if (!_dal.Exists(obj.UserId))
                return (false, "Người dùng không tồn tại");

            var result = _dal.Update(obj);
            return result > 0 ? (true, "Cập nhật thành công") : (false, "Có lỗi xảy ra");
        }

        public (bool Success, string Message) UpdateProfile(string userId, string? hoTen, string? tenUser, string? matKhauMoi = null)
        {
            if (string.IsNullOrEmpty(userId))
                return (false, "Mã người dùng không được để trống");

            var result = _dal.UpdateProfile(userId, hoTen, tenUser, matKhauMoi);
            return result > 0 ? (true, "Cập nhật thành công") : (false, "Có lỗi xảy ra");
        }

        public (bool Success, string Message) Delete(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return (false, "Mã không được để trống");
            if (!_dal.Exists(userId))
                return (false, "Người dùng không tồn tại");
            
            // Kiểm tra không cho xóa Admin
            var user = _dal.GetById(userId);
            if (user != null && IsAdmin(user.VaiTro))
                return (false, "Không thể xóa tài khoản Admin/Quản trị hệ thống!");
            
            if (_dal.HasRelatedData(userId))
                return (false, "Không thể xóa vì có dữ liệu liên quan!");

            var result = _dal.Delete(userId);
            return result > 0 ? (true, "Xóa thành công") : (false, "Có lỗi xảy ra");
        }

        public bool Exists(string userId) => _dal.Exists(userId);
        public bool ExistsByTenUser(string tenUser, string? excludeUserId = null) 
            => _dal.ExistsByTenUser(tenUser, excludeUserId);
        public List<User> Search(string? searchName, string? searchUser, string? vaiTro)
            => _dal.Search(searchName, searchUser, vaiTro);
        public List<string> GetDistinctVaiTro() => _dal.GetDistinctVaiTro();
        public bool HasRelatedData(string userId) => _dal.HasRelatedData(userId);

        public (bool Success, string Message, string? HoTen) ResetPassword(string userId)
        {
            var user = _dal.GetById(userId);
            if (user == null) return (false, "Không tìm thấy người dùng", null);

            var result = _dal.ResetPassword(userId);
            return result > 0 
                ? (true, $"Đã reset mật khẩu của {user.HoTen} về '123456'", user.HoTen) 
                : (false, "Có lỗi xảy ra", null);
        }
    }
}
