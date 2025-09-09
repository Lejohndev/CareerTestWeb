using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace CareerTestWeb.Data
{
    public class MBTIDbContext : DbContext
    {
        public MBTIDbContext(DbContextOptions<MBTIDbContext> options) : base(options)
        {
        }

        public DbSet<CauHoi> CauHois { get; set; }
        public DbSet<DapAnChuan> DapAnChuans { get; set; }
        public DbSet<Users> Users { get; set; }
        public DbSet<TraLoi> TraLois { get; set; }
        public DbSet<KetQuaMBTI> KetQuaMBTIs { get; set; }
        public DbSet<TinhCachMBTI> TinhCachMBTIs { get; set; }
        public DbSet<NgheNghiep> NgheNghieps { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CauHoi>().ToTable("CauHoi");
            modelBuilder.Entity<DapAnChuan>().ToTable("DapAnChuan");
            modelBuilder.Entity<Users>().ToTable("Users");
            modelBuilder.Entity<TraLoi>().ToTable("TraLoi");
            modelBuilder.Entity<KetQuaMBTI>().ToTable("KetQuaMBTI");
            modelBuilder.Entity<TinhCachMBTI>().ToTable("TinhCachMBTI");
            modelBuilder.Entity<NgheNghiep>().ToTable("NgheNghiep");

            modelBuilder.Entity<CauHoi>().HasKey(ch => new { ch.IDQues, ch.IDAns });
            modelBuilder.Entity<DapAnChuan>().HasKey(dc => dc.IDQues);
            modelBuilder.Entity<TraLoi>().HasKey(tl => new { tl.UserID, tl.IDQues });
            modelBuilder.Entity<NgheNghiep>().HasKey(nn => new { nn.MBTI, nn.TenNghe });
            modelBuilder.Entity<KetQuaMBTI>().HasKey(kq => kq.UserID);
            modelBuilder.Entity<TinhCachMBTI>().HasKey(tc => tc.MBTI);

            modelBuilder.Entity<DapAnChuan>()
                .HasOne<DapAnChuan>()
                .WithMany()
                .HasForeignKey(dc => new { dc.IDQues, dc.IDAns })
                .HasPrincipalKey(ch => new { ch.IDQues, ch.IDAns });

            modelBuilder.Entity<TraLoi>()
                .HasOne<TraLoi>()
                .WithMany()
                .HasForeignKey(tl => new { tl.IDQues, tl.IDAns })
                .HasPrincipalKey(ch => new { ch.IDQues, ch.IDAns });

            modelBuilder.Entity<TraLoi>()
                .HasOne<Users>()
                .WithMany()
                .HasForeignKey(tl => tl.UserID);

            modelBuilder.Entity<KetQuaMBTI>()
                .HasOne<Users>()
                .WithMany()
                .HasForeignKey(kq => kq.UserID);

            modelBuilder.Entity<NgheNghiep>()
                .HasOne<TinhCachMBTI>()
                .WithMany()
                .HasForeignKey(nn => nn.MBTI);
        }
    }

    public class CauHoi
    {
        public int STT { get; set; }
        public int IDQues { get; set; }
        public string? NameQues { get; set; }
        public int QuestionType { get; set; }
        public int IDAns { get; set; }
        public string? NameAns { get; set; }
        public int AnswerType { get; set; }
        public int GroupID { get; set; }
    }

    public class DapAnChuan
    {
        public int IDQues { get; set; }
        public int IDAns { get; set; }
        public int AnswerType { get; set; }
        public int GroupID { get; set; }
        public virtual CauHoi CauHoi { get; set; }
    }

    public class Users
    {
        [Key] // Đánh dấu UserID là khóa chính
        public int UserID { get; set; }
        public string? Ten { get; set; }
        public int? Tuoi { get; set; }
        public string? Email { get; set; }
        public DateTime NgayTest { get; set; }
    }

    public class TraLoi
    {
        public int UserID { get; set; }
        public int IDQues { get; set; }
        public int IDAns { get; set; }
        public virtual CauHoi CauHoi { get; set; }
        public virtual Users User { get; set; }
    }

    public class KetQuaMBTI
    {
        public int UserID { get; set; }
        public string? MBTI { get; set; }
        public float? DiemE_I { get; set; }
        public float? DiemS_N { get; set; }
        public float? DiemT_F { get; set; }
        public float? DiemJ_P { get; set; }
        public virtual Users User { get; set; }
    }

    public class TinhCachMBTI
    {
        public string? MBTI { get; set; }
        public string? TenNhom { get; set; }
        public string? MoTa { get; set; }
        public string? UuDiem { get; set; }
        public string? NhuocDiem { get; set; }
    }

    public class NgheNghiep
    {
        public string? MBTI { get; set; }
        public string? TenNghe { get; set; }
        public int ThuNhapTB { get; set; }
        public int CoHoiViecLam { get; set; }
        public int MucDoPhuHop { get; set; }
        public int CoHoiThangTien { get; set; }
        public int DoOnDinh { get; set; }
        public virtual TinhCachMBTI TinhCachMBTI { get; set; }
    }
}