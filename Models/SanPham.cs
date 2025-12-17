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

    public virtual ICollection<CtDonhang> CtDonhangs { get; set; } = new List<CtDonhang>();

    public virtual ICollection<Cungcap> Cungcaps { get; set; } = new List<Cungcap>();

    public virtual NhomSanPham? ManhomspNavigation { get; set; }

    public virtual VatLieu? MavlNavigation { get; set; }

    public virtual ICollection<QuanBaSp> Madotgiamgia { get; set; } = new List<QuanBaSp>();

    public virtual ICollection<MucDichSuDung> Mamdsds { get; set; } = new List<MucDichSuDung>();
}
