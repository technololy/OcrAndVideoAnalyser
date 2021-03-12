using Microsoft.EntityFrameworkCore.Migrations;

namespace AcctOpeningImageValidationAPI.Migrations
{
    public partial class newColumnToScannedDetailedTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DocumentType",
                table: "ScannedIDCardDetail",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DocumentType",
                table: "ScannedIDCardDetail");
        }
    }
}
