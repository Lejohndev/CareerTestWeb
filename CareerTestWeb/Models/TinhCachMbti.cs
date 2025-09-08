using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CareerTestWeb.Models;

[Table("TinhCachMBTI")]
public partial class TinhCachMbti
{
    [Key]
    [Column("MBTI")]
    [StringLength(4)]
    [Unicode(false)]
    public string Mbti { get; set; } = null!;

    [StringLength(100)]
    public string? TenNhom { get; set; }

    public string? MoTa { get; set; }

    public string? UuDiem { get; set; }

    public string? NhuocDiem { get; set; }

    [InverseProperty("MbtiNavigation")]
    public virtual ICollection<NgheNghiep> NgheNghieps { get; set; } = new List<NgheNghiep>();
}
