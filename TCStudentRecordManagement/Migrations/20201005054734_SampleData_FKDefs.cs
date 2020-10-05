using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TCStudentRecordManagement.Migrations
{
    public partial class SampleData_FKDefs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_students_cohorts_CohortID",
                table: "students");

            migrationBuilder.RenameIndex(
                name: "IX_students_UserID",
                table: "students",
                newName: "FK_Student_User");

            migrationBuilder.RenameIndex(
                name: "IX_students_CohortID",
                table: "students",
                newName: "FK_Student_Cohort");

            migrationBuilder.InsertData(
                table: "cohorts",
                columns: new[] { "CohortID", "EndDate", "Name", "StartDate" },
                values: new object[,]
                {
                    { -1, new DateTime(2020, 10, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), "4.1 Summer/Autumn 2020", new DateTime(2020, 6, 15, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { -2, new DateTime(2020, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), "4.2 Autumn/Winter 2020", new DateTime(2020, 8, 15, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "users",
                columns: new[] { "UserID", "Email", "Firstname", "Lastname", "Rights" },
                values: new object[,]
                {
                    { 1, "john.dough@abc.com", "John", "Dough", (byte)1 },
                    { 2, "anne.jackson@abc.com", "Anne", "Jackson", (byte)1 },
                    { 3, "ivan.patterson@abc.com", "Ivan", "Patterson", (byte)1 },
                    { 4, "trevor.noah@abc.com", "Trevor", "Noah", (byte)1 },
                    { 5, "peter.patsie@abc.com", "Peter", "Patsie", (byte)1 },
                    { 6, "jane.smitherson@abc.com", "Jane", "Smitherson", (byte)2 }
                });

            migrationBuilder.InsertData(
                table: "students",
                columns: new[] { "StudentID", "BearTracksID", "CohortID", "UserID" },
                values: new object[,]
                {
                    { -1, null, -1, 1 },
                    { -2, null, -1, 2 },
                    { -3, null, -1, 3 },
                    { -4, null, -2, 4 },
                    { -5, null, -2, 5 }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Student_Cohort",
                table: "students",
                column: "CohortID",
                principalTable: "cohorts",
                principalColumn: "CohortID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Student_Cohort",
                table: "students");

            migrationBuilder.DeleteData(
                table: "students",
                keyColumn: "StudentID",
                keyValue: -5);

            migrationBuilder.DeleteData(
                table: "students",
                keyColumn: "StudentID",
                keyValue: -4);

            migrationBuilder.DeleteData(
                table: "students",
                keyColumn: "StudentID",
                keyValue: -3);

            migrationBuilder.DeleteData(
                table: "students",
                keyColumn: "StudentID",
                keyValue: -2);

            migrationBuilder.DeleteData(
                table: "students",
                keyColumn: "StudentID",
                keyValue: -1);

            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "UserID",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "cohorts",
                keyColumn: "CohortID",
                keyValue: -2);

            migrationBuilder.DeleteData(
                table: "cohorts",
                keyColumn: "CohortID",
                keyValue: -1);

            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "UserID",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "UserID",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "UserID",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "UserID",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "users",
                keyColumn: "UserID",
                keyValue: 5);

            migrationBuilder.RenameIndex(
                name: "FK_Student_User",
                table: "students",
                newName: "IX_students_UserID");

            migrationBuilder.RenameIndex(
                name: "FK_Student_Cohort",
                table: "students",
                newName: "IX_students_CohortID");

            migrationBuilder.AddForeignKey(
                name: "FK_students_cohorts_CohortID",
                table: "students",
                column: "CohortID",
                principalTable: "cohorts",
                principalColumn: "CohortID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
