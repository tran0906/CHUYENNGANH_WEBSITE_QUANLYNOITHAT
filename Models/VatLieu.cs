using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DOANCHUYENNGANH_WEB_QLNOITHAT.Models;

public partial class VatLieu
{
    [Required(ErrorMessage = "Mã vật liệu là bắt buộc")]
    [StringLength(20, ErrorMessage = "Mã vật liệu tối đa 20 ký tự")]
    [Display(Name = "Mã vật liệu")]
    public string Mavl { get; set; } = null!;

    [Required(ErrorMessage = "Tên vật liệu là bắt buộc")]
    [StringLength(100, ErrorMessage = "Tên vật liệu tối đa 100 ký tự")]
    [Display(Name = "Tên vật liệu")]
    public string? Tenvl { get; set; }

    [StringLength(50, ErrorMessage = "Màu vật liệu tối đa 50 ký tự")]
    [Display(Name = "Màu")]
    public string? Mauvl { get; set; }

    [Range(0, 99999, ErrorMessage = "Số lượng phải từ 0 đến 99,999")]
    [Display(Name = "Số lượng")]
    public int? Soluong { get; set; }

    [StringLength(500, ErrorMessage = "Mô tả tối đa 500 ký tự")]
    [Display(Name = "Mô tả")]
    public string? Motavl { get; set; }

    public virtual ICollection<SanPham> SanPhams { get; set; } = new List<SanPham>();
}
