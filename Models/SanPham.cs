using System;
using System.Collections.Generic;

namespace DOANCHUYENNGANH_WEB_QLNOITHAT.Models;

public partial class SanPham
{
    public string Masp { get; set; } = null!;

    public string? Manhomsp { get; set; }

    public string? Mavl { get; set; }

    public string? Tensp { get; set; }

    public decimal? Giaban { get; set; }

    public int? Soluongton { get; set; }

    public string? Hinhanh { get; set; }

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
