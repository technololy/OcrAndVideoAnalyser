using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace SendImageToOneExpress.Models
{
    public partial class MandateMgtReqContext : DbContext
    {
        public MandateMgtReqContext()
        {
        }

        public MandateMgtReqContext(DbContextOptions<MandateMgtReqContext> options)
            : base(options)
        {
        }

        public virtual DbSet<TblRecords> TblRecords { get; set; }
        public virtual DbSet<TblRecordsDone> TblRecordsDone { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=10.0.41.101;Database=MandateMgtReq;user id=sa;password=tylent;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TblRecords>(entity =>
            {
                entity.ToTable("tbl_Records");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.AccountName).HasMaxLength(255);

                entity.Property(e => e.Bvn)
                    .HasColumnName("BVN")
                    .HasMaxLength(255);

                entity.Property(e => e.DateOpen).HasMaxLength(255);

                entity.Property(e => e.Email).HasMaxLength(255);

                entity.Property(e => e.PhoneNumber).HasMaxLength(255);

                entity.Property(e => e.Restriction).HasMaxLength(255);

                entity.Property(e => e.RestrictionCode).HasMaxLength(255);
            });

            modelBuilder.Entity<TblRecordsDone>(entity =>
            {
                entity.ToTable("tbl_RecordsDone");

                entity.Property(e => e.AccountName).HasMaxLength(255);

                entity.Property(e => e.Bvn)
                    .HasColumnName("BVN")
                    .HasMaxLength(255);

                entity.Property(e => e.DateOpen).HasMaxLength(255);

                entity.Property(e => e.Email).HasMaxLength(255);

                entity.Property(e => e.PhoneNumber).HasMaxLength(255);

                entity.Property(e => e.Restriction).HasMaxLength(255);

                entity.Property(e => e.RestrictionCode).HasMaxLength(255);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
