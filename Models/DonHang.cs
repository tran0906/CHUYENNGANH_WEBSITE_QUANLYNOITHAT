using System;
using System.Collections.Generic;

namespace DOANCHUYENNGANH_WEB_QLNOITHAT.Models;

public partial class DonHang
{
    public string Madonhang { get; set; } = null!;

    public string Makh { get; set; } = null!;

    public string? Nguoiduyetid { get; set; }

    public string? Ghichu { get; set; }

    public string? Trangthai { get; set; }

    public DateTime Ngaydat { get; set; }

    public virtual ICollection<CtDonhang> CtDonhangs { get; set; } = new List<CtDonhang>();

    public virtual KhachHang MakhNavigation { get; set; } = null!;

    public virtual ICollection<PhieuXuatKho> PhieuXuatKhos { get; set; } = new List<PhieuXuatKho>();

    public virtual ICollection<ThanhToan> ThanhToans { get; set; } = new List<ThanhToan>();

    public virtual ICollection<VanChuyen> VanChuyens { get; set; } = new List<VanChuyen>();
}
