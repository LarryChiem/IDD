using Microsoft.EntityFrameworkCore.Migrations;

namespace Appserver.Migrations
{
    public partial class FormType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "formType",
                table: "Stagings",
                type: "nvarchar(50)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "formType",
                table: "Stagings");
        }
    }
}
