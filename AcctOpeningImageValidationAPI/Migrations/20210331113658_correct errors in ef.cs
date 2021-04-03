using Microsoft.EntityFrameworkCore.Migrations;

namespace AcctOpeningImageValidationAPI.Migrations
{
    public partial class correcterrorsinef : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FacialValidations_OCRUsages_OcrUsageId",
                table: "FacialValidations");

            migrationBuilder.RenameColumn(
                name: "OcrUsageId",
                table: "FacialValidations",
                newName: "OCRUsageId");

            migrationBuilder.RenameIndex(
                name: "IX_FacialValidations_OcrUsageId",
                table: "FacialValidations",
                newName: "IX_FacialValidations_OCRUsageId");

            migrationBuilder.AddForeignKey(
                name: "FK_FacialValidations_OCRUsages_OCRUsageId",
                table: "FacialValidations",
                column: "OCRUsageId",
                principalTable: "OCRUsages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FacialValidations_OCRUsages_OCRUsageId",
                table: "FacialValidations");

            migrationBuilder.RenameColumn(
                name: "OCRUsageId",
                table: "FacialValidations",
                newName: "OcrUsageId");

            migrationBuilder.RenameIndex(
                name: "IX_FacialValidations_OCRUsageId",
                table: "FacialValidations",
                newName: "IX_FacialValidations_OcrUsageId");

            migrationBuilder.AddForeignKey(
                name: "FK_FacialValidations_OCRUsages_OcrUsageId",
                table: "FacialValidations",
                column: "OcrUsageId",
                principalTable: "OCRUsages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
