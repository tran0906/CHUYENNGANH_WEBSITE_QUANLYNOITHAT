using DOANCHUYENNGANH_WEB_QLNOITHAT.DAL;
using DOANCHUYENNGANH_WEB_QLNOITHAT.Models;

namespace DOANCHUYENNGANH_WEB_QLNOITHAT.BLL
{
    public class DonHangBLL
    {
        private readonly DonHangDAL _dal = new DonHangDAL();

        public List<DonHang> GetAll() => _dal.GetAll();
        public List<DonHang> Search(string? maDh, string? maKh, string? trangThai, DateTime? tuNgay, DateTime? denNgay)
            => _dal.Search(maDh, maKh, trangThai, tuNgay, denNgay);
        public List<DonHang> GetByKhachHang(string maKh) => _dal.GetByKhachHang(maKh);

        public DonHang? GetById(string maDh)
        {
            if (string.IsNullOrEmpty(maDh)) return null;
            return _dal.GetById(maDh);
        }

        public (bool Success, string Message) Insert(DonHang obj)
        {
            if (string.IsNullOrEmpty(obj.Madonhang))
                obj.Madonhang = _dal.GenerateNewId();
            if (_dal.Exists(obj.Madonhang))
                return (false, "Mã đơn hàng đã tồn tại");
            if (string.IsNullOrEmpty(obj.Makh))
                return (false, "Vui lòng chọn khách hàng");
            // Userid có thể null nếu đơn hàng từ website (khách tự đặt)

            var result = _dal.Insert(obj);
            return result > 0 ? (true, "Tạo đơn hàng thành công") : (false, "Có lỗi xảy ra");
        }

        public (bool Success, string Message) Update(DonHang obj)
        {
            if (string.IsNullOrEmpty(obj.Madonhang))
                return (false, "Mã đơn hàng không được để trống");
            if (!_dal.Exists(obj.Madonhang))
                return (false, "Đơn hàng không tồn tại");

            var result = _dal.Update(obj);
            return result > 0 ? (true, "Cập nhật thành công") : (false, "Có lỗi xảy ra");
        }

        public (bool Success, string Message) UpdateTrangThai(string maDh, string trangThai, string? nguoiDuyetId = null)
        {
            if (string.IsNullOrEmpty(maDh))
                return (false, "Mã đơn hàng không được để trống");
            if (!_dal.Exists(maDh))
                return (false, "Đơn hàng không tồn tại");

            var validStatuses = new[] { "Chờ xác nhận", "Đã xác nhận", "Chờ xử lý", "Đang xử lý", "Đang giao", "Đã giao", "Hoàn thành", "Đã hủy" };
            if (!validStatuses.Contains(trangThai))
                return (false, "Trạng thái không hợp lệ");

            var result = _dal.UpdateTrangThai(maDh, trangThai, nguoiDuyetId);
            return result > 0 ? (true, "Cập nhật trạng thái thành công") : (false, "Có lỗi xảy ra");
        }

        public (bool Success, string Message) Delete(string maDh)
        {
            if (string.IsNullOrEmpty(maDh))
                return (false, "Mã không được để trống");
            if (!_dal.Exists(maDh))
                return (false, "Đơn hàng không tồn tại");

            try
            {
                var result = _dal.Delete(maDh);
                return result > 0 ? (true, "Xóa đơn hàng thành công") : (false, "Có lỗi xảy ra khi xóa");
            }
            catch (Exception)
            {
                return (false, "Không thể xóa đơn hàng này vì đang có chi tiết, thanh toán hoặc vận chuyển liên quan");
            }
        }

        public bool Exists(string maDh) => _dal.Exists(maDh);
        public int Count() => _dal.Count();
        public int CountByTrangThai(string trangThai) => _dal.CountByTrangThai(trangThai);
        public string GenerateNewId() => _dal.GenerateNewId();
        public decimal GetTongDoanhThu() => _dal.GetTongDoanhThu();
        public decimal GetDoanhThuTheoThoiGian(DateTime tuNgay, DateTime denNgay) => _dal.GetDoanhThuTheoThoiGian(tuNgay, denNgay);
        
        // Lấy đơn hàng của khách hàng kèm tổng tiền
        public List<DonHang> GetByKhachHangWithTotal(string maKh) => _dal.GetByKhachHangWithTotal(maKh);
        
        // Tính tổng chi tiêu của khách hàng (chỉ đơn hoàn thành)
        public decimal GetTongChiTieuKhachHang(string maKh) => _dal.GetTongChiTieuKhachHang(maKh);
    }
}
