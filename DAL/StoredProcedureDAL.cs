using System.Data;
using Microsoft.Data.SqlClient;
using DOANCHUYENNGANH_WEB_QLNOITHAT.Models;

namespace DOANCHUYENNGANH_WEB_QLNOITHAT.DAL
{
    /// <summary>
    /// DAL gọi các Stored Procedures - Mô hình 3 lớp
    /// </summary>
    public class StoredProcedureDAL
    {
        #region Đặt hàng - sp_DatHang
        public DataTable DatHang(string maKh, string maSp, int soLuong, string? ghiChu = null)
        {
            SqlParameter[] parameters = {
                new SqlParameter("@MAKH", maKh),
                new SqlParameter("@MASP", maSp),
                new SqlParameter("@SOLUONG", soLuong),
                new SqlParameter("@GHICHU", (object?)ghiChu ?? DBNull.Value)
            };
            return SqlConnectionHelper.ExecuteStoredProcedure("sp_DatHang", parameters);
        }
        #endregion

        #region Hủy đơn hàng - sp_HuyDonHang
        public int HuyDonHang(string maDonHang)
        {
            SqlParameter[] parameters = { new SqlParameter("@MADONHANG", maDonHang) };
            return SqlConnectionHelper.ExecuteStoredProcedureNonQuery("sp_HuyDonHang", parameters);
        }
        #endregion

        #region Hoàn trả - sp_HoanTra
        public int HoanTra(string maDonHang, string maSp, int soLuong)
        {
            SqlParameter[] parameters = {
                new SqlParameter("@MADONHANG", maDonHang),
                new SqlParameter("@MASP", maSp),
                new SqlParameter("@SOLUONG", soLuong)
            };
            return SqlConnectionHelper.ExecuteStoredProcedureNonQuery("sp_HoanTra", parameters);
        }
        #endregion

        #region Xác nhận thanh toán - sp_NV_XacNhanThanhToan
        public int XacNhanThanhToan(string maDonHang, string userId)
        {
            SqlParameter[] parameters = {
                new SqlParameter("@MADONHANG", maDonHang),
                new SqlParameter("@USERID", userId)
            };
            return SqlConnectionHelper.ExecuteStoredProcedureNonQuery("sp_NV_XacNhanThanhToan", parameters);
        }
        #endregion

        #region Xuất kho - sp_NV_XuatKho
        public int XuatKho(string maDonHang, string userId)
        {
            SqlParameter[] parameters = {
                new SqlParameter("@MADONHANG", maDonHang),
                new SqlParameter("@USERID", userId)
            };
            return SqlConnectionHelper.ExecuteStoredProcedureNonQuery("sp_NV_XuatKho", parameters);
        }
        #endregion

        #region Điều phối giao hàng - sp_NV_DieuPhoiGiaoHang
        public int DieuPhoiGiaoHang(string maDonHang, string donViVanChuyen, string userId)
        {
            SqlParameter[] parameters = {
                new SqlParameter("@MADONHANG", maDonHang),
                new SqlParameter("@DONVIVC", donViVanChuyen),
                new SqlParameter("@USERID", userId)
            };
            return SqlConnectionHelper.ExecuteStoredProcedureNonQuery("sp_NV_DieuPhoiGiaoHang", parameters);
        }
        #endregion

        #region Kiểm tra tồn kho - sp_NV_KiemTraTonKho
        public DataTable KiemTraTonKho(string? maSp = null)
        {
            SqlParameter[]? parameters = maSp != null 
                ? new SqlParameter[] { new SqlParameter("@MASP", maSp) }
                : null;
            return SqlConnectionHelper.ExecuteStoredProcedure("sp_NV_KiemTraTonKho", parameters);
        }
        #endregion

        #region Xem đơn hàng - sp_NV_XemDonHang
        public DataTable XemDonHang(string? maDonHang = null, string? trangThai = null)
        {
            var paramList = new List<SqlParameter>();
            if (!string.IsNullOrEmpty(maDonHang))
                paramList.Add(new SqlParameter("@MADONHANG", maDonHang));
            if (!string.IsNullOrEmpty(trangThai))
                paramList.Add(new SqlParameter("@TRANGTHAI", trangThai));
            
            return SqlConnectionHelper.ExecuteStoredProcedure("sp_NV_XemDonHang", 
                paramList.Count > 0 ? paramList.ToArray() : null);
        }
        #endregion

        #region Doanh thu ngày - sp_NV_DoanhThuNgay
        public DataTable DoanhThuNgay(DateTime ngay)
        {
            SqlParameter[] parameters = { new SqlParameter("@NGAY", ngay.Date) };
            return SqlConnectionHelper.ExecuteStoredProcedure("sp_NV_DoanhThuNgay", parameters);
        }
        #endregion

        #region Thống kê doanh thu tháng - sp_ThongKeDoanhThu_Thang
        public DataTable ThongKeDoanhThuThang(int thang, int nam)
        {
            SqlParameter[] parameters = {
                new SqlParameter("@THANG", thang),
                new SqlParameter("@NAM", nam)
            };
            return SqlConnectionHelper.ExecuteStoredProcedure("sp_ThongKeDoanhThu_Thang", parameters);
        }
        #endregion

        #region Thống kê doanh thu quý - sp_ThongKeDoanhThu_Quy
        public DataTable ThongKeDoanhThuQuy(int quy, int nam)
        {
            SqlParameter[] parameters = {
                new SqlParameter("@QUY", quy),
                new SqlParameter("@NAM", nam)
            };
            return SqlConnectionHelper.ExecuteStoredProcedure("sp_ThongKeDoanhThu_Quy", parameters);
        }
        #endregion

        #region Thống kê doanh thu năm - sp_ThongKeDoanhThu_Nam
        public DataTable ThongKeDoanhThuNam(int nam)
        {
            SqlParameter[] parameters = { new SqlParameter("@NAM", nam) };
            return SqlConnectionHelper.ExecuteStoredProcedure("sp_ThongKeDoanhThu_Nam", parameters);
        }
        #endregion

        #region Top sản phẩm bán chạy - sp_TopSanPhamBanChay
        public DataTable TopSanPhamBanChay(int top = 10, int? nam = null)
        {
            nam ??= DateTime.Now.Year;
            SqlParameter[] parameters = { 
                new SqlParameter("@TOP", top),
                new SqlParameter("@NAM", nam)
            };
            return SqlConnectionHelper.ExecuteStoredProcedure("sp_TopSanPhamBanChay", parameters);
        }
        #endregion
    }
}
