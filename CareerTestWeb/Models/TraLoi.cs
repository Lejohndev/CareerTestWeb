using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CareerTestWeb.Models;

[PrimaryKey("UserId", "Idques")]
[Table("TraLoi")]
public partial class TraLoi
{
    [Key]
    [Column("UserID")]
    public int UserId { get; set; }

    [Key]
    [Column("IDQues")]
    public int Idques { get; set; }

    [Column("IDAns")]
    public int? Idans { get; set; }

    [ForeignKey("Idques, Idans")]
    [InverseProperty("TraLois")]
    public virtual CauHoi? CauHoi { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("TraLois")]
    public virtual User User { get; set; } = null!;
}
