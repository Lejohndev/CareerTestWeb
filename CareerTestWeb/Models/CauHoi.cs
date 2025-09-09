using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CareerTestWeb.Models;

[PrimaryKey("Idques", "Idans")]
[Table("CauHoi")]
public partial class CauHoi
{
    [Column("STT")]
    public int? Stt { get; set; }

    [Key]
    [Column("IDQues")]
    public int Idques { get; set; }

    [StringLength(500)]
    public string? NameQues { get; set; }

    public int? QuestionType { get; set; }

    [Key]
    [Column("IDAns")]
    public int Idans { get; set; }

    [StringLength(500)]
    public string? NameAns { get; set; }

    public int? AnswerType { get; set; }

    [Column("GroupID")]
    public int? GroupId { get; set; }

    [InverseProperty("CauHoi")]
    public virtual ICollection<DapAnChuan> DapAnChuans { get; set; } = new List<DapAnChuan>();

    [InverseProperty("CauHoi")]
    public virtual ICollection<TraLoi> TraLois { get; set; } = new List<TraLoi>();
    [NotMapped]
    public List<TraLoi> TraLoi { get; set; }
}
