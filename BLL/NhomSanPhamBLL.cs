// FILE: BLL/NhomSanPhamBLL.cs
// TẦNG BLL - Xử lý nghiệp vụ Nhóm sản phẩm
// LUỒNG: Controller → BLL → DAL → Database

using DOANCHUYENNGANH_WEB_QLNOITHAT.DAL;
using DOANCHUYENNGANH_WEB_QLNOITHAT.Models;

namespace DOANCHUYENNGANH_WEB_QLNOITHAT.BLL
{
    public class NhomSanPhamBLL
    {
        private readonly NhomSanPhamDAL _dal = new NhomSanPhamDAL();

        public List<NhomSanPham> GetAll() => _dal.GetAll();

        public NhomSanPham? GetById(string ma)
        {
            if (string.IsNullOrEmpty(ma)) return null;
            return _dal.GetById(ma);
        }

        public (bool Success, string Message) Insert(NhomSanPham obj)
        {
            if (string.IsNullOrEmpty(obj.Manhomsp))
                return (false, "Mã nhóm sản phẩm không được để trống");
            if (_dal.Exists(obj.Manhomsp))
                return (false, "Mã nhóm sản phẩm đã tồn tại");

            var result = _dal.Insert(obj);
            return result > 0 ? (true, "Thêm thành công") : (false, "Có lỗi xảy ra");
        }

        public (bool Success, string Message) Update(NhomSanPham obj)
        {
            if (string.IsNullOrEmpty(obj.Manhomsp))
                return (false, "Mã nhóm sản phẩm không được để trống");
            if (!_dal.Exists(obj.Manhomsp))
                return (false, "Nhóm sản phẩm không tồn tại");

            var result = _dal.Update(obj);
            return result > 0 ? (true, "Cập nhật thành công") : (false, "Có lỗi xảy ra");
        }

        public (bool Success, string Message) Delete(string ma)
        {
            if (string.IsNullOrEmpty(ma))
                return (false, "Mã không được để trống");
            if (!_dal.Exists(ma))
                return (false, "Nhóm sản phẩm không tồn tại");

            try
            {
                var result = _dal.Delete(ma);
                return result > 0 ? (true, "Xóa nhóm sản phẩm thành công") : (false, "Có lỗi xảy ra khi xóa");
            }
            catch (Exception)
            {
                return (false, "Không thể xóa nhóm sản phẩm này vì đang có sản phẩm thuộc nhóm");
            }
        }

        public bool Exists(string ma) => _dal.Exists(ma);
    }
}
