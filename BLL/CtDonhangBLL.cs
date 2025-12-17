using DOANCHUYENNGANH_WEB_QLNOITHAT.DAL;
using DOANCHUYENNGANH_WEB_QLNOITHAT.Models;

namespace DOANCHUYENNGANH_WEB_QLNOITHAT.BLL
{
    public class CtDonhangBLL
    {
        private readonly CtDonhangDAL _dal = new CtDonhangDAL();

        public List<CtDonhang> GetAll() => _dal.GetAll();
        public List<CtDonhang> GetByDonHang(string maDh) => _dal.GetByDonHang(maDh);

        public CtDonhang? GetById(string maSp, string maDh)
        {
            if (string.IsNullOrEmpty(maSp) || string.IsNullOrEmpty(maDh)) return null;
            return _dal.GetById(maSp, maDh);
        }

        public (bool Success, string Message) Insert(CtDonhang obj)
        {
            if (string.IsNullOrEmpty(obj.Masp) || string.IsNullOrEmpty(obj.Madonhang))
                return (false, "Mã sản phẩm và mã đơn hàng không được để trống");
            if (obj.Soluong <= 0)
                return (false, "Số lượng phải lớn hơn 0");

            obj.Thanhtien = (obj.Soluong ?? 0) * (obj.Dongia ?? 0);
            return _dal.Insert(obj) > 0 ? (true, "Thêm thành công") : (false, "Có lỗi xảy ra");
        }

        public (bool Success, string Message) Update(CtDonhang obj)
        {
            if (string.IsNullOrEmpty(obj.Masp) || string.IsNullOrEmpty(obj.Madonhang))
                return (false, "Mã sản phẩm và mã đơn hàng không được để trống");

            obj.Thanhtien = (obj.Soluong ?? 0) * (obj.Dongia ?? 0);
            return _dal.Update(obj) > 0 ? (true, "Cập nhật thành công") : (false, "Có lỗi xảy ra");
        }

        public (bool Success, string Message) Delete(string maSp, string maDh)
        {
            if (string.IsNullOrEmpty(maSp) || string.IsNullOrEmpty(maDh))
                return (false, "Mã không được để trống");
            return _dal.Delete(maSp, maDh) > 0 ? (true, "Xóa thành công") : (false, "Có lỗi xảy ra");
        }

        public decimal GetTongTienByDonHang(string maDh) => _dal.GetTongTienByDonHang(maDh);
    }
}
