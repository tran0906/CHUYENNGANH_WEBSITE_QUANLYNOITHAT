using System;
using System.Collections.Generic;

namespace DOANCHUYENNGANH_WEB_QLNOITHAT.Models;

public partial class PhieuXuatKho
{
    public string Maphieuxuat { get; set; } = null!;

    public string Userid { get; set; } = null!;

    public string Madonhang { get; set; } = null!;

    public DateTime? Ngayxuat { get; set; }

    public string? Manvduyet { get; set; }

    public virtual DonHang MadonhangNavigation { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
