using System;
using System.Collections.Generic;

namespace DOANCHUYENNGANH_WEB_QLNOITHAT.Models;

public partial class VwDoanhThuTongHop
{
    public string Madonhang { get; set; } = null!;

    public decimal? Sotien { get; set; }

    public DateTime? Ngaythanhtoan { get; set; }

    public int? Nam { get; set; }

    public int? Thang { get; set; }

    public int? Quy { get; set; }
}
