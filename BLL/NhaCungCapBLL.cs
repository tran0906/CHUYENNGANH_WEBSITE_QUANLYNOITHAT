using DOANCHUYENNGANH_WEB_QLNOITHAT.DAL;
using DOANCHUYENNGANH_WEB_QLNOITHAT.Models;

namespace DOANCHUYENNGANH_WEB_QLNOITHAT.BLL
{
    public class NhaCungCapBLL
    {
        private readonly NhaCungCapDAL _dal = new NhaCungCapDAL();

        public List<NhaCungCap> GetAll() => _dal.GetAll();

        public NhaCungCap? GetById(string ma)
        {
            if (string.IsNullOrEmpty(ma)) return null;
            return _dal.GetById(ma);
        }

        public (bool Success, string Message) Insert(NhaCungCap obj)
        {
            if (string.IsNullOrEmpty(obj.Mancc))
                return (false, "Mã nhà cung cấp không được để trống");

            if (_dal.Exists(obj.Mancc))
                return (false, "Mã nhà cung cấp đã tồn tại");

            return _dal.Insert(obj) > 0 
                ? (true, "Thêm nhà cung cấp thành công") 
                : (false, "Có lỗi xảy ra");
        }

        public (bool Success, string Message) Update(NhaCungCap obj)
        {
            if (string.IsNullOrEmpty(obj.Mancc))
                return (false, "Mã nhà cung cấp không được để trống");

            if (!_dal.Exists(obj.Mancc))
                return (false, "Nhà cung cấp không tồn tại");

            return _dal.Update(obj) > 0 
                ? (true, "Cập nhật thành công") 
                : (false, "Có lỗi xảy ra");
        }

        public (bool Success, string Message) Delete(string ma)
        {
            if (string.IsNullOrEmpty(ma))
                return (false, "Mã không được để trống");

            if (!_dal.Exists(ma))
                return (false, "Nhà cung cấp không tồn tại");

            try
            {
                return _dal.Delete(ma) > 0 
                    ? (true, "Xóa nhà cung cấp thành công") 
                    : (false, "Có lỗi xảy ra khi xóa");
            }
            catch (Exception)
            {
                return (false, "Không thể xóa nhà cung cấp này vì đang có sản phẩm cung cấp liên quan");
            }
        }

        public bool Exists(string ma) => _dal.Exists(ma);
    }
}
