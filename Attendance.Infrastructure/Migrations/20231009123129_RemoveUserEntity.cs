using Microsoft.EntityFrameworkCore.Migrations;

namespace Attendance.Infrastructure.Migrations
{
    public partial class RemoveUserEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //keep users table for backward compatibility
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
