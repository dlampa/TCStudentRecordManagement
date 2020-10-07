using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TCStudentRecordManagement.Migrations
{
    public partial class Timesheets : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assignment_Cohort",
                table: "assignments");

            migrationBuilder.CreateTable(
                name: "timesheets",
                columns: table => new
                {
                    RecordID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentID = table.Column<int>(type: "int", nullable: false),
                    AssignmentID = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TimeAllocation = table.Column<decimal>(type: "decimal(3,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_timesheets", x => x.RecordID);
                    table.ForeignKey(
                        name: "FK_Timesheet_Assignment",
                        column: x => x.AssignmentID,
                        principalTable: "assignments",
                        principalColumn: "AssignmentID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Timesheet_Student",
                        column: x => x.StudentID,
                        principalTable: "students",
                        principalColumn: "StudentID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "assignment_types",
                keyColumn: "TypeID",
                keyValue: -4,
                column: "Description",
                value: "Practice");

            migrationBuilder.UpdateData(
                table: "assignment_types",
                keyColumn: "TypeID",
                keyValue: -3,
                column: "Description",
                value: "Break");

            migrationBuilder.UpdateData(
                table: "assignment_types",
                keyColumn: "TypeID",
                keyValue: -2,
                column: "Description",
                value: "Online self-study");

            migrationBuilder.UpdateData(
                table: "assignment_types",
                keyColumn: "TypeID",
                keyValue: -1,
                column: "Description",
                value: "Classroom lectures");

            migrationBuilder.InsertData(
                table: "assignment_types",
                columns: new[] { "TypeID", "Description" },
                values: new object[,]
                {
                    { -5, "Weekend assignment" },
                    { -6, "Milestone" },
                    { -7, "Capstone" }
                });

            migrationBuilder.InsertData(
                table: "assignments",
                columns: new[] { "AssignmentID", "CohortID", "DocURL", "EndDate", "StartDate", "Title", "TopicID", "TypeID" },
                values: new object[] { -5, -1, "http://does-not-exist.com/", new DateTime(2020, 9, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2020, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "Entity Framework Practice 2: Cars", -13, -4 });

            migrationBuilder.InsertData(
                table: "cohorts",
                columns: new[] { "CohortID", "EndDate", "Name", "StartDate" },
                values: new object[] { -9999, new DateTime(9999, 12, 31, 23, 59, 59, 999, DateTimeKind.Unspecified).AddTicks(9999), "Common", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) });

            migrationBuilder.UpdateData(
                table: "topics",
                keyColumn: "TopicID",
                keyValue: -15,
                column: "Description",
                value: "C# WebAPI");

            migrationBuilder.UpdateData(
                table: "topics",
                keyColumn: "TopicID",
                keyValue: -14,
                column: "Description",
                value: "C# MVC");

            migrationBuilder.UpdateData(
                table: "topics",
                keyColumn: "TopicID",
                keyValue: -13,
                column: "Description",
                value: "C# Entity Framework");

            migrationBuilder.UpdateData(
                table: "topics",
                keyColumn: "TopicID",
                keyValue: -12,
                column: "Description",
                value: "C# OOP");

            migrationBuilder.UpdateData(
                table: "topics",
                keyColumn: "TopicID",
                keyValue: -11,
                column: "Description",
                value: "PHP APIs");

            migrationBuilder.UpdateData(
                table: "topics",
                keyColumn: "TopicID",
                keyValue: -10,
                column: "Description",
                value: "PHP fundamentals");

            migrationBuilder.UpdateData(
                table: "topics",
                keyColumn: "TopicID",
                keyValue: -9,
                column: "Description",
                value: "Wordpress");

            migrationBuilder.UpdateData(
                table: "topics",
                keyColumn: "TopicID",
                keyValue: -8,
                column: "Description",
                value: "SQL fundamentals");

            migrationBuilder.UpdateData(
                table: "topics",
                keyColumn: "TopicID",
                keyValue: -7,
                column: "Description",
                value: "React/Redux");

            migrationBuilder.UpdateData(
                table: "topics",
                keyColumn: "TopicID",
                keyValue: -6,
                column: "Description",
                value: "React fundamentals");

            migrationBuilder.UpdateData(
                table: "topics",
                keyColumn: "TopicID",
                keyValue: -5,
                column: "Description",
                value: "Javascript AJAX and API");

            migrationBuilder.UpdateData(
                table: "topics",
                keyColumn: "TopicID",
                keyValue: -4,
                column: "Description",
                value: "Javascript fundamentals");

            migrationBuilder.UpdateData(
                table: "topics",
                keyColumn: "TopicID",
                keyValue: -3,
                column: "Description",
                value: "HTML5 and CSS");

            migrationBuilder.UpdateData(
                table: "topics",
                keyColumn: "TopicID",
                keyValue: -2,
                column: "Description",
                value: "C# fundamentals");

            migrationBuilder.UpdateData(
                table: "topics",
                keyColumn: "TopicID",
                keyValue: -1,
                column: "Description",
                value: "Generic");

            migrationBuilder.InsertData(
                table: "topics",
                columns: new[] { "TopicID", "Description" },
                values: new object[] { -16, "C# WebAPI and React" });

            migrationBuilder.InsertData(
                table: "assignments",
                columns: new[] { "AssignmentID", "CohortID", "DocURL", "EndDate", "StartDate", "Title", "TopicID", "TypeID" },
                values: new object[,]
                {
                    { -4, -1, "http://does-not-exist.com/", new DateTime(2020, 6, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2020, 6, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "Tic-Tac-Toe", -2, -5 },
                    { -8, -2, "http://does-not-exist.com/", new DateTime(2020, 9, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2020, 7, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "Javascript Todo App", -4, -5 },
                    { -6, -2, "http://does-not-exist.com/", new DateTime(2020, 9, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2020, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "React/Redux Group Milestone 2", -7, -6 },
                    { -7, -2, "http://does-not-exist.com/", new DateTime(2020, 7, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2020, 7, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Personal Portfolio: Milestone 1", -15, -6 },
                    { -1, -9999, null, new DateTime(9999, 12, 31, 23, 59, 59, 999, DateTimeKind.Unspecified).AddTicks(9999), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Classroom instruction", -1, -1 },
                    { -2, -9999, null, new DateTime(9999, 12, 31, 23, 59, 59, 999, DateTimeKind.Unspecified).AddTicks(9999), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Online self-study", -1, -2 },
                    { -3, -9999, null, new DateTime(9999, 12, 31, 23, 59, 59, 999, DateTimeKind.Unspecified).AddTicks(9999), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Break", -1, -3 }
                });

            migrationBuilder.CreateIndex(
                name: "FK_Timesheet_Assignment",
                table: "timesheets",
                column: "AssignmentID");

            migrationBuilder.CreateIndex(
                name: "FK_Timesheet_Student",
                table: "timesheets",
                column: "StudentID");

            migrationBuilder.AddForeignKey(
                name: "FK_Assignment_Cohort",
                table: "assignments",
                column: "CohortID",
                principalTable: "cohorts",
                principalColumn: "CohortID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assignment_Cohort",
                table: "assignments");

            migrationBuilder.DropTable(
                name: "timesheets");

            migrationBuilder.DeleteData(
                table: "assignment_types",
                keyColumn: "TypeID",
                keyValue: -7);

            migrationBuilder.DeleteData(
                table: "assignments",
                keyColumn: "AssignmentID",
                keyValue: -8);

            migrationBuilder.DeleteData(
                table: "assignments",
                keyColumn: "AssignmentID",
                keyValue: -7);

            migrationBuilder.DeleteData(
                table: "assignments",
                keyColumn: "AssignmentID",
                keyValue: -6);

            migrationBuilder.DeleteData(
                table: "assignments",
                keyColumn: "AssignmentID",
                keyValue: -5);

            migrationBuilder.DeleteData(
                table: "assignments",
                keyColumn: "AssignmentID",
                keyValue: -4);

            migrationBuilder.DeleteData(
                table: "assignments",
                keyColumn: "AssignmentID",
                keyValue: -3);

            migrationBuilder.DeleteData(
                table: "assignments",
                keyColumn: "AssignmentID",
                keyValue: -2);

            migrationBuilder.DeleteData(
                table: "assignments",
                keyColumn: "AssignmentID",
                keyValue: -1);

            migrationBuilder.DeleteData(
                table: "topics",
                keyColumn: "TopicID",
                keyValue: -16);

            migrationBuilder.DeleteData(
                table: "assignment_types",
                keyColumn: "TypeID",
                keyValue: -6);

            migrationBuilder.DeleteData(
                table: "assignment_types",
                keyColumn: "TypeID",
                keyValue: -5);

            migrationBuilder.DeleteData(
                table: "cohorts",
                keyColumn: "CohortID",
                keyValue: -9999);

            migrationBuilder.UpdateData(
                table: "assignment_types",
                keyColumn: "TypeID",
                keyValue: -4,
                column: "Description",
                value: "Capstone");

            migrationBuilder.UpdateData(
                table: "assignment_types",
                keyColumn: "TypeID",
                keyValue: -3,
                column: "Description",
                value: "Milestone");

            migrationBuilder.UpdateData(
                table: "assignment_types",
                keyColumn: "TypeID",
                keyValue: -2,
                column: "Description",
                value: "Weekend assignment");

            migrationBuilder.UpdateData(
                table: "assignment_types",
                keyColumn: "TypeID",
                keyValue: -1,
                column: "Description",
                value: "Practice");

            migrationBuilder.UpdateData(
                table: "topics",
                keyColumn: "TopicID",
                keyValue: -15,
                column: "Description",
                value: "C# WebAPI and React");

            migrationBuilder.UpdateData(
                table: "topics",
                keyColumn: "TopicID",
                keyValue: -14,
                column: "Description",
                value: "C# WebAPI");

            migrationBuilder.UpdateData(
                table: "topics",
                keyColumn: "TopicID",
                keyValue: -13,
                column: "Description",
                value: "C# MVC");

            migrationBuilder.UpdateData(
                table: "topics",
                keyColumn: "TopicID",
                keyValue: -12,
                column: "Description",
                value: "C# Entity Framework");

            migrationBuilder.UpdateData(
                table: "topics",
                keyColumn: "TopicID",
                keyValue: -11,
                column: "Description",
                value: "C# OOP");

            migrationBuilder.UpdateData(
                table: "topics",
                keyColumn: "TopicID",
                keyValue: -10,
                column: "Description",
                value: "PHP APIs");

            migrationBuilder.UpdateData(
                table: "topics",
                keyColumn: "TopicID",
                keyValue: -9,
                column: "Description",
                value: "PHP fundamentals");

            migrationBuilder.UpdateData(
                table: "topics",
                keyColumn: "TopicID",
                keyValue: -8,
                column: "Description",
                value: "Wordpress");

            migrationBuilder.UpdateData(
                table: "topics",
                keyColumn: "TopicID",
                keyValue: -7,
                column: "Description",
                value: "SQL fundamentals");

            migrationBuilder.UpdateData(
                table: "topics",
                keyColumn: "TopicID",
                keyValue: -6,
                column: "Description",
                value: "React/Redux");

            migrationBuilder.UpdateData(
                table: "topics",
                keyColumn: "TopicID",
                keyValue: -5,
                column: "Description",
                value: "React fundamentals");

            migrationBuilder.UpdateData(
                table: "topics",
                keyColumn: "TopicID",
                keyValue: -4,
                column: "Description",
                value: "Javascript AJAX and API");

            migrationBuilder.UpdateData(
                table: "topics",
                keyColumn: "TopicID",
                keyValue: -3,
                column: "Description",
                value: "Javascript fundamentals");

            migrationBuilder.UpdateData(
                table: "topics",
                keyColumn: "TopicID",
                keyValue: -2,
                column: "Description",
                value: "HTML5 and CSS");

            migrationBuilder.UpdateData(
                table: "topics",
                keyColumn: "TopicID",
                keyValue: -1,
                column: "Description",
                value: "C# fundamentals");

            migrationBuilder.AddForeignKey(
                name: "FK_Assignment_Cohort",
                table: "assignments",
                column: "CohortID",
                principalTable: "cohorts",
                principalColumn: "CohortID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
