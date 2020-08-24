using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ReadTextFromImageConsole.Models
{
    public partial class SterlingOnlineBankingContext : DbContext
    {
        public SterlingOnlineBankingContext()
        {
        }

        public SterlingOnlineBankingContext(DbContextOptions<SterlingOnlineBankingContext> options)
            : base(options)
        {
        }

        public virtual DbSet<CamuCorpData> CamuCorpData { get; set; }
        public virtual DbSet<CamuDocument> CamuDocument { get; set; }
        public virtual DbSet<CamuRepush> CamuRepush { get; set; }
        public virtual DbSet<Camudatafield> Camudatafield { get; set; }
        public virtual DbSet<Camudatafield1> Camudatafield1 { get; set; }
        public virtual DbSet<CardRequest> CardRequest { get; set; }
        public virtual DbSet<Ereference> Ereference { get; set; }
        public virtual DbSet<EreferenceResponse> EreferenceResponse { get; set; }
        public virtual DbSet<IbscardRequestResponse> IbscardRequestResponse { get; set; }
        public virtual DbSet<OnebankMobile> OnebankMobile { get; set; }
        public virtual DbSet<Product> Product { get; set; }
        public virtual DbSet<ProductCategory> ProductCategory { get; set; }
        public virtual DbSet<SterlingBranchNew> SterlingBranchNew { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<UserNuban> UserNuban { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=10.0.41.101;Database=SterlingOnlineBanking;user id=sa;password=tylent;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CamuCorpData>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Chequerequired).HasColumnName("chequerequired");
            });

            modelBuilder.Entity<CamuDocument>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Folio).HasColumnName("folio");

                entity.Property(e => e.Idno).HasColumnName("idno");

                entity.Property(e => e.Idtype).HasColumnName("idtype");

                entity.Property(e => e.Iexpirydate).HasColumnName("iexpirydate");

                entity.Property(e => e.Issuedate).HasColumnName("issuedate");

                entity.Property(e => e.MeansOfIdbase64).HasColumnName("MeansOfIDBase64");

                entity.Property(e => e.PhotoIdbase64).HasColumnName("PhotoIDBase64");

                entity.Property(e => e.Urlreference1Base64).HasColumnName("URLReference1Base64");

                entity.Property(e => e.Urlreference2Base64).HasColumnName("URLReference2Base64");
            });

            modelBuilder.Entity<CamuRepush>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.AccountNumber).IsUnicode(false);
            });

            modelBuilder.Entity<Camudatafield>(entity =>
            {
                entity.ToTable("CAMUDatafield");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Bvn).HasColumnName("BVN");

                entity.Property(e => e.DateFacialImageIsChecked).HasColumnType("datetime");

                entity.Property(e => e.DateIdentificationImageIsChecked).HasColumnType("datetime");

                entity.Property(e => e.Dob).HasColumnName("DOB");

                entity.Property(e => e.FacialImageCheckResponse).HasMaxLength(50);

                entity.Property(e => e.FacialImageCheckResponseJson).HasColumnName("FacialImageCheckResponseJSON");

                entity.Property(e => e.Folio).HasColumnName("folio");

                entity.Property(e => e.IdentificationImageCheckResponse).HasMaxLength(50);

                entity.Property(e => e.IdentificationImageCheckResponseJson).HasColumnName("IdentificationImageCheckResponseJSON");

                entity.Property(e => e.Idno).HasColumnName("idno");

                entity.Property(e => e.Idtype).HasColumnName("idtype");

                entity.Property(e => e.Iexpirydate).HasColumnName("iexpirydate");

                entity.Property(e => e.Issuedate).HasColumnName("issuedate");

                entity.Property(e => e.Urlmandate).HasColumnName("URLMandate");

                entity.Property(e => e.UrlmeansOfId).HasColumnName("URLMeansOfID");

                entity.Property(e => e.Urlother).HasColumnName("URLOther");

                entity.Property(e => e.UrlphotoId).HasColumnName("URLPhotoID");

                entity.Property(e => e.Urlreference1).HasColumnName("URLReference1");

                entity.Property(e => e.Urlreference2).HasColumnName("URLReference2");
            });

            modelBuilder.Entity<Camudatafield1>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("CAMUDatafield_1");

                entity.Property(e => e.Bvn).HasColumnName("BVN");

                entity.Property(e => e.DateFacialImageIsChecked).HasColumnType("datetime");

                entity.Property(e => e.DateIdentificationImageIsChecked).HasColumnType("datetime");

                entity.Property(e => e.Dob).HasColumnName("DOB");

                entity.Property(e => e.FacialImageCheckResponse).HasMaxLength(50);

                entity.Property(e => e.FacialImageCheckResponseJson).HasColumnName("FacialImageCheckResponseJSON");

                entity.Property(e => e.Folio).HasColumnName("folio");

                entity.Property(e => e.IdentificationImageCheckResponse).HasMaxLength(50);

                entity.Property(e => e.IdentificationImageCheckResponseJson).HasColumnName("IdentificationImageCheckResponseJSON");

                entity.Property(e => e.Idno).HasColumnName("idno");

                entity.Property(e => e.Idtype).HasColumnName("idtype");

                entity.Property(e => e.Iexpirydate).HasColumnName("iexpirydate");

                entity.Property(e => e.Issuedate).HasColumnName("issuedate");

                entity.Property(e => e.Urlmandate).HasColumnName("URLMandate");

                entity.Property(e => e.UrlmeansOfId).HasColumnName("URLMeansOfID");

                entity.Property(e => e.Urlother).HasColumnName("URLOther");

                entity.Property(e => e.UrlphotoId).HasColumnName("URLPhotoID");

                entity.Property(e => e.Urlreference1).HasColumnName("URLReference1");

                entity.Property(e => e.Urlreference2).HasColumnName("URLReference2");
            });

            modelBuilder.Entity<CardRequest>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();
            });

            modelBuilder.Entity<Ereference>(entity =>
            {
                entity.ToTable("EReference");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.CustomerId).HasColumnName("Customer_ID");

                entity.Property(e => e.DateAccountOpened).HasColumnName("Date_Account_Opened");

                entity.Property(e => e.ProductType).HasColumnName("Product_Type");

                entity.Property(e => e.RefereeAccountNumber).HasColumnName("Referee_Account_Number");

                entity.Property(e => e.RefereeBank).HasColumnName("Referee_Bank");

                entity.Property(e => e.RefereeName).HasColumnName("Referee_Name");
            });

            modelBuilder.Entity<EreferenceResponse>(entity =>
            {
                entity.ToTable("EReferenceResponse");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.CustomerId).HasColumnName("Customer_ID");

                entity.Property(e => e.DateAccountOpened).HasColumnName("Date_Account_Opened");

                entity.Property(e => e.ProductType).HasColumnName("Product_Type");

                entity.Property(e => e.RefereeAccountNumber).HasColumnName("Referee_Account_Number");

                entity.Property(e => e.RefereeBank).HasColumnName("Referee_Bank");

                entity.Property(e => e.RefereeName).HasColumnName("Referee_Name");
            });

            modelBuilder.Entity<IbscardRequestResponse>(entity =>
            {
                entity.ToTable("IBSCardRequestResponse");

                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.ReferenceId).HasColumnName("ReferenceID");
            });

            modelBuilder.Entity<OnebankMobile>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("ONEBANK_MOBILE");

                entity.Property(e => e.AccountNumber).HasColumnName("ACCOUNT_NUMBER");

                entity.Property(e => e.AccountOfficer).HasColumnName("ACCOUNT_OFFICER");

                entity.Property(e => e.AccountTitle1).HasColumnName("ACCOUNT_TITLE_1");

                entity.Property(e => e.Bvn).HasColumnName("BVN");

                entity.Property(e => e.CoCode).HasColumnName("CO_CODE");

                entity.Property(e => e.ContactDate).HasColumnName("CONTACT_DATE");

                entity.Property(e => e.CustomerCode).HasColumnName("CUSTOMER_CODE");

                entity.Property(e => e.Email1).HasColumnName("EMAIL_1");

                entity.Property(e => e.Mobile).HasColumnName("MOBILE");

                entity.Property(e => e.Phone1).HasColumnName("PHONE_1");

                entity.Property(e => e.Product).HasColumnName("PRODUCT");

                entity.Property(e => e.Sms1).HasColumnName("SMS_1");

                entity.Property(e => e.StartDate).HasColumnName("START_DATE");

                entity.Property(e => e.WorkingBalance).HasColumnName("WORKING_BALANCE");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            });

            modelBuilder.Entity<ProductCategory>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.CategoryId).HasColumnName("CategoryID");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.Bvn).HasColumnName("BVN");

                entity.Property(e => e.BvnFirstname).HasColumnName("BVN_Firstname");

                entity.Property(e => e.BvnLastname).HasColumnName("BVN_Lastname");

                entity.Property(e => e.Cerpac).HasColumnName("cerpac");

                entity.Property(e => e.ClientId).HasColumnName("ClientID");

                entity.Property(e => e.Dao).HasColumnName("DAO");

                entity.Property(e => e.MainBase64Image)
                    .HasColumnName("Main_Base64Image")
                    .IsUnicode(false);

                entity.Property(e => e.MainBvn)
                    .HasColumnName("Main_BVN")
                    .IsUnicode(false);

                entity.Property(e => e.MainDateOfBirth)
                    .HasColumnName("Main_DateOfBirth")
                    .IsUnicode(false);

                entity.Property(e => e.MainEmail)
                    .HasColumnName("Main_Email")
                    .IsUnicode(false);

                entity.Property(e => e.MainEnrollmentBank)
                    .HasColumnName("Main_EnrollmentBank")
                    .IsUnicode(false);

                entity.Property(e => e.MainEnrollmentBranch)
                    .HasColumnName("Main_EnrollmentBranch")
                    .IsUnicode(false);

                entity.Property(e => e.MainFirstName)
                    .HasColumnName("Main_FirstName")
                    .IsUnicode(false);

                entity.Property(e => e.MainGender)
                    .HasColumnName("Main_Gender")
                    .IsUnicode(false);

                entity.Property(e => e.MainLastName)
                    .HasColumnName("Main_LastName")
                    .IsUnicode(false);

                entity.Property(e => e.MainLevelOfAccount)
                    .HasColumnName("Main_LevelOfAccount")
                    .IsUnicode(false);

                entity.Property(e => e.MainLgaOfOrigin)
                    .HasColumnName("Main_LgaOfOrigin")
                    .IsUnicode(false);

                entity.Property(e => e.MainLgaOfResidence)
                    .HasColumnName("Main_LgaOfResidence")
                    .IsUnicode(false);

                entity.Property(e => e.MainMaritalStatus)
                    .HasColumnName("Main_MaritalStatus")
                    .IsUnicode(false);

                entity.Property(e => e.MainMiddleName)
                    .HasColumnName("Main_MiddleName")
                    .IsUnicode(false);

                entity.Property(e => e.MainNameOnCard)
                    .HasColumnName("Main_NameOnCard")
                    .IsUnicode(false);

                entity.Property(e => e.MainNationality)
                    .HasColumnName("Main_Nationality")
                    .IsUnicode(false);

                entity.Property(e => e.MainNin)
                    .HasColumnName("Main_NIN")
                    .IsUnicode(false);

                entity.Property(e => e.MainPhoneNumber)
                    .HasColumnName("Main_PhoneNumber")
                    .IsUnicode(false);

                entity.Property(e => e.MainPhoneNumber2)
                    .HasColumnName("Main_PhoneNumber2")
                    .IsUnicode(false);

                entity.Property(e => e.MainRegistrationDate)
                    .HasColumnName("Main_RegistrationDate")
                    .IsUnicode(false);

                entity.Property(e => e.MainResidentialAddress)
                    .HasColumnName("Main_ResidentialAddress")
                    .IsUnicode(false);

                entity.Property(e => e.MainResponseCode)
                    .HasColumnName("Main_ResponseCode")
                    .IsUnicode(false);

                entity.Property(e => e.MainResponseDesc)
                    .HasColumnName("Main_ResponseDesc")
                    .IsUnicode(false);

                entity.Property(e => e.MainStateOfOrigin)
                    .HasColumnName("Main_StateOfOrigin")
                    .IsUnicode(false);

                entity.Property(e => e.MainStateOfResidence)
                    .HasColumnName("Main_StateOfResidence")
                    .IsUnicode(false);

                entity.Property(e => e.MainTitle)
                    .HasColumnName("Main_Title")
                    .IsUnicode(false);

                entity.Property(e => e.MainWatchListed)
                    .HasColumnName("Main_WatchListed")
                    .IsUnicode(false);

                entity.Property(e => e.Nokaddress).HasColumnName("NOKAddress");

                entity.Property(e => e.NokfirstName).HasColumnName("NOKFirstName");

                entity.Property(e => e.NokfullName).HasColumnName("NOKFullName");

                entity.Property(e => e.NoklastName).HasColumnName("NOKLastName");

                entity.Property(e => e.NokmiddleName).HasColumnName("NOKMiddleName");

                entity.Property(e => e.NokphoneNumber).HasColumnName("NOKPhoneNumber");

                entity.Property(e => e.Nokrelationship).HasColumnName("NOKRelationship");

                entity.Property(e => e.SignatoryAddress).HasColumnName("signatoryAddress");

                entity.Property(e => e.SignatoryPhoneNumber).HasColumnName("signatoryPhoneNumber");

                entity.Property(e => e.Tin).HasColumnName("tin");
            });

            modelBuilder.Entity<UserNuban>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.BvnFirstname).HasColumnName("BVN_Firstname");

                entity.Property(e => e.BvnLastname).HasColumnName("BVN_Lastname");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
