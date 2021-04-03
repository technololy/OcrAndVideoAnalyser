using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AcctOpeningImageValidationAPI.Migrations
{
    public partial class addNewDBsAndOCRUsageIDToDBs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OCRUsageId",
                table: "ScannedIDCardDetail",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "FaceID",
                table: "FacialValidations",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OcrUsageId",
                table: "FacialValidations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ImageScanneds",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ImageURL = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OcrUsageId = table.Column<int>(type: "int", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateModified = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImageScanneds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImageScanneds_OCRUsages_OcrUsageId",
                        column: x => x.OcrUsageId,
                        principalTable: "OCRUsages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SimilarFacesRecord",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FaceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    PersistedFaceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Confidence = table.Column<double>(type: "float", nullable: false),
                    OCRUsageId = table.Column<int>(type: "int", nullable: false),
                    DateInserted = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SimilarFacesRecord", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SimilarFacesRecord_OCRUsages_OCRUsageId",
                        column: x => x.OCRUsageId,
                        principalTable: "OCRUsages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ScannedIDCardDetail_OCRUsageId",
                table: "ScannedIDCardDetail",
                column: "OCRUsageId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FacialValidations_OcrUsageId",
                table: "FacialValidations",
                column: "OcrUsageId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ImageScanneds_OcrUsageId",
                table: "ImageScanneds",
                column: "OcrUsageId");

            migrationBuilder.CreateIndex(
                name: "IX_SimilarFacesRecord_OCRUsageId",
                table: "SimilarFacesRecord",
                column: "OCRUsageId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_FacialValidations_OCRUsages_OcrUsageId",
                table: "FacialValidations",
                column: "OcrUsageId",
                principalTable: "OCRUsages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ScannedIDCardDetail_OCRUsages_OCRUsageId",
                table: "ScannedIDCardDetail",
                column: "OCRUsageId",
                principalTable: "OCRUsages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FacialValidations_OCRUsages_OcrUsageId",
                table: "FacialValidations");

            migrationBuilder.DropForeignKey(
                name: "FK_ScannedIDCardDetail_OCRUsages_OCRUsageId",
                table: "ScannedIDCardDetail");

            migrationBuilder.DropTable(
                name: "ImageScanneds");

            migrationBuilder.DropTable(
                name: "SimilarFacesRecord");

            migrationBuilder.DropIndex(
                name: "IX_ScannedIDCardDetail_OCRUsageId",
                table: "ScannedIDCardDetail");

            migrationBuilder.DropIndex(
                name: "IX_FacialValidations_OcrUsageId",
                table: "FacialValidations");

            migrationBuilder.DropColumn(
                name: "OCRUsageId",
                table: "ScannedIDCardDetail");

            migrationBuilder.DropColumn(
                name: "FaceID",
                table: "FacialValidations");

            migrationBuilder.DropColumn(
                name: "OcrUsageId",
                table: "FacialValidations");
        }
    }
}
