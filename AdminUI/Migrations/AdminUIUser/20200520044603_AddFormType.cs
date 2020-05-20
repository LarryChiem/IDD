using Microsoft.EntityFrameworkCore.Migrations;

namespace AdminUI.Migrations.AdminUIUser
{
    public partial class AddFormType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Locked",
                table: "Filter");

            migrationBuilder.AddColumn<string>(
                name: "FormType",
                table: "Filter",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FormType",
                table: "Filter");

            migrationBuilder.AddColumn<bool>(
                name: "Locked",
                table: "Filter",
                type: "bit",
                nullable: true);
        }
    }
}
