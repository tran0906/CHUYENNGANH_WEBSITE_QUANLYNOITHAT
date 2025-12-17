using System;
using System.Collections.Generic;

namespace DOANCHUYENNGANH_WEB_QLNOITHAT.Models;

public partial class ThanhToan
{
    public string Mathanhtoan { get; set; } = null!;

    public string Madonhang { get; set; } = null!;

    public string Userid { get; set; } = null!;

    public decimal? Sotien { get; set; }

    public string? Phuongthuc { get; set; }

    public string? Manvduyet { get; set; }

    public DateTime? Ngaythanhtoan { get; set; }

    public virtual DonHang MadonhangNavigation { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
