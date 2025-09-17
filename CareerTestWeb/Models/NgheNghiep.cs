using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CareerTestWeb.Models;

[PrimaryKey("Mbti", "TenNghe")]
[Table("NgheNghiep")]
public partial class NgheNghiep
{
    [Key]
    [Column("MBTI")]
    [StringLength(4)]
    [Unicode(false)]
    public string Mbti { get; set; } = null!;

    [Key]
    [StringLength(200)]
    public string TenNghe { get; set; } = null!;

    [Column("ThuNhapTB")]
    public int? ThuNhapTb { get; set; }

    public int? CoHoiViecLam { get; set; }

    public int? MucDoPhuHop { get; set; }

    public int? CoHoiThangTien { get; set; }

    public int? DoOnDinh { get; set; }

    [ForeignKey("Mbti")]
    [InverseProperty("NgheNghieps")]
    public virtual TinhCachMbti MbtiNavigation { get; set; } = null!;
}
