using Microsoft.EntityFrameworkCore.Migrations;

namespace AdminUI.Migrations
{
    public partial class Remove_Submission_Index_Generation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "DECLARE @MAX int\n" +
                "SELECT @MAX=ISNULL(Max(Id),0) FROM (SELECT Id FROM dbo.Submissions UNION SELECT Id FROM dbo.Stagings ) AS subquery;\n" +
                "DBCC CHECKIDENT ('[Submissions]', RESEED,@MAX)\n" +
                "DBCC CHECKIDENT ('[Stagings]', RESEED,@MAX)\n"
                );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                "DECLARE @MAX int\n" +
                "SELECT @MAX=ISNULL(Max(Id),0) FROM dbo.Submissions;\n" +
                "DBCC CHECKIDENT ('[Submissions]', RESEED,@MAX);\n" +
                "SELECT @MAX=ISNULL(Max(Id),0) FROM dbo.Stagings;\n" +
                "DBCC CHECKIDENT ('[Stagings]', RESEED,@MAX);\n"
                );

        }
    }
}
