using System;
using System.Collections.Generic;

namespace DOANCHUYENNGANH_WEB_QLNOITHAT.Models;

public partial class NhaCungCap
{
    public string Mancc { get; set; } = null!;

    public string? Tenncc { get; set; }

    public string? Diachincc { get; set; }

    public string? Sdtncc { get; set; }

    public string? Emailncc { get; set; }

    public string? Sanpham { get; set; }

    public decimal? Gianhap { get; set; }

    public DateTime? Ngaycapnhatsp { get; set; }

    public virtual ICollection<Cungcap> Cungcaps { get; set; } = new List<Cungcap>();
}
