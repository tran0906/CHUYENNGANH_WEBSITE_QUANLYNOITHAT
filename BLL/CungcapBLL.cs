// FILE: BLL/CungcapBLL.cs
// TẦNG BLL - Xử lý nghiệp vụ Cung cấp (NCC - SP)
// LUỒNG: Controller → BLL → DAL → Database

using DOANCHUYENNGANH_WEB_QLNOITHAT.DAL;
using DOANCHUYENNGANH_WEB_QLNOITHAT.Models;

namespace DOANCHUYENNGANH_WEB_QLNOITHAT.BLL
{
    public class CungcapBLL
    {
        private readonly CungcapDAL _dal = new CungcapDAL();

        public List<Cungcap> GetAll() => _dal.GetAll();

        public Cungcap? GetById(string maNcc, string maSp)
        {
            if (string.IsNullOrEmpty(maNcc) || string.IsNullOrEmpty(maSp)) return null;
            return _dal.GetById(maNcc, maSp);
        }

        public (bool Success, string Message) Insert(Cungcap obj)
        {
            if (string.IsNullOrEmpty(obj.Mancc) || string.IsNullOrEmpty(obj.Masp))
                return (false, "Mã nhà cung cấp và mã sản phẩm không được để trống");

            if (_dal.Exists(obj.Mancc, obj.Masp))
                return (false, "Quan hệ cung cấp đã tồn tại");

            if (obj.Soluongsp.HasValue && obj.Soluongsp < 0)
                return (false, "Số lượng không được âm");

            return _dal.Insert(obj) > 0 
                ? (true, "Thêm quan hệ cung cấp thành công") 
                : (false, "Có lỗi xảy ra");
        }

        public (bool Success, string Message) Update(Cungcap obj)
        {
            if (string.IsNullOrEmpty(obj.Mancc) || string.IsNullOrEmpty(obj.Masp))
                return (false, "Mã nhà cung cấp và mã sản phẩm không được để trống");

            if (!_dal.Exists(obj.Mancc, obj.Masp))
                return (false, "Quan hệ cung cấp không tồn tại");

            if (obj.Soluongsp.HasValue && obj.Soluongsp < 0)
                return (false, "Số lượng không được âm");

            return _dal.Update(obj) > 0 
                ? (true, "Cập nhật thành công") 
                : (false, "Có lỗi xảy ra");
        }

        public (bool Success, string Message) Delete(string maNcc, string maSp)
        {
            if (string.IsNullOrEmpty(maNcc) || string.IsNullOrEmpty(maSp))
                return (false, "Mã không được để trống");

            if (!_dal.Exists(maNcc, maSp))
                return (false, "Quan hệ cung cấp không tồn tại");

            return _dal.Delete(maNcc, maSp) > 0 
                ? (true, "Xóa thành công") 
                : (false, "Có lỗi xảy ra");
        }

        public bool Exists(string maNcc, string maSp) => _dal.Exists(maNcc, maSp);
    }
}
