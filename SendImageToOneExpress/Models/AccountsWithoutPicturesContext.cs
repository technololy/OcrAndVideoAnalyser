using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace SendImageToOneExpress.Models
{
    public partial class AccountsWithoutPicturesContext : DbContext
    {
        public AccountsWithoutPicturesContext()
        {
        }

        public AccountsWithoutPicturesContext(DbContextOptions<AccountsWithoutPicturesContext> options)
            : base(options)
        {
        }

        public virtual DbSet<MandatePic> MandatePic { get; set; }
        public virtual DbSet<MandatePictureMgt> MandatePictureMgt { get; set; }
        public virtual DbSet<MandatePictureMgtDone> MandatePictureMgtDone { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=10.0.41.101;Database=AccountsWithoutPictures;user id=sa;password=tylent;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MandatePic>(entity =>
            {
                entity.HasKey(e => e.Nuban);

                entity.ToTable("MANDATE_PIC");

                entity.Property(e => e.Nuban).HasMaxLength(50);

                entity.Property(e => e.AccountName).HasMaxLength(50);

                entity.Property(e => e.Bvn)
                    .HasColumnName("BVN")
                    .HasMaxLength(50);

                entity.Property(e => e.DateOpen).HasColumnType("date");

                entity.Property(e => e.Email).HasMaxLength(50);

                entity.Property(e => e.PhoneNumber).HasMaxLength(50);

                entity.Property(e => e.Restriction).HasMaxLength(50);

                entity.Property(e => e.WorkingBalance).HasMaxLength(50);
            });

            modelBuilder.Entity<MandatePictureMgt>(entity =>
            {
                entity.HasKey(e => e.Nuban);

                entity.ToTable("MANDATE_PICTURE_MGT");

                entity.Property(e => e.Nuban).HasMaxLength(50);

                entity.Property(e => e.AccountName).HasMaxLength(50);

                entity.Property(e => e.Bvn)
                    .HasColumnName("BVN")
                    .HasMaxLength(50);

                entity.Property(e => e.DateOpen).HasColumnType("date");

                entity.Property(e => e.Email).HasMaxLength(50);

                entity.Property(e => e.PhoneNumber).HasMaxLength(50);

                entity.Property(e => e.Restriction).HasMaxLength(50);

                entity.Property(e => e.WorkingBalance).HasMaxLength(50);
            });

            modelBuilder.Entity<MandatePictureMgtDone>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("MANDATE_PICTURE_MGT_DONE");

                entity.Property(e => e.AccountName).HasMaxLength(50);

                entity.Property(e => e.Bvn)
                    .HasColumnName("BVN")
                    .HasMaxLength(50);

                entity.Property(e => e.DateOpen).HasColumnType("date");

                entity.Property(e => e.Email).HasMaxLength(50);

                entity.Property(e => e.Nuban)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.PhoneNumber).HasMaxLength(50);

                entity.Property(e => e.Restriction).HasMaxLength(50);

                entity.Property(e => e.WorkingBalance).HasMaxLength(50);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
