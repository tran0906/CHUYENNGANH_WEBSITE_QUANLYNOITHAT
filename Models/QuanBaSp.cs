using System;
using System.Collections.Generic;

namespace DOANCHUYENNGANH_WEB_QLNOITHAT.Models;

public partial class QuanBaSp
{
    public string Madotgiamgia { get; set; } = null!;

    public string? Userid { get; set; }

    public DateTime? Ngaybatdau { get; set; }

    public DateTime? Ngayketthuc { get; set; }

    public string? Manvchon { get; set; }

    public int? Phantramgiam { get; set; }

    public virtual User? User { get; set; }

    public virtual ICollection<SanPham> Masps { get; set; } = new List<SanPham>();
}
