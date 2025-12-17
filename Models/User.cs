using System;
using System.Collections.Generic;

namespace DOANCHUYENNGANH_WEB_QLNOITHAT.Models;

public partial class User
{
    public string UserId { get; set; } = null!;

    public string? TenUser { get; set; }

    public string? MatKhau { get; set; }

    public string? VaiTro { get; set; }

    public string? HoTen { get; set; }

    public DateTime? NgayTao { get; set; }

    public virtual ICollection<DonHang> DonHangs { get; set; } = new List<DonHang>();

    public virtual ICollection<PhieuXuatKho> PhieuXuatKhos { get; set; } = new List<PhieuXuatKho>();

    public virtual ICollection<QuanBaSp> QuanBaSps { get; set; } = new List<QuanBaSp>();

    public virtual ICollection<ThanhToan> ThanhToans { get; set; } = new List<ThanhToan>();

    public virtual ICollection<VanChuyen> VanChuyens { get; set; } = new List<VanChuyen>();
}
