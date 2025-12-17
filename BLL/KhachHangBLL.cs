using DOANCHUYENNGANH_WEB_QLNOITHAT.DAL;
using DOANCHUYENNGANH_WEB_QLNOITHAT.Models;

namespace DOANCHUYENNGANH_WEB_QLNOITHAT.BLL
{
    public class KhachHangBLL
    {
        private readonly KhachHangDAL _dal = new KhachHangDAL();

        public List<KhachHang> GetAll() => _dal.GetAll();
        public List<KhachHang> Search(string? maKh, string? hoTen, string? sdt) => _dal.Search(maKh, hoTen, sdt);

        public KhachHang? GetById(string maKh)
        {
            if (string.IsNullOrEmpty(maKh)) return null;
            return _dal.GetById(maKh);
        }

        public (bool Success, string Message, KhachHang? KhachHang) Login(string sdt, string matKhau)
        {
            if (string.IsNullOrEmpty(sdt) || string.IsNullOrEmpty(matKhau))
                return (false, "Vui lòng nhập đầy đủ thông tin đăng nhập", null);

            var kh = _dal.Login(sdt, matKhau);
            return kh == null 
                ? (false, "Số điện thoại hoặc mật khẩu không đúng", null)
                : (true, "Đăng nhập thành công", kh);
        }

        public (bool Success, string Message) Register(KhachHang kh)
        {
            if (string.IsNullOrEmpty(kh.Hotenkh))
                return (false, "Họ tên không được để trống");
            if (string.IsNullOrEmpty(kh.Sdtkh))
                return (false, "Số điện thoại không được để trống");
            if (string.IsNullOrEmpty(kh.Matkhau))
                return (false, "Mật khẩu không được để trống");
            if (_dal.ExistsBySdt(kh.Sdtkh))
                return (false, "Số điện thoại đã được đăng ký");

            if (string.IsNullOrEmpty(kh.Makh))
                kh.Makh = GenerateNewId();

            var result = _dal.Insert(kh);
            return result > 0 ? (true, "Đăng ký thành công") : (false, "Có lỗi xảy ra");
        }

        public (bool Success, string Message) Insert(KhachHang obj)
        {
            if (string.IsNullOrEmpty(obj.Makh))
                return (false, "Mã khách hàng không được để trống");
            if (_dal.Exists(obj.Makh))
                return (false, "Mã khách hàng đã tồn tại");

            var result = _dal.Insert(obj);
            return result > 0 ? (true, "Thêm thành công") : (false, "Có lỗi xảy ra");
        }

        public (bool Success, string Message) Update(KhachHang obj)
        {
            if (string.IsNullOrEmpty(obj.Makh))
                return (false, "Mã khách hàng không được để trống");
            if (!_dal.Exists(obj.Makh))
                return (false, "Khách hàng không tồn tại");

            var result = _dal.Update(obj);
            return result > 0 ? (true, "Cập nhật thành công") : (false, "Có lỗi xảy ra");
        }

        public (bool Success, string Message) Delete(string maKh)
        {
            if (string.IsNullOrEmpty(maKh))
                return (false, "Mã không được để trống");
            if (!_dal.Exists(maKh))
                return (false, "Khách hàng không tồn tại");

            try
            {
                var result = _dal.Delete(maKh);
                return result > 0 ? (true, "Xóa khách hàng thành công") : (false, "Có lỗi xảy ra khi xóa");
            }
            catch (Exception)
            {
                return (false, "Không thể xóa khách hàng này vì đang có đơn hàng liên quan");
            }
        }

        public bool Exists(string maKh) => _dal.Exists(maKh);
        public int Count() => _dal.Count();

        private string GenerateNewId()
        {
            var all = _dal.GetAll();
            if (!all.Any()) return "KH001";
            var maxId = all.Where(x => x.Makh.StartsWith("KH"))
                          .Select(x => int.TryParse(x.Makh.Substring(2), out int n) ? n : 0)
                          .DefaultIfEmpty(0).Max();
            return $"KH{(maxId + 1):D3}";
        }
    }
}
