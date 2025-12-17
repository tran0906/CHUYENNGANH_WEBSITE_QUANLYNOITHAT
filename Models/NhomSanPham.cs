using System;
using System.Collections.Generic;

namespace DOANCHUYENNGANH_WEB_QLNOITHAT.Models;

public partial class NhomSanPham
{
    public string Manhomsp { get; set; } = null!;

    public string? Tennhomsp { get; set; }

    public string? Mota { get; set; }

    public virtual ICollection<SanPham> SanPhams { get; set; } = new List<SanPham>();
}
