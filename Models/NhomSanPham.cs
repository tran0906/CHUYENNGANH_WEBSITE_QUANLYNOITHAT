using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DOANCHUYENNGANH_WEB_QLNOITHAT.Models;

public partial class NhomSanPham
{
    [Required(ErrorMessage = "Mã nhóm sản phẩm là bắt buộc")]
    [StringLength(20, ErrorMessage = "Mã nhóm SP tối đa 20 ký tự")]
    [Display(Name = "Mã nhóm SP")]
    public string Manhomsp { get; set; } = null!;

    [Required(ErrorMessage = "Tên nhóm sản phẩm là bắt buộc")]
    [StringLength(100, ErrorMessage = "Tên nhóm SP tối đa 100 ký tự")]
    [Display(Name = "Tên nhóm SP")]
    public string? Tennhomsp { get; set; }

    [StringLength(500, ErrorMessage = "Mô tả tối đa 500 ký tự")]
    [Display(Name = "Mô tả")]
    public string? Mota { get; set; }

    public virtual ICollection<SanPham> SanPhams { get; set; } = new List<SanPham>();
}
