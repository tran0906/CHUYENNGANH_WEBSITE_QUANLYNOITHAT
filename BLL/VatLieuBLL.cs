using DOANCHUYENNGANH_WEB_QLNOITHAT.DAL;
using DOANCHUYENNGANH_WEB_QLNOITHAT.Models;

namespace DOANCHUYENNGANH_WEB_QLNOITHAT.BLL
{
    public class VatLieuBLL
    {
        private readonly VatLieuDAL _dal = new VatLieuDAL();

        public List<VatLieu> GetAll() => _dal.GetAll();

        public VatLieu? GetById(string ma)
        {
            if (string.IsNullOrEmpty(ma)) return null;
            return _dal.GetById(ma);
        }

        public (bool Success, string Message) Insert(VatLieu obj)
        {
            if (string.IsNullOrEmpty(obj.Mavl))
                return (false, "Mã vật liệu không được để trống");
            if (_dal.Exists(obj.Mavl))
                return (false, "Mã vật liệu đã tồn tại");

            var result = _dal.Insert(obj);
            return result > 0 ? (true, "Thêm thành công") : (false, "Có lỗi xảy ra");
        }

        public (bool Success, string Message) Update(VatLieu obj)
        {
            if (string.IsNullOrEmpty(obj.Mavl))
                return (false, "Mã vật liệu không được để trống");
            if (!_dal.Exists(obj.Mavl))
                return (false, "Vật liệu không tồn tại");

            var result = _dal.Update(obj);
            return result > 0 ? (true, "Cập nhật thành công") : (false, "Có lỗi xảy ra");
        }

        public (bool Success, string Message) Delete(string ma)
        {
            if (string.IsNullOrEmpty(ma))
                return (false, "Mã không được để trống");
            if (!_dal.Exists(ma))
                return (false, "Vật liệu không tồn tại");

            try
            {
                var result = _dal.Delete(ma);
                return result > 0 ? (true, "Xóa vật liệu thành công") : (false, "Có lỗi xảy ra khi xóa");
            }
            catch (Exception)
            {
                return (false, "Không thể xóa vật liệu này vì đang có sản phẩm sử dụng");
            }
        }

        public bool Exists(string ma) => _dal.Exists(ma);
    }
}
