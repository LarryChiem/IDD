using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AdminUI.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Lock",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    User = table.Column<string>(nullable: true),
                    LastActivity = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lock", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Submissions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Submitted = table.Column<DateTime>(nullable: false),
                    FormType = table.Column<string>(nullable: true),
                    ProviderName = table.Column<string>(nullable: true),
                    ProviderId = table.Column<string>(nullable: true),
                    ClientName = table.Column<string>(nullable: true),
                    ClientPrime = table.Column<string>(nullable: true),
                    ServiceGoal = table.Column<string>(nullable: true),
                    ProgressNotes = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: true),
                    UserActivity = table.Column<string>(nullable: true),
                    RejectionReason = table.Column<string>(nullable: true),
                    LockInfoId = table.Column<int>(nullable: true),
                    UriString = table.Column<string>(nullable: true),
                    Discriminator = table.Column<string>(nullable: false),
                    TotalMiles = table.Column<double>(nullable: true),
                    TotalHours = table.Column<double>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Submissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Submissions_Lock_LockInfoId",
                        column: x => x.LockInfoId,
                        principalTable: "Lock",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MileageEntry",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(nullable: false),
                    Group = table.Column<bool>(nullable: false),
                    Status = table.Column<string>(nullable: true),
                    Miles = table.Column<double>(nullable: false),
                    MileageFormId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MileageEntry", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MileageEntry_Submissions_MileageFormId",
                        column: x => x.MileageFormId,
                        principalTable: "Submissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TimeEntry",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Date = table.Column<DateTime>(nullable: false),
                    Group = table.Column<bool>(nullable: false),
                    Status = table.Column<string>(nullable: true),
                    In = table.Column<DateTime>(nullable: false),
                    Out = table.Column<DateTime>(nullable: false),
                    Hours = table.Column<double>(nullable: false),
                    TimesheetId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TimeEntry", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TimeEntry_Submissions_TimesheetId",
                        column: x => x.TimesheetId,
                        principalTable: "Submissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MileageEntry_MileageFormId",
                table: "MileageEntry",
                column: "MileageFormId");

            migrationBuilder.CreateIndex(
                name: "IX_Submissions_LockInfoId",
                table: "Submissions",
                column: "LockInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_TimeEntry_TimesheetId",
                table: "TimeEntry",
                column: "TimesheetId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MileageEntry");

            migrationBuilder.DropTable(
                name: "TimeEntry");

            migrationBuilder.DropTable(
                name: "Submissions");

            migrationBuilder.DropTable(
                name: "Lock");
        }
    }
}
