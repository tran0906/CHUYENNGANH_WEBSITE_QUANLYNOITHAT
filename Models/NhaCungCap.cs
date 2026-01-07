using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DOANCHUYENNGANH_WEB_QLNOITHAT.Models;

public partial class NhaCungCap
{
    [Required(ErrorMessage = "Mã nhà cung cấp là bắt buộc")]
    [StringLength(20, ErrorMessage = "Mã NCC tối đa 20 ký tự")]
    [Display(Name = "Mã NCC")]
    public string Mancc { get; set; } = null!;

    [Required(ErrorMessage = "Tên nhà cung cấp là bắt buộc")]
    [StringLength(200, ErrorMessage = "Tên NCC tối đa 200 ký tự")]
    [Display(Name = "Tên NCC")]
    public string? Tenncc { get; set; }

    [StringLength(500, ErrorMessage = "Địa chỉ tối đa 500 ký tự")]
    [Display(Name = "Địa chỉ")]
    public string? Diachincc { get; set; }

    [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
    [StringLength(15, ErrorMessage = "SĐT tối đa 15 ký tự")]
    [Display(Name = "Số điện thoại")]
    public string? Sdtncc { get; set; }

    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    [StringLength(100, ErrorMessage = "Email tối đa 100 ký tự")]
    [Display(Name = "Email")]
    public string? Emailncc { get; set; }

    [StringLength(500, ErrorMessage = "Sản phẩm cung cấp tối đa 500 ký tự")]
    [Display(Name = "Sản phẩm cung cấp")]
    public string? Sanpham { get; set; }

    [Range(0, 999999999, ErrorMessage = "Giá nhập phải từ 0 đến 999,999,999")]
    [Display(Name = "Giá nhập")]
    public decimal? Gianhap { get; set; }

    [Display(Name = "Ngày cập nhật")]
    public DateTime? Ngaycapnhatsp { get; set; }

    public virtual ICollection<Cungcap> Cungcaps { get; set; } = new List<Cungcap>();
}
