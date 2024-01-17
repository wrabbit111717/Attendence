using Microsoft.EntityFrameworkCore.Migrations;

namespace Attendance.Infrastructure.Migrations
{
    public partial class KeepLegacyUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //UserQuestionnaire table already exists (skip)
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
