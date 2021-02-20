using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AcctOpeningImageValidationAPI.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppruvResponses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BVN = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StatusOfRequest = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateInserted = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())"),
                    JsonResponse = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppruvResponses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FacialValidations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BVN = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Accessories = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FacialHair = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Hair = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Emotion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Smile = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Age = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HeadPose = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Occlusion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateInserted = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FacialValidations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OCRResponses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BVN = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: true),
                    JsonResponse = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateInserted = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OCRResponses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ScannedIDCardDetail",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MiddleName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IDType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IDNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IssueDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateOfBirth = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExpiryDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FormerIDNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IDClass = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BVN = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BloodGroup = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Height = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IssuingAuthority = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Occupation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NextOfKin = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FirstIssueState = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Delim = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateInserted = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScannedIDCardDetail", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppruvResponses");

            migrationBuilder.DropTable(
                name: "FacialValidations");

            migrationBuilder.DropTable(
                name: "OCRResponses");

            migrationBuilder.DropTable(
                name: "ScannedIDCardDetail");
        }
    }
}
