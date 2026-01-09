// FILE: BLL/QuanBaSpBLL.cs
// TẦNG BLL - Xử lý nghiệp vụ Đợt quảng bá/giảm giá
// LUỒNG: Controller → BLL → DAL → Database

using DOANCHUYENNGANH_WEB_QLNOITHAT.DAL;
using DOANCHUYENNGANH_WEB_QLNOITHAT.Models;

namespace DOANCHUYENNGANH_WEB_QLNOITHAT.BLL
{
    public class QuanBaSpBLL
    {
        private readonly QuanBaSpDAL _dal = new QuanBaSpDAL();

        public List<QuanBaSp> GetAll() => _dal.GetAll();

        public QuanBaSp? GetById(string ma)
        {
            if (string.IsNullOrEmpty(ma)) return null;
            return _dal.GetById(ma);
        }

        public (bool Success, string Message) Insert(QuanBaSp obj)
        {
            if (string.IsNullOrEmpty(obj.Madotgiamgia))
                return (false, "Mã đợt giảm giá không được để trống");

            if (_dal.Exists(obj.Madotgiamgia))
                return (false, "Mã đợt giảm giá đã tồn tại");

            if (obj.Ngaybatdau.HasValue && obj.Ngayketthuc.HasValue && obj.Ngaybatdau > obj.Ngayketthuc)
                return (false, "Ngày bắt đầu phải trước ngày kết thúc");

            return _dal.Insert(obj) > 0 
                ? (true, "Thêm đợt quảng bá thành công") 
                : (false, "Có lỗi xảy ra");
        }

        public (bool Success, string Message) Update(QuanBaSp obj)
        {
            if (string.IsNullOrEmpty(obj.Madotgiamgia))
                return (false, "Mã đợt giảm giá không được để trống");

            if (!_dal.Exists(obj.Madotgiamgia))
                return (false, "Đợt quảng bá không tồn tại");

            if (obj.Ngaybatdau.HasValue && obj.Ngayketthuc.HasValue && obj.Ngaybatdau > obj.Ngayketthuc)
                return (false, "Ngày bắt đầu phải trước ngày kết thúc");

            return _dal.Update(obj) > 0 
                ? (true, "Cập nhật thành công") 
                : (false, "Có lỗi xảy ra");
        }

        public (bool Success, string Message) Delete(string ma)
        {
            if (string.IsNullOrEmpty(ma))
                return (false, "Mã không được để trống");

            if (!_dal.Exists(ma))
                return (false, "Đợt quảng bá không tồn tại");

            try
            {
                return _dal.Delete(ma) > 0 
                    ? (true, "Xóa thành công") 
                    : (false, "Có lỗi xảy ra khi xóa");
            }
            catch (Exception)
            {
                return (false, "Không thể xóa đợt quảng bá này vì đang có sản phẩm liên quan");
            }
        }

        public bool Exists(string ma) => _dal.Exists(ma);
    }
}
