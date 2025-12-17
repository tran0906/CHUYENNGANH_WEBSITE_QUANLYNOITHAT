using System;
using System.Collections.Generic;

namespace DOANCHUYENNGANH_WEB_QLNOITHAT.Models;

public partial class Cungcap
{
    public string Mancc { get; set; } = null!;

    public string Masp { get; set; } = null!;

    public int? Soluongsp { get; set; }

    public virtual NhaCungCap ManccNavigation { get; set; } = null!;

    public virtual SanPham MaspNavigation { get; set; } = null!;
}
