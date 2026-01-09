// FILE: BLL/MucDichSuDungBLL.cs
// TẦNG BLL - Xử lý nghiệp vụ Mục đích sử dụng
// LUỒNG: Controller → BLL → DAL → Database

using DOANCHUYENNGANH_WEB_QLNOITHAT.DAL;
using DOANCHUYENNGANH_WEB_QLNOITHAT.Models;

namespace DOANCHUYENNGANH_WEB_QLNOITHAT.BLL
{
    public class MucDichSuDungBLL
    {
        private readonly MucDichSuDungDAL _dal = new MucDichSuDungDAL();

        public List<MucDichSuDung> GetAll() => _dal.GetAll();

        public MucDichSuDung? GetById(string ma)
        {
            if (string.IsNullOrEmpty(ma)) return null;
            return _dal.GetById(ma);
        }

        public (bool Success, string Message) Insert(MucDichSuDung obj)
        {
            if (string.IsNullOrEmpty(obj.Mamdsd))
                return (false, "Mã mục đích sử dụng không được để trống");

            if (_dal.Exists(obj.Mamdsd))
                return (false, "Mã mục đích sử dụng đã tồn tại");

            return _dal.Insert(obj) > 0 
                ? (true, "Thêm mục đích sử dụng thành công") 
                : (false, "Có lỗi xảy ra");
        }

        public (bool Success, string Message) Update(MucDichSuDung obj)
        {
            if (string.IsNullOrEmpty(obj.Mamdsd))
                return (false, "Mã mục đích sử dụng không được để trống");

            if (!_dal.Exists(obj.Mamdsd))
                return (false, "Mục đích sử dụng không tồn tại");

            return _dal.Update(obj) > 0 
                ? (true, "Cập nhật thành công") 
                : (false, "Có lỗi xảy ra");
        }

        public (bool Success, string Message) Delete(string ma)
        {
            if (string.IsNullOrEmpty(ma))
                return (false, "Mã không được để trống");

            if (!_dal.Exists(ma))
                return (false, "Mục đích sử dụng không tồn tại");

            try
            {
                return _dal.Delete(ma) > 0 
                    ? (true, "Xóa thành công") 
                    : (false, "Có lỗi xảy ra khi xóa");
            }
            catch (Exception)
            {
                return (false, "Không thể xóa mục đích sử dụng này vì đang có sản phẩm liên quan");
            }
        }

        public bool Exists(string ma) => _dal.Exists(ma);

        public List<MucDichSuDung> Search(string? keyword) => _dal.Search(keyword);
    }
}
