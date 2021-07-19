using Microsoft.EntityFrameworkCore.Migrations;

namespace AcctOpeningImageValidationAPI.Migrations
{
    public partial class OcrUsageIdRemoved : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FacialValidations_OCRUsages_OCRUsageId",
                table: "FacialValidations");

            migrationBuilder.DropIndex(
                name: "IX_FacialValidations_OCRUsageId",
                table: "FacialValidations");

            migrationBuilder.AlterColumn<int>(
                name: "OCRUsageId",
                table: "FacialValidations",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_FacialValidations_OCRUsageId",
                table: "FacialValidations",
                column: "OCRUsageId",
                unique: true,
                filter: "[OCRUsageId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_FacialValidations_OCRUsages_OCRUsageId",
                table: "FacialValidations",
                column: "OCRUsageId",
                principalTable: "OCRUsages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FacialValidations_OCRUsages_OCRUsageId",
                table: "FacialValidations");

            migrationBuilder.DropIndex(
                name: "IX_FacialValidations_OCRUsageId",
                table: "FacialValidations");

            migrationBuilder.AlterColumn<int>(
                name: "OCRUsageId",
                table: "FacialValidations",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FacialValidations_OCRUsageId",
                table: "FacialValidations",
                column: "OCRUsageId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_FacialValidations_OCRUsages_OCRUsageId",
                table: "FacialValidations",
                column: "OCRUsageId",
                principalTable: "OCRUsages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
