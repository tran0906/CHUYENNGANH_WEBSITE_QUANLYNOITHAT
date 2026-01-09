// FILE: BLL/StoredProcedureBLL.cs
// TẦNG BLL - Gọi các Stored Procedures (đặt hàng, hủy đơn, thống kê...)
// LUỒNG: Controller → BLL → DAL → Stored Procedure

using System.Data;
using DOANCHUYENNGANH_WEB_QLNOITHAT.DAL;
using DOANCHUYENNGANH_WEB_QLNOITHAT.Models;

namespace DOANCHUYENNGANH_WEB_QLNOITHAT.BLL
{
    public class StoredProcedureBLL
    {
        private readonly StoredProcedureDAL _spDal = new StoredProcedureDAL();

        #region Đặt hàng
        /// <summary>
        /// Đặt hàng - tạo đơn hàng + chi tiết + trừ tồn kho (1 transaction)
        /// </summary>
        public (bool Success, string Message, string? MaDonHang) DatHang(
            string maKh, string maSp, int soLuong, string? ghiChu = null)
        {
            if (string.IsNullOrEmpty(maKh))
                return (false, "Mã khách hàng không được để trống", null);
            if (string.IsNullOrEmpty(maSp))
                return (false, "Mã sản phẩm không được để trống", null);
            if (soLuong <= 0)
                return (false, "Số lượng phải lớn hơn 0", null);

            try
            {
                var result = _spDal.DatHang(maKh, maSp, soLuong, ghiChu);
                if (result.Rows.Count > 0)
                {
                    var maDonHang = result.Rows[0]["MADONHANG"]?.ToString();
                    return (true, "Đặt hàng thành công", maDonHang);
                }
                return (false, "Có lỗi xảy ra khi đặt hàng", null);
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi: {ex.Message}", null);
            }
        }
        #endregion

        #region Hủy đơn hàng
        /// <summary>
        /// Hủy đơn hàng - hoàn lại tồn kho
        /// </summary>
        public (bool Success, string Message) HuyDonHang(string maDonHang)
        {
            if (string.IsNullOrEmpty(maDonHang))
                return (false, "Mã đơn hàng không được để trống");

            try
            {
                var result = _spDal.HuyDonHang(maDonHang);
                return result > 0 
                    ? (true, "Hủy đơn hàng thành công") 
                    : (false, "Không thể hủy đơn hàng");
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi: {ex.Message}");
            }
        }
        #endregion

        #region Hoàn trả
        /// <summary>
        /// Hoàn trả sản phẩm
        /// </summary>
        public (bool Success, string Message) HoanTra(string maDonHang, string maSp, int soLuong)
        {
            if (string.IsNullOrEmpty(maDonHang))
                return (false, "Mã đơn hàng không được để trống");
            if (string.IsNullOrEmpty(maSp))
                return (false, "Mã sản phẩm không được để trống");
            if (soLuong <= 0)
                return (false, "Số lượng hoàn trả phải lớn hơn 0");

            try
            {
                var result = _spDal.HoanTra(maDonHang, maSp, soLuong);
                return result > 0 
                    ? (true, "Hoàn trả thành công") 
                    : (false, "Không thể hoàn trả");
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi: {ex.Message}");
            }
        }
        #endregion

        #region Xác nhận thanh toán
        /// <summary>
        /// Nhân viên xác nhận thanh toán
        /// </summary>
        public (bool Success, string Message) XacNhanThanhToan(string maDonHang, string userId)
        {
            if (string.IsNullOrEmpty(maDonHang))
                return (false, "Mã đơn hàng không được để trống");
            if (string.IsNullOrEmpty(userId))
                return (false, "Mã nhân viên không được để trống");

            try
            {
                var result = _spDal.XacNhanThanhToan(maDonHang, userId);
                return result > 0 
                    ? (true, "Xác nhận thanh toán thành công") 
                    : (false, "Không thể xác nhận thanh toán");
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi: {ex.Message}");
            }
        }
        #endregion

        #region Xuất kho
        /// <summary>
        /// Nhân viên xuất kho
        /// </summary>
        public (bool Success, string Message) XuatKho(string maDonHang, string userId)
        {
            if (string.IsNullOrEmpty(maDonHang))
                return (false, "Mã đơn hàng không được để trống");

            try
            {
                var result = _spDal.XuatKho(maDonHang, userId);
                return result > 0 
                    ? (true, "Xuất kho thành công") 
                    : (false, "Không thể xuất kho");
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi: {ex.Message}");
            }
        }
        #endregion

        #region Điều phối giao hàng
        /// <summary>
        /// Nhân viên điều phối giao hàng
        /// </summary>
        public (bool Success, string Message) DieuPhoiGiaoHang(string maDonHang, string donViVanChuyen, string userId)
        {
            if (string.IsNullOrEmpty(maDonHang))
                return (false, "Mã đơn hàng không được để trống");

            try
            {
                var result = _spDal.DieuPhoiGiaoHang(maDonHang, donViVanChuyen, userId);
                return result > 0 
                    ? (true, "Điều phối giao hàng thành công") 
                    : (false, "Không thể điều phối giao hàng");
            }
            catch (Exception ex)
            {
                return (false, $"Lỗi: {ex.Message}");
            }
        }
        #endregion

        #region Kiểm tra tồn kho
        /// <summary>
        /// Kiểm tra tồn kho sản phẩm
        /// </summary>
        public DataTable KiemTraTonKho(string? maSp = null)
        {
            return _spDal.KiemTraTonKho(maSp);
        }
        #endregion

        #region Xem đơn hàng
        /// <summary>
        /// Xem chi tiết đơn hàng
        /// </summary>
        public DataTable XemDonHang(string? maDonHang = null, string? trangThai = null)
        {
            return _spDal.XemDonHang(maDonHang, trangThai);
        }
        #endregion

        #region Báo cáo doanh thu
        /// <summary>
        /// Doanh thu theo ngày
        /// </summary>
        public DataTable DoanhThuNgay(DateTime ngay)
        {
            return _spDal.DoanhThuNgay(ngay);
        }

        /// <summary>
        /// Thống kê doanh thu theo tháng
        /// </summary>
        public DataTable ThongKeDoanhThuThang(int thang, int nam)
        {
            return _spDal.ThongKeDoanhThuThang(thang, nam);
        }

        /// <summary>
        /// Thống kê doanh thu theo quý
        /// </summary>
        public DataTable ThongKeDoanhThuQuy(int quy, int nam)
        {
            return _spDal.ThongKeDoanhThuQuy(quy, nam);
        }

        /// <summary>
        /// Thống kê doanh thu theo năm
        /// </summary>
        public DataTable ThongKeDoanhThuNam(int nam)
        {
            return _spDal.ThongKeDoanhThuNam(nam);
        }

        /// <summary>
        /// Top sản phẩm bán chạy
        /// </summary>
        public DataTable TopSanPhamBanChay(int top = 10, int? nam = null)
        {
            return _spDal.TopSanPhamBanChay(top, nam);
        }
        #endregion
    }
}
