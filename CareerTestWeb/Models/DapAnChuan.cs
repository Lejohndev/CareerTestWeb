using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CareerTestWeb.Models;

[Table("DapAnChuan")]
public partial class DapAnChuan
{
    [Key]
    [Column("IDQues")]
    public int Idques { get; set; }

    [Column("IDAns")]
    public int? Idans { get; set; }

    public int? AnswerType { get; set; }

    [Column("GroupID")]
    public int? GroupId { get; set; }

    [ForeignKey("Idques, Idans")]
    [InverseProperty("DapAnChuans")]
    public virtual CauHoi? CauHoi { get; set; }
}
