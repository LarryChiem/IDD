using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace Appserver.Migrations
{
    public partial class AddGuid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Guid",
                table: "Stagings",
                nullable: true);

            migrationBuilder.Sql(
                "DECLARE @Id int\n" +
                "DECLARE STAGE_CURSOR CURSOR\n" +
                "LOCAL STATIC READ_ONLY FORWARD_ONLY\n" +
                "FOR\n" +
                "SELECT DISTINCT Id FROM Stagings\n" +
                "\n" +
                "OPEN STAGE_CURSOR\n" +
                "FETCH NEXT FROM STAGE_CURSOR INTO @Id\n" +
                "WHILE @@FETCH_STATUS = 0\n" +
                "BEGIN\n" +
                "UPDATE Stagings SET Guid=@Id WHERE Id=@Id\n" +
                "FETCH NEXT FROM STAGE_CURSOR INTO @Id\n" +
                "END\n" +
                "CLOSE STAGE_CURSOR\n" +
                "DEALLOCATE STAGE_CURSOR\n"
                );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Guid",
                table: "Stagings");
        }
    }
}
