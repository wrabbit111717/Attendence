using Microsoft.EntityFrameworkCore.Migrations;

namespace Attendance.Infrastructure.Migrations
{
    public partial class AddBriefcaseInspectionSourceCode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Briefcase_UserId",
                table: "Briefcase");

            migrationBuilder.AddColumn<string>(
                name: "InspectionSourceCode",
                table: "Briefcase",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Briefcase_UserId_InspectionCode",
                table: "Briefcase",
                columns: new[] { "UserId", "InspectionCode" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Briefcase_UserId_InspectionCode",
                table: "Briefcase");

            migrationBuilder.DropColumn(
                name: "InspectionSourceCode",
                table: "Briefcase");

            migrationBuilder.CreateIndex(
                name: "IX_Briefcase_UserId",
                table: "Briefcase",
                column: "UserId");
        }
    }
}
