using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DOANCHUYENNGANH_WEB_QLNOITHAT.Models;

public partial class Cungcap
{
    [Required(ErrorMessage = "Mã nhà cung cấp là bắt buộc")]
    [Display(Name = "Nhà cung cấp")]
    public string Mancc { get; set; } = null!;

    [Required(ErrorMessage = "Mã sản phẩm là bắt buộc")]
    [Display(Name = "Sản phẩm")]
    public string Masp { get; set; } = null!;

    [Range(1, 99999, ErrorMessage = "Số lượng phải từ 1 đến 99,999")]
    [Display(Name = "Số lượng")]
    public int? Soluongsp { get; set; }

    public virtual NhaCungCap ManccNavigation { get; set; } = null!;

    public virtual SanPham MaspNavigation { get; set; } = null!;
}
