using DOANCHUYENNGANH_WEB_QLNOITHAT.DAL;
using DOANCHUYENNGANH_WEB_QLNOITHAT.Models;

namespace DOANCHUYENNGANH_WEB_QLNOITHAT.BLL
{
    /// <summary>
    /// BLL cho quan hệ SuDung (MucDichSuDung - SanPham)
    /// </summary>
    public class SuDungBLL
    {
        private readonly SuDungDAL _dal = new SuDungDAL();
        private readonly MucDichSuDungDAL _mdsdDal = new MucDichSuDungDAL();
        private readonly SanPhamDAL _spDal = new SanPhamDAL();

        /// <summary>
        /// Lấy danh sách sản phẩm theo mục đích sử dụng
        /// </summary>
        public List<SanPham> GetSanPhamByMdsd(string mamdsd)
        {
            if (string.IsNullOrEmpty(mamdsd)) return new List<SanPham>();
            return _dal.GetSanPhamByMdsd(mamdsd);
        }

        /// <summary>
        /// Thêm quan hệ mới
        /// </summary>
        public (bool Success, string Message) Insert(string mamdsd, string masp)
        {
            if (string.IsNullOrEmpty(mamdsd) || string.IsNullOrEmpty(masp))
                return (false, "Vui lòng chọn đầy đủ thông tin");

            if (!_mdsdDal.Exists(mamdsd))
                return (false, "Mục đích sử dụng không tồn tại");

            if (!_spDal.Exists(masp))
                return (false, "Sản phẩm không tồn tại");

            if (_dal.Exists(mamdsd, masp))
                return (false, "Sản phẩm đã được gán cho mục đích sử dụng này");

            return _dal.Insert(mamdsd, masp) > 0
                ? (true, "Thêm thành công")
                : (false, "Có lỗi xảy ra");
        }

        /// <summary>
        /// Xóa quan hệ
        /// </summary>
        public (bool Success, string Message) Delete(string mamdsd, string masp)
        {
            if (string.IsNullOrEmpty(mamdsd) || string.IsNullOrEmpty(masp))
                return (false, "Thông tin không hợp lệ");

            if (!_dal.Exists(mamdsd, masp))
                return (false, "Quan hệ không tồn tại");

            return _dal.Delete(mamdsd, masp) > 0
                ? (true, "Xóa thành công")
                : (false, "Có lỗi xảy ra");
        }

        public bool Exists(string mamdsd, string masp) => _dal.Exists(mamdsd, masp);
    }
}
