using Microsoft.EntityFrameworkCore.Migrations;

namespace AdminUI.Migrations.PayPeriod
{
    public partial class AddCurrent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Current",
                table: "PayPeriods",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Current",
                table: "PayPeriods");
        }
    }
}
