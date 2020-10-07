using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TCStudentRecordManagement.Migrations
{
    public partial class NoticesAndRelationships1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_students_users_UserID",
                table: "students");

            migrationBuilder.DropIndex(
                name: "FK_Student_User",
                table: "students");

            migrationBuilder.CreateTable(
                name: "notices",
                columns: table => new
                {
                    NoticeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CohortID = table.Column<int>(type: "int", nullable: false),
                    StaffID = table.Column<int>(type: "int", nullable: false),
                    ValidFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Markdown = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HTML = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_notices", x => x.NoticeID);
                    table.ForeignKey(
                        name: "FK_Notice_Cohort",
                        column: x => x.CohortID,
                        principalTable: "cohorts",
                        principalColumn: "CohortID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Notice_User",
                        column: x => x.StaffID,
                        principalTable: "users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "notices",
                columns: new[] { "NoticeID", "CohortID", "HTML", "Markdown", "StaffID", "ValidFrom" },
                values: new object[] { -1, -1, null, null, 6, new DateTime(2020, 6, 25, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.InsertData(
                table: "notices",
                columns: new[] { "NoticeID", "CohortID", "HTML", "Markdown", "StaffID", "ValidFrom" },
                values: new object[] { -2, -2, null, null, 6, new DateTime(2020, 6, 25, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.CreateIndex(
                name: "FK_Student_User",
                table: "students",
                column: "UserID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "FK_Notice_Cohort",
                table: "notices",
                column: "CohortID");

            migrationBuilder.CreateIndex(
                name: "FK_Notice_User",
                table: "notices",
                column: "StaffID");

            migrationBuilder.AddForeignKey(
                name: "FK_Student_User",
                table: "students",
                column: "UserID",
                principalTable: "users",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Student_User",
                table: "students");

            migrationBuilder.DropTable(
                name: "notices");

            migrationBuilder.DropIndex(
                name: "FK_Student_User",
                table: "students");

            migrationBuilder.CreateIndex(
                name: "FK_Student_User",
                table: "students",
                column: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_students_users_UserID",
                table: "students",
                column: "UserID",
                principalTable: "users",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
