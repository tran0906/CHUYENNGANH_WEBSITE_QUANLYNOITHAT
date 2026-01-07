using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DOANCHUYENNGANH_WEB_QLNOITHAT.Models;

public partial class User
{
    [Required(ErrorMessage = "Mã người dùng là bắt buộc")]
    [StringLength(20, ErrorMessage = "Mã người dùng tối đa 20 ký tự")]
    [Display(Name = "Mã người dùng")]
    public string UserId { get; set; } = null!;

    [Required(ErrorMessage = "Tên đăng nhập là bắt buộc")]
    [StringLength(50, ErrorMessage = "Tên đăng nhập tối đa 50 ký tự")]
    [Display(Name = "Tên đăng nhập")]
    public string? TenUser { get; set; }

    [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu từ 6-100 ký tự")]
    [Display(Name = "Mật khẩu")]
    public string? MatKhau { get; set; }

    [Required(ErrorMessage = "Vai trò là bắt buộc")]
    [Display(Name = "Vai trò")]
    public string? VaiTro { get; set; }

    [Required(ErrorMessage = "Họ tên là bắt buộc")]
    [StringLength(100, ErrorMessage = "Họ tên tối đa 100 ký tự")]
    [Display(Name = "Họ và tên")]
    public string? HoTen { get; set; }

    [Display(Name = "Ngày tạo")]
    public DateTime? NgayTao { get; set; }

    public virtual ICollection<DonHang> DonHangs { get; set; } = new List<DonHang>();

    public virtual ICollection<PhieuXuatKho> PhieuXuatKhos { get; set; } = new List<PhieuXuatKho>();

    public virtual ICollection<QuanBaSp> QuanBaSps { get; set; } = new List<QuanBaSp>();

    public virtual ICollection<ThanhToan> ThanhToans { get; set; } = new List<ThanhToan>();

    public virtual ICollection<VanChuyen> VanChuyens { get; set; } = new List<VanChuyen>();
}
