﻿// <auto-generated />
using System;
using AcctOpeningImageValidationAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AcctOpeningImageValidationAPI.Migrations
{
    [DbContext(typeof(SterlingOnebankIDCardsContext))]
    partial class SterlingOnebankIDCardsContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.3")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("AcctOpeningImageValidationAPI.Models.AppruvResponse", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("BVN")
                        .HasMaxLength(11)
                        .HasColumnType("nvarchar(11)");

                    b.Property<DateTime?>("DateInserted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("JsonResponse")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("StatusOfRequest")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("AppruvResponses");
                });

            modelBuilder.Entity("AcctOpeningImageValidationAPI.Models.FacialValidation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Accessories")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Age")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("BVN")
                        .HasMaxLength(11)
                        .HasColumnType("nvarchar(11)");

                    b.Property<DateTime?>("DateInserted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Emotion")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("FaceID")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("FacialHair")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Gender")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Hair")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("HeadPose")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("OCRUsageId")
                        .HasColumnType("int");

                    b.Property<string>("Occlusion")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Smile")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("OCRUsageId")
                        .IsUnique()
                        .HasFilter("[OCRUsageId] IS NOT NULL");

                    b.ToTable("FacialValidations");
                });

            modelBuilder.Entity("AcctOpeningImageValidationAPI.Models.ImagesScanned", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateModified")
                        .HasColumnType("datetime2");

                    b.Property<string>("ImageURL")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsEnabled")
                        .HasColumnType("bit");

                    b.Property<int>("OcrUsageId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("OcrUsageId");

                    b.ToTable("ImageScanneds");
                });

            modelBuilder.Entity("AcctOpeningImageValidationAPI.Models.OCRResponse", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("BVN")
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<DateTime?>("DateInserted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<string>("JsonResponse")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("OCRResponses");
                });

            modelBuilder.Entity("AcctOpeningImageValidationAPI.Models.OCRUsage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Count")
                        .HasColumnType("int");

                    b.Property<DateTime>("DateCreated")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("DateModified")
                        .HasColumnType("datetime2");

                    b.Property<string>("EmailAddress")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsEnabled")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.ToTable("OCRUsages");
                });

            modelBuilder.Entity("AcctOpeningImageValidationAPI.Models.RequestLogs", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("DateCreated")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<DateTime>("DateModified")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FileName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("IsEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("RequestLog");
                });

            modelBuilder.Entity("AcctOpeningImageValidationAPI.Models.ScannedIDCardDetails", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Address")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("BVN")
                        .HasMaxLength(11)
                        .HasColumnType("nvarchar(11)");

                    b.Property<string>("BloodGroup")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("DateInserted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<string>("DateOfBirth")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Delim")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("DocumentType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ExpiryDate")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FirstIssueState")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FirstName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FormerIDNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FullName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Gender")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Height")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("IDClass")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("IDNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("IDType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("IssueDate")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("IssuingAuthority")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("LastName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("MiddleName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("NextOfKin")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("OCRUsageId")
                        .HasColumnType("int");

                    b.Property<string>("Occupation")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("OCRUsageId")
                        .IsUnique();

                    b.ToTable("ScannedIDCardDetail");
                });

            modelBuilder.Entity("AcctOpeningImageValidationAPI.Models.SimilarFace", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<double>("Confidence")
                        .HasColumnType("float");

                    b.Property<DateTime?>("DateInserted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime")
                        .HasDefaultValueSql("(getdate())");

                    b.Property<Guid?>("FaceId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("OCRUsageId")
                        .HasColumnType("int");

                    b.Property<Guid?>("PersistedFaceId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("OCRUsageId")
                        .IsUnique();

                    b.ToTable("SimilarFacesRecord");
                });

            modelBuilder.Entity("AcctOpeningImageValidationAPI.Models.FacialValidation", b =>
                {
                    b.HasOne("AcctOpeningImageValidationAPI.Models.OCRUsage", null)
                        .WithOne("facialValidation")
                        .HasForeignKey("AcctOpeningImageValidationAPI.Models.FacialValidation", "OCRUsageId");
                });

            modelBuilder.Entity("AcctOpeningImageValidationAPI.Models.ImagesScanned", b =>
                {
                    b.HasOne("AcctOpeningImageValidationAPI.Models.OCRUsage", "oCRUsage")
                        .WithMany("ImageScanned")
                        .HasForeignKey("OcrUsageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("oCRUsage");
                });

            modelBuilder.Entity("AcctOpeningImageValidationAPI.Models.ScannedIDCardDetails", b =>
                {
                    b.HasOne("AcctOpeningImageValidationAPI.Models.OCRUsage", "OCRUsage")
                        .WithOne("scannedIDCardDetails")
                        .HasForeignKey("AcctOpeningImageValidationAPI.Models.ScannedIDCardDetails", "OCRUsageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("OCRUsage");
                });

            modelBuilder.Entity("AcctOpeningImageValidationAPI.Models.SimilarFace", b =>
                {
                    b.HasOne("AcctOpeningImageValidationAPI.Models.OCRUsage", "OCRUsage")
                        .WithOne("SimilarFaces")
                        .HasForeignKey("AcctOpeningImageValidationAPI.Models.SimilarFace", "OCRUsageId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("OCRUsage");
                });

            modelBuilder.Entity("AcctOpeningImageValidationAPI.Models.OCRUsage", b =>
                {
                    b.Navigation("facialValidation");

                    b.Navigation("ImageScanned");

                    b.Navigation("scannedIDCardDetails");

                    b.Navigation("SimilarFaces");
                });
#pragma warning restore 612, 618
        }
    }
}
