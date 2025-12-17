using System;
using System.Collections.Generic;

namespace DOANCHUYENNGANH_WEB_QLNOITHAT.Models;

public partial class VatLieu
{
    public string Mavl { get; set; } = null!;

    public string? Tenvl { get; set; }

    public string? Mauvl { get; set; }

    public int? Soluong { get; set; }

    public string? Motavl { get; set; }

    public virtual ICollection<SanPham> SanPhams { get; set; } = new List<SanPham>();
}
