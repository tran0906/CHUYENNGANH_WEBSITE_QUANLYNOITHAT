using DOANCHUYENNGANH_WEB_QLNOITHAT.DAL;
using DOANCHUYENNGANH_WEB_QLNOITHAT.Models;

namespace DOANCHUYENNGANH_WEB_QLNOITHAT.BLL
{
    /// <summary>
    /// BLL cho quan hệ QuangBa (SanPham - QuanBaSp)
    /// </summary>
    public class QuangBaBLL
    {
        private readonly QuangBaDAL _dal = new QuangBaDAL();
        private readonly SanPhamDAL _spDal = new SanPhamDAL();
        private readonly QuanBaSpDAL _qbspDal = new QuanBaSpDAL();

        /// <summary>
        /// Lấy danh sách sản phẩm theo đợt giảm giá
        /// </summary>
        public List<SanPham> GetSanPhamByDotGiamGia(string madotgiamgia)
        {
            if (string.IsNullOrEmpty(madotgiamgia)) return new List<SanPham>();
            return _dal.GetSanPhamByDotGiamGia(madotgiamgia);
        }

        /// <summary>
        /// Lấy danh sách đợt giảm giá theo sản phẩm
        /// </summary>
        public List<QuanBaSp> GetDotGiamGiaByMasp(string masp)
        {
            if (string.IsNullOrEmpty(masp)) return new List<QuanBaSp>();
            return _dal.GetDotGiamGiaByMasp(masp);
        }

        /// <summary>
        /// Thêm sản phẩm vào đợt giảm giá
        /// </summary>
        public (bool Success, string Message) Insert(string masp, string madotgiamgia)
        {
            if (string.IsNullOrEmpty(masp) || string.IsNullOrEmpty(madotgiamgia))
                return (false, "Vui lòng chọn đầy đủ thông tin");

            if (!_spDal.Exists(masp))
                return (false, "Sản phẩm không tồn tại");

            if (!_qbspDal.Exists(madotgiamgia))
                return (false, "Đợt giảm giá không tồn tại");

            if (_dal.Exists(masp, madotgiamgia))
                return (false, "Sản phẩm đã được thêm vào đợt giảm giá này");

            return _dal.Insert(masp, madotgiamgia) > 0
                ? (true, "Thêm thành công")
                : (false, "Có lỗi xảy ra");
        }

        /// <summary>
        /// Xóa sản phẩm khỏi đợt giảm giá
        /// </summary>
        public (bool Success, string Message) Delete(string masp, string madotgiamgia)
        {
            if (string.IsNullOrEmpty(masp) || string.IsNullOrEmpty(madotgiamgia))
                return (false, "Thông tin không hợp lệ");

            if (!_dal.Exists(masp, madotgiamgia))
                return (false, "Quan hệ không tồn tại");

            return _dal.Delete(masp, madotgiamgia) > 0
                ? (true, "Xóa thành công")
                : (false, "Có lỗi xảy ra");
        }

        public bool Exists(string masp, string madotgiamgia) => _dal.Exists(masp, madotgiamgia);
    }
}
