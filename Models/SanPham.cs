using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DOANCHUYENNGANH_WEB_QLNOITHAT.Models;

public partial class SanPham
{
    [Required(ErrorMessage = "Mã sản phẩm là bắt buộc")]
    [StringLength(20, ErrorMessage = "Mã sản phẩm tối đa 20 ký tự")]
    [Display(Name = "Mã sản phẩm")]
    public string Masp { get; set; } = null!;

    [Display(Name = "Nhóm sản phẩm")]
    public string? Manhomsp { get; set; }

    [Display(Name = "Vật liệu")]
    public string? Mavl { get; set; }

    [Required(ErrorMessage = "Tên sản phẩm là bắt buộc")]
    [StringLength(200, ErrorMessage = "Tên sản phẩm tối đa 200 ký tự")]
    [Display(Name = "Tên sản phẩm")]
    public string? Tensp { get; set; }

    [Range(0, 999999999, ErrorMessage = "Giá bán phải từ 0 đến 999,999,999")]
    [Display(Name = "Giá bán (VNĐ)")]
    public decimal? Giaban { get; set; }

    [Range(0, 99999, ErrorMessage = "Số lượng tồn phải từ 0 đến 99,999")]
    [Display(Name = "Số lượng tồn")]
    public int? Soluongton { get; set; }

    [Display(Name = "Hình ảnh")]
    public string? Hinhanh { get; set; }

    [StringLength(2000, ErrorMessage = "Mô tả tối đa 2000 ký tự")]
    [Display(Name = "Mô tả")]
    public string? Mota { get; set; }

    // Property ảo cho giá gốc (không lưu database) - dùng cho quảng bá giảm giá
    public decimal? GiaGoc { get; set; }
    
    // % giảm giá từ đợt quảng bá
    private int? _phanTramGiam;
    public int PhanTramGiam 
    { 
        get => _phanTramGiam ?? (DangGiamGia && GiaGoc > 0 
            ? (int)Math.Round((1 - (Giaban ?? 0) / GiaGoc.Value) * 100) 
            : 0);
        set => _phanTramGiam = value;
    }
    
    // Kiểm tra sản phẩm có đang giảm giá không
    public bool DangGiamGia => GiaGoc.HasValue && GiaGoc > Giaban;

    public virtual ICollection<CtDonhang> CtDonhangs { get; set; } = new List<CtDonhang>();

    public virtual ICollection<Cungcap> Cungcaps { get; set; } = new List<Cungcap>();

    public virtual NhomSanPham? ManhomspNavigation { get; set; }

    public virtual VatLieu? MavlNavigation { get; set; }

    public virtual ICollection<QuanBaSp> Madotgiamgia { get; set; } = new List<QuanBaSp>();

    public virtual ICollection<MucDichSuDung> Mamdsds { get; set; } = new List<MucDichSuDung>();
}
