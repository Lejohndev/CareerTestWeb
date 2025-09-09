using System.Collections.Generic;
using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;
using CareerTestWeb.Models;
namespace CareerTestWeb.Data
{


        public class MBTIDbContext : DbContext
        {
            public MBTIDbContext(DbContextOptions<MBTIDbContext> options) : base(options)
            {
            }

            public DbSet<CauHoi> CauHoi { get; set; }
            public DbSet<DapAnChuan> DapAnChuans { get; set; }
            public DbSet<Users> Users { get; set; }
            public DbSet<TraLoi> TraLoi { get; set; }
            public DbSet<KetQuaMBTI> KetQuaMBTIs { get; set; }
            public DbSet<TinhCachMBTI> TinhCachMBTIs { get; set; }
            public DbSet<NgheNghiep> NgheNghieps { get; set; }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                // Cấu hình khóa chính và khóa ngoại nếu cần (EF Core tự động phát hiện từ SQL)
                modelBuilder.Entity<CauHoi>().HasKey(ch => new { ch.IDQues, ch.IDAns });
                modelBuilder.Entity<DapAnChuan>().HasKey(dc => dc.IDQues);
                modelBuilder.Entity<TraLoi>().HasKey(tl => new { tl.UserID, tl.IDQues });
                modelBuilder.Entity<NgheNghiep>().HasKey(nn => new { nn.MBTI, nn.TenNghe });
                modelBuilder.Entity<KetQuaMBTI>().HasNoKey();
                modelBuilder.Entity<TinhCachMBTI>().HasNoKey();
                modelBuilder.Entity<Users>().HasNoKey();
        }
        }

        // Định nghĩa các lớp model
        public class CauHoi
        {
            public int STT { get; set; }
            public int IDQues { get; set; }
            public string NameQues { get; set; }
            public int QuestionType { get; set; }
            public int IDAns { get; set; }
            public string NameAns { get; set; }
            public int AnswerType { get; set; }
            public int GroupID { get; set; }
        public List<TraLoi> TraLoi { get; internal set; }
    }

        public class DapAnChuan
        {
            public int IDQues { get; set; }
            public int IDAns { get; set; }
            public int AnswerType { get; set; }
            public int GroupID { get; set; }
        }

        public class Users
        {
            public int UserID { get; set; }
            public string Ten { get; set; }
            public int? Tuoi { get; set; } // Nullable nếu không bắt buộc
            public string Email { get; set; }
            public DateTime NgayTest { get; set; }
        }

        public class TraLoi
        {
            public int UserID { get; set; }
            public int IDQues { get; set; }
            public int IDAns { get; set; }
        }

        public class KetQuaMBTI
        {
            public int UserID { get; set; }
            public string MBTI { get; set; }
            public float? DiemE_I { get; set; } // Nullable nếu không bắt buộc
            public float? DiemS_N { get; set; }
            public float? DiemT_F { get; set; }
            public float? DiemJ_P { get; set; }
        }

        public class TinhCachMBTI
        {
            public string MBTI { get; set; }
            public string TenNhom { get; set; }
            public string MoTa { get; set; }
            public string UuDiem { get; set; }
            public string NhuocDiem { get; set; }
        }

        public class NgheNghiep
        {
            public string MBTI { get; set; }
            public string TenNghe { get; set; }
            public int ThuNhapTB { get; set; }
            public int CoHoiViecLam { get; set; }
            public int MucDoPhuHop { get; set; }
            public int CoHoiThangTien { get; set; }
            public int DoOnDinh { get; set; }
        }
    }

