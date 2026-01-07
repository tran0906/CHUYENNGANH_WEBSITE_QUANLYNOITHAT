using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DOANCHUYENNGANH_WEB_QLNOITHAT.Models;

public partial class KhachHang
{
    [Required(ErrorMessage = "Mã khách hàng là bắt buộc")]
    [StringLength(20, ErrorMessage = "Mã khách hàng tối đa 20 ký tự")]
    [Display(Name = "Mã KH")]
    public string Makh { get; set; } = null!;

    [Required(ErrorMessage = "Họ tên là bắt buộc")]
    [StringLength(100, ErrorMessage = "Họ tên tối đa 100 ký tự")]
    [Display(Name = "Họ tên")]
    public string? Hotenkh { get; set; }

    [Required(ErrorMessage = "Địa chỉ là bắt buộc")]
    [StringLength(500, ErrorMessage = "Địa chỉ tối đa 500 ký tự")]
    [Display(Name = "Địa chỉ")]
    public string? Diachikh { get; set; }

    [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
    [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
    [StringLength(15, MinimumLength = 10, ErrorMessage = "SĐT từ 10-15 ký tự")]
    [Display(Name = "Số điện thoại")]
    public string? Sdtkh { get; set; }

    [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu từ 6-100 ký tự")]
    [Display(Name = "Mật khẩu")]
    public string? Matkhau { get; set; }

    public virtual ICollection<DonHang> DonHangs { get; set; } = new List<DonHang>();
}
