using Microsoft.EntityFrameworkCore.Migrations;

namespace AdminUI.Migrations
{
    public partial class MileageEntry : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PurposeOfTrip",
                table: "MileageEntry",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PurposeOfTrip",
                table: "MileageEntry");
        }
    }
}
