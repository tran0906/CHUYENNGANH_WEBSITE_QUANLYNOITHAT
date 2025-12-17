using System;
using System.Collections.Generic;

namespace DOANCHUYENNGANH_WEB_QLNOITHAT.Models;

public partial class CtDonhang
{
    public string Masp { get; set; } = null!;

    public string Madonhang { get; set; } = null!;

    public int? Soluong { get; set; }

    public decimal? Dongia { get; set; }

    public decimal? Thanhtien { get; set; }

    public virtual DonHang MadonhangNavigation { get; set; } = null!;

    public virtual SanPham MaspNavigation { get; set; } = null!;
}
