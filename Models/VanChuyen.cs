using System;
using System.Collections.Generic;

namespace DOANCHUYENNGANH_WEB_QLNOITHAT.Models;

public partial class VanChuyen
{
    public string Mavandon { get; set; } = null!;

    public string Userid { get; set; } = null!;

    public string Madonhang { get; set; } = null!;

    public string? Donvivanchuyen { get; set; }

    public string? Trangthaigiao { get; set; }

    public string? Manvdieuphoi { get; set; }

    public virtual DonHang MadonhangNavigation { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
