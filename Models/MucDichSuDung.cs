using System;
using System.Collections.Generic;

namespace DOANCHUYENNGANH_WEB_QLNOITHAT.Models;

public partial class MucDichSuDung
{
    public string Mamdsd { get; set; } = null!;

    public string? Tenmdsd { get; set; }

    public string? Motamdsd { get; set; }

    public virtual ICollection<SanPham> Masps { get; set; } = new List<SanPham>();
}
