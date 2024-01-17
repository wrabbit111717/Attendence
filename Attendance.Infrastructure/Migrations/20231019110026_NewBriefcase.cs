using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Attendance.Infrastructure.Migrations
{
    public partial class NewBriefcase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Briefcase",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(maxLength: 450, nullable: false),
                    CompanyRepresentativeName = table.Column<string>(nullable: false),
                    VesselId = table.Column<int>(nullable: false),
                    InspectionTypeId = table.Column<int>(nullable: false),
                    InspectionSourceId = table.Column<int>(nullable: false),
                    InspectionSourceName = table.Column<string>(nullable: false),
                    InspectionCode = table.Column<string>(maxLength: 64, nullable: false),
                    VettingDate = table.Column<DateTime>(nullable: false),
                    InspectorName = table.Column<string>(maxLength: 50, nullable: true),
                    PortId = table.Column<int>(nullable: false),
                    PortName = table.Column<string>(nullable: false),
                    PortCountry = table.Column<string>(nullable: false),
                    Comments = table.Column<string>(nullable: true),
                    Sent = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Briefcase", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Briefcase_InspectionTypes_InspectionTypeId",
                        column: x => x.InspectionTypeId,
                        principalTable: "InspectionTypes",
                        principalColumn: "InspectionTypeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Briefcase_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Briefcase_Vessel_VesselId",
                        column: x => x.VesselId,
                        principalTable: "Vessel",
                        principalColumn: "VesselId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BriefcaseQuestionnaires",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BriefcaseId = table.Column<int>(nullable: false),
                    QId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BriefcaseQuestionnaires", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BriefcaseQuestionnaires_Briefcase_BriefcaseId",
                        column: x => x.BriefcaseId,
                        principalTable: "Briefcase",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BriefcaseQuestionnaires_VIQInfo_QId",
                        column: x => x.QId,
                        principalTable: "VIQInfo",
                        principalColumn: "QId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Briefcase_InspectionTypeId",
                table: "Briefcase",
                column: "InspectionTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_Briefcase_UserId",
                table: "Briefcase",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Briefcase_VesselId",
                table: "Briefcase",
                column: "VesselId");

            migrationBuilder.CreateIndex(
                name: "IX_BriefcaseQuestionnaires_BriefcaseId",
                table: "BriefcaseQuestionnaires",
                column: "BriefcaseId");

            migrationBuilder.CreateIndex(
                name: "IX_BriefcaseQuestionnaires_QId_BriefcaseId",
                table: "BriefcaseQuestionnaires",
                columns: new[] { "QId", "BriefcaseId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BriefcaseQuestionnaires");

            migrationBuilder.DropTable(
                name: "Briefcase");
        }
    }
}
