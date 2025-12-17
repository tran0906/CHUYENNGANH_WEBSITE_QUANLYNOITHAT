using DOANCHUYENNGANH_WEB_QLNOITHAT.DAL;
using DOANCHUYENNGANH_WEB_QLNOITHAT.Models;

namespace DOANCHUYENNGANH_WEB_QLNOITHAT.BLL
{
    public class ThanhToanBLL
    {
        private readonly ThanhToanDAL _dal = new ThanhToanDAL();

        public List<ThanhToan> GetAll() => _dal.GetAll();
        public List<ThanhToan> GetByDonHang(string maDh) => _dal.GetByDonHang(maDh);

        public ThanhToan? GetById(string ma)
        {
            if (string.IsNullOrEmpty(ma)) return null;
            return _dal.GetById(ma);
        }

        public (bool Success, string Message) Insert(ThanhToan obj)
        {
            if (string.IsNullOrEmpty(obj.Mathanhtoan))
                obj.Mathanhtoan = _dal.GenerateNewId();
            if (string.IsNullOrEmpty(obj.Madonhang))
                return (false, "Vui lòng chọn đơn hàng");
            if (obj.Sotien <= 0)
                return (false, "Số tiền phải lớn hơn 0");

            return _dal.Insert(obj) > 0 ? (true, "Thêm thành công") : (false, "Có lỗi xảy ra");
        }

        public (bool Success, string Message) Update(ThanhToan obj)
        {
            if (string.IsNullOrEmpty(obj.Mathanhtoan))
                return (false, "Mã thanh toán không được để trống");
            if (!_dal.Exists(obj.Mathanhtoan))
                return (false, "Thanh toán không tồn tại");

            return _dal.Update(obj) > 0 ? (true, "Cập nhật thành công") : (false, "Có lỗi xảy ra");
        }

        public (bool Success, string Message) Delete(string ma)
        {
            if (string.IsNullOrEmpty(ma))
                return (false, "Mã không được để trống");
            if (!_dal.Exists(ma))
                return (false, "Thanh toán không tồn tại");

            try
            {
                return _dal.Delete(ma) > 0 ? (true, "Xóa thành công") : (false, "Có lỗi xảy ra khi xóa");
            }
            catch (Exception)
            {
                return (false, "Không thể xóa thanh toán này vì đang có dữ liệu liên quan");
            }
        }

        public bool Exists(string ma) => _dal.Exists(ma);
        public string GenerateNewId() => _dal.GenerateNewId();
    }
}
