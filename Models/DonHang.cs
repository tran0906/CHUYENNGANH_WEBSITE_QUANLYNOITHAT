using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DOANCHUYENNGANH_WEB_QLNOITHAT.Models;

public partial class DonHang
{
    [Required(ErrorMessage = "Mã đơn hàng là bắt buộc")]
    [StringLength(20, ErrorMessage = "Mã đơn hàng tối đa 20 ký tự")]
    [Display(Name = "Mã đơn hàng")]
    public string Madonhang { get; set; } = null!;

    [Required(ErrorMessage = "Mã khách hàng là bắt buộc")]
    [Display(Name = "Khách hàng")]
    public string Makh { get; set; } = null!;

    [Display(Name = "Người duyệt")]
    public string? Nguoiduyetid { get; set; }

    [StringLength(1000, ErrorMessage = "Ghi chú tối đa 1000 ký tự")]
    [Display(Name = "Ghi chú")]
    public string? Ghichu { get; set; }

    [StringLength(50, ErrorMessage = "Trạng thái tối đa 50 ký tự")]
    [Display(Name = "Trạng thái")]
    public string? Trangthai { get; set; }

    [Required(ErrorMessage = "Ngày đặt là bắt buộc")]
    [Display(Name = "Ngày đặt")]
    [DataType(DataType.DateTime)]
    public DateTime Ngaydat { get; set; }

    public virtual ICollection<CtDonhang> CtDonhangs { get; set; } = new List<CtDonhang>();

    public virtual KhachHang MakhNavigation { get; set; } = null!;

    public virtual ICollection<PhieuXuatKho> PhieuXuatKhos { get; set; } = new List<PhieuXuatKho>();

    public virtual ICollection<ThanhToan> ThanhToans { get; set; } = new List<ThanhToan>();

    public virtual ICollection<VanChuyen> VanChuyens { get; set; } = new List<VanChuyen>();
}
