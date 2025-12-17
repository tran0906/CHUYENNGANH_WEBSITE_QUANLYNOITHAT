using System;
using System.Collections.Generic;

namespace DOANCHUYENNGANH_WEB_QLNOITHAT.Models;

public partial class KhachHang
{
    public string Makh { get; set; } = null!;

    public string? Hotenkh { get; set; }

    public string? Diachikh { get; set; }

    public string? Sdtkh { get; set; }

    public string? Matkhau { get; set; }

    public virtual ICollection<DonHang> DonHangs { get; set; } = new List<DonHang>();
}
