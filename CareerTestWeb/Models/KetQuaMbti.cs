using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CareerTestWeb.Models;

[Table("KetQuaMBTI")]
public partial class KetQuaMbti
{
    [Key]
    [Column("UserID")]
    public int UserId { get; set; }

    [Column("MBTI")]
    [StringLength(4)]
    [Unicode(false)]
    public string? Mbti { get; set; }

    [Column("DiemE_I")]
    public double? DiemEI { get; set; }

    [Column("DiemS_N")]
    public double? DiemSN { get; set; }

    [Column("DiemT_F")]
    public double? DiemTF { get; set; }

    [Column("DiemJ_P")]
    public double? DiemJP { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("KetQuaMbti")]
    public virtual User User { get; set; } = null!;
}
