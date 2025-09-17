using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CareerTestWeb.Models;

public partial class CareerDbContext : DbContext
{
    public CareerDbContext()
    {
    }

    public CareerDbContext(DbContextOptions<CareerDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<CauHoi> CauHoi { get; set; }

    public virtual DbSet<DapAnChuan> DapAnChuan { get; set; }

    public virtual DbSet<KetQuaMbti> KetQuaMBTI { get; set; }

    public virtual DbSet<NgheNghiep> NgheNghiep { get; set; }

    public virtual DbSet<TinhCachMbti> TinhCachMBTI { get; set; }

    public virtual DbSet<TraLoi> TraLoi { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=DESKTOP-B3STGKR;Database=MBTI;Trusted_Connection=True;TrustServerCertificate=True;");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CauHoi>(entity =>
        {
            entity.HasKey(e => new { e.Idques, e.Idans }).HasName("PK__CauHoi__5418E56632ABD78F");
        });

        modelBuilder.Entity<DapAnChuan>(entity =>
        {
            entity.HasKey(e => e.Idques).HasName("PK__DapAnChu__8D2AFC9F90897C28");

            entity.Property(e => e.Idques).ValueGeneratedNever();

            entity.HasOne(d => d.CauHoi).WithMany(p => p.DapAnChuans).HasConstraintName("FK__DapAnChuan__267ABA7A");
        });

        modelBuilder.Entity<KetQuaMbti>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__KetQuaMB__1788CCAC9A8960EF");

            entity.Property(e => e.UserId).ValueGeneratedNever();
            entity.Property(e => e.Mbti).IsFixedLength();

            entity.HasOne(d => d.User).WithOne(p => p.KetQuaMbti)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__KetQuaMBT__UserI__300424B4");
        });

        modelBuilder.Entity<NgheNghiep>(entity =>
        {
            entity.HasKey(e => new { e.Mbti, e.TenNghe }).HasName("PK__NgheNghi__12D2B24ED9D5F53F");

            entity.Property(e => e.Mbti).IsFixedLength();

            entity.HasOne(d => d.MbtiNavigation).WithMany(p => p.NgheNghieps)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__NgheNghiep__MBTI__34C8D9D1");
        });

        modelBuilder.Entity<TinhCachMbti>(entity =>
        {
            entity.HasKey(e => e.Mbti).HasName("PK__TinhCach__60617C6F980F598E");

            entity.Property(e => e.Mbti).IsFixedLength();
        });

        modelBuilder.Entity<TraLoi>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.Idques }).HasName("PK__TraLoi__FF5A6365D702B047");

            entity.HasOne(d => d.User).WithMany(p => p.TraLois)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__TraLoi__UserID__2C3393D0");

            entity.HasOne(d => d.CauHoi).WithMany(p => p.TraLois).HasConstraintName("FK__TraLoi__2D27B809");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCAC6188A923");

            entity.Property(e => e.NgayTest).HasDefaultValueSql("(getdate())");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
