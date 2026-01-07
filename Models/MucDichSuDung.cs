using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DOANCHUYENNGANH_WEB_QLNOITHAT.Models;

public partial class MucDichSuDung
{
    [Required(ErrorMessage = "Mã mục đích sử dụng là bắt buộc")]
    [StringLength(20, ErrorMessage = "Mã MĐSD tối đa 20 ký tự")]
    [Display(Name = "Mã MĐSD")]
    public string Mamdsd { get; set; } = null!;

    [Required(ErrorMessage = "Tên mục đích sử dụng là bắt buộc")]
    [StringLength(100, ErrorMessage = "Tên MĐSD tối đa 100 ký tự")]
    [Display(Name = "Tên mục đích")]
    public string? Tenmdsd { get; set; }

    [StringLength(500, ErrorMessage = "Mô tả tối đa 500 ký tự")]
    [Display(Name = "Mô tả")]
    public string? Motamdsd { get; set; }

    public virtual ICollection<SanPham> Masps { get; set; } = new List<SanPham>();
}
