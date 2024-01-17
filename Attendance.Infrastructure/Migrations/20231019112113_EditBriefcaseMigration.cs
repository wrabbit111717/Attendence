using Microsoft.EntityFrameworkCore.Migrations;

namespace Attendance.Infrastructure.Migrations
{
    public partial class EditBriefcaseMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InspectionSourceName",
                table: "Briefcase");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InspectionSourceName",
                table: "Briefcase",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
