// FILE: BLL/SanPhamBLL.cs - Tầng nghiệp vụ cho Sản phẩm (validate, tính giá khuyến mãi)

using DOANCHUYENNGANH_WEB_QLNOITHAT.DAL;
using DOANCHUYENNGANH_WEB_QLNOITHAT.Models;

namespace DOANCHUYENNGANH_WEB_QLNOITHAT.BLL
{
    public class SanPhamBLL
    {
        private readonly SanPhamDAL _dal = new SanPhamDAL(); // Gọi tầng DAL
        private readonly QuangBaDAL _quangBaDAL = new QuangBaDAL();

        public List<SanPham> GetAll() => _dal.GetAll();
        
        // Lấy tất cả SP kèm giá khuyến mãi (nếu có)
        public List<SanPham> GetAllWithPromotion()
        {
            var products = _dal.GetAll();
            var promotionProducts = _quangBaDAL.GetAllPromotedProducts();
            
            foreach (var sp in products)
            {
                if (promotionProducts.TryGetValue(sp.Masp!, out int phantram))
                {
                    sp.PhanTramGiam = phantram;
                    sp.GiaGoc = sp.Giaban;
                    sp.Giaban = Math.Round((sp.Giaban ?? 0) * (100 - phantram) / 100, 0);
                }
            }
            return products;
        }
        
        // Lấy SP theo mã kèm giá khuyến mãi
        public SanPham? GetByIdWithPromotion(string maSp)
        {
            var sp = _dal.GetById(maSp);
            if (sp != null)
            {
                var phantram = _quangBaDAL.GetPhanTramGiam(maSp);
                if (phantram.HasValue && phantram.Value > 0)
                {
                    sp.PhanTramGiam = phantram.Value;
                    sp.GiaGoc = sp.Giaban;
                    sp.Giaban = Math.Round((sp.Giaban ?? 0) * (100 - phantram.Value) / 100, 0);
                }
            }
            return sp;
        }

        public List<SanPham> Search(string? search, string? nhomSp, string? vatLieu)
            => _dal.Search(search, nhomSp, vatLieu);

        public SanPham? GetById(string maSp)
        {
            if (string.IsNullOrEmpty(maSp)) return null;
            return _dal.GetById(maSp);
        }

        public (bool Success, string Message) Insert(SanPham sp)
        {
            if (string.IsNullOrEmpty(sp.Masp))
                return (false, "Mã sản phẩm không được để trống");
            if (_dal.Exists(sp.Masp))
                return (false, "Mã sản phẩm đã tồn tại");
            if (string.IsNullOrEmpty(sp.Tensp))
                return (false, "Tên sản phẩm không được để trống");
            if (sp.Giaban.HasValue && sp.Giaban < 0)
                return (false, "Giá bán không được âm");
            if (sp.Soluongton.HasValue && sp.Soluongton < 0)
                return (false, "Số lượng tồn không được âm");

            var result = _dal.Insert(sp);
            return result > 0 ? (true, "Thêm sản phẩm thành công") : (false, "Có lỗi xảy ra");
        }

        public (bool Success, string Message) Update(SanPham sp)
        {
            if (string.IsNullOrEmpty(sp.Masp))
                return (false, "Mã sản phẩm không được để trống");
            if (!_dal.Exists(sp.Masp))
                return (false, "Sản phẩm không tồn tại");
            if (string.IsNullOrEmpty(sp.Tensp))
                return (false, "Tên sản phẩm không được để trống");
            if (sp.Giaban.HasValue && sp.Giaban < 0)
                return (false, "Giá bán không được âm");
            if (sp.Soluongton.HasValue && sp.Soluongton < 0)
                return (false, "Số lượng tồn không được âm");

            var result = _dal.Update(sp);
            return result > 0 ? (true, "Cập nhật thành công") : (false, "Có lỗi xảy ra");
        }

        public (bool Success, string Message) Delete(string maSp)
        {
            if (string.IsNullOrEmpty(maSp))
                return (false, "Mã sản phẩm không được để trống");
            if (!_dal.Exists(maSp))
                return (false, "Sản phẩm không tồn tại");

            try
            {
                var result = _dal.Delete(maSp);
                return result > 0 ? (true, "Xóa sản phẩm thành công") : (false, "Có lỗi xảy ra khi xóa");
            }
            catch (Exception)
            {
                return (false, "Không thể xóa sản phẩm này vì đang có trong đơn hàng hoặc dữ liệu liên quan");
            }
        }

        public bool Exists(string maSp) => _dal.Exists(maSp);
        public int Count() => _dal.Count();
    }
}
