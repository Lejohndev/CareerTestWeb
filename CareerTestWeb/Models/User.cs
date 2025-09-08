using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace CareerTestWeb.Models;

public partial class User
{
    [Key]
    [Column("UserID")]
    public int UserId { get; set; }

    [StringLength(100)]
    public string? Ten { get; set; }

    public int? Tuoi { get; set; }

    [StringLength(100)]
    public string? Email { get; set; }

    public DateOnly? NgayTest { get; set; }

    [InverseProperty("User")]
    public virtual KetQuaMbti? KetQuaMbti { get; set; }

    [InverseProperty("User")]
    public virtual ICollection<TraLoi> TraLois { get; set; } = new List<TraLoi>();
}
