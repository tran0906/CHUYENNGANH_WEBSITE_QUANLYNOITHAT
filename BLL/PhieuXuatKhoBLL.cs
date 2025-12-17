using DOANCHUYENNGANH_WEB_QLNOITHAT.DAL;
using DOANCHUYENNGANH_WEB_QLNOITHAT.Models;

namespace DOANCHUYENNGANH_WEB_QLNOITHAT.BLL
{
    public class PhieuXuatKhoBLL
    {
        private readonly PhieuXuatKhoDAL _dal = new PhieuXuatKhoDAL();

        public List<PhieuXuatKho> GetAll() => _dal.GetAll();

        public PhieuXuatKho? GetById(string ma)
        {
            if (string.IsNullOrEmpty(ma)) return null;
            return _dal.GetById(ma);
        }

        public (bool Success, string Message) Insert(PhieuXuatKho obj)
        {
            if (string.IsNullOrEmpty(obj.Maphieuxuat))
                obj.Maphieuxuat = _dal.GenerateNewId();
            if (string.IsNullOrEmpty(obj.Madonhang))
                return (false, "Vui lòng chọn đơn hàng");

            return _dal.Insert(obj) > 0 ? (true, "Thêm thành công") : (false, "Có lỗi xảy ra");
        }

        public (bool Success, string Message) Update(PhieuXuatKho obj)
        {
            if (string.IsNullOrEmpty(obj.Maphieuxuat))
                return (false, "Mã phiếu xuất không được để trống");
            if (!_dal.Exists(obj.Maphieuxuat))
                return (false, "Phiếu xuất kho không tồn tại");

            return _dal.Update(obj) > 0 ? (true, "Cập nhật thành công") : (false, "Có lỗi xảy ra");
        }

        public (bool Success, string Message) Delete(string ma)
        {
            if (string.IsNullOrEmpty(ma))
                return (false, "Mã không được để trống");
            if (!_dal.Exists(ma))
                return (false, "Phiếu xuất kho không tồn tại");

            try
            {
                return _dal.Delete(ma) > 0 ? (true, "Xóa thành công") : (false, "Có lỗi xảy ra khi xóa");
            }
            catch (Exception)
            {
                return (false, "Không thể xóa phiếu xuất kho này vì đang có dữ liệu liên quan");
            }
        }

        public bool Exists(string ma) => _dal.Exists(ma);
        public string GenerateNewId() => _dal.GenerateNewId();
    }
}
