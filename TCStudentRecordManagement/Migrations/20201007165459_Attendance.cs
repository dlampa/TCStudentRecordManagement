using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TCStudentRecordManagement.Migrations
{
    public partial class Attendance : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "attendance_states",
                columns: table => new
                {
                    StateID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "varchar(50)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_attendance_states", x => x.StateID);
                });

            migrationBuilder.CreateTable(
                name: "attendance",
                columns: table => new
                {
                    RecordID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentID = table.Column<int>(type: "int", nullable: false),
                    AttendanceStateID = table.Column<int>(type: "int", nullable: false),
                    StaffID = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_attendance", x => x.RecordID);
                    table.ForeignKey(
                        name: "FK_Attendance_AttendanceState",
                        column: x => x.AttendanceStateID,
                        principalTable: "attendance_states",
                        principalColumn: "StateID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_attendance_users_StaffID",
                        column: x => x.StaffID,
                        principalTable: "users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Attendance_Student",
                        column: x => x.StudentID,
                        principalTable: "students",
                        principalColumn: "StudentID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "attendance_states",
                columns: new[] { "StateID", "Description" },
                values: new object[] { -1, "Present" });

            migrationBuilder.InsertData(
                table: "attendance_states",
                columns: new[] { "StateID", "Description" },
                values: new object[] { -2, "Absent with notice" });

            migrationBuilder.InsertData(
                table: "attendance_states",
                columns: new[] { "StateID", "Description" },
                values: new object[] { -3, "Absent without notice" });

            migrationBuilder.InsertData(
                table: "attendance",
                columns: new[] { "RecordID", "AttendanceStateID", "Date", "StaffID", "StudentID" },
                values: new object[] { -1, -1, new DateTime(2020, 6, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), 6, -1 });

            migrationBuilder.InsertData(
                table: "attendance",
                columns: new[] { "RecordID", "AttendanceStateID", "Date", "StaffID", "StudentID" },
                values: new object[] { -2, -2, new DateTime(2020, 6, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), 6, -4 });

            migrationBuilder.CreateIndex(
                name: "FK_Attendance_AttendanceState",
                table: "attendance",
                column: "AttendanceStateID");

            migrationBuilder.CreateIndex(
                name: "FK_Attendance_User",
                table: "attendance",
                column: "StaffID");

            migrationBuilder.CreateIndex(
                name: "FK_Attendance_Student",
                table: "attendance",
                column: "StudentID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "attendance");

            migrationBuilder.DropTable(
                name: "attendance_states");
        }
    }
}
