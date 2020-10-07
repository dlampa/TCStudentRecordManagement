using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TCStudentRecordManagement.Migrations
{
    public partial class Staff_TableRename : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_attendance_users_StaffID",
                table: "attendance");

            migrationBuilder.DropForeignKey(
                name: "FK_Notice_User",
                table: "notices");

            migrationBuilder.DropForeignKey(
                name: "FK_Timesheet_Assignment",
                table: "timesheets");

            migrationBuilder.DropTable(
                name: "assignments");

            migrationBuilder.DropTable(
                name: "assignment_types");

            migrationBuilder.DropColumn(
                name: "Rights",
                table: "users");

            migrationBuilder.RenameColumn(
                name: "TopicID",
                table: "topics",
                newName: "UnitID");

            migrationBuilder.RenameIndex(
                name: "FK_Timesheet_Assignment",
                table: "timesheets",
                newName: "FK_Timesheet_Task");

            migrationBuilder.RenameIndex(
                name: "FK_Notice_User",
                table: "notices",
                newName: "FK_Notice_Staff");

            migrationBuilder.RenameIndex(
                name: "FK_Attendance_User",
                table: "attendance",
                newName: "FK_Attendance_Staff");

            migrationBuilder.AddColumn<bool>(
                name: "Active",
                table: "users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Date",
                table: "timesheets",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<string>(
                name: "Comment",
                table: "attendance",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "staff",
                columns: table => new
                {
                    StaffID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    SuperUser = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_staff", x => x.StaffID);
                    table.ForeignKey(
                        name: "FK_Staff_User",
                        column: x => x.UserID,
                        principalTable: "users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "task_types",
                columns: table => new
                {
                    TypeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "varchar(50)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_task_types", x => x.TypeID);
                });

            migrationBuilder.CreateTable(
                name: "tasks",
                columns: table => new
                {
                    TaskID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UnitID = table.Column<int>(type: "int", nullable: false),
                    TypeID = table.Column<int>(type: "int", nullable: false),
                    CohortID = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "varchar(100)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocURL = table.Column<string>(type: "varchar(255)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tasks", x => x.TaskID);
                    table.ForeignKey(
                        name: "FK_Task_Cohort",
                        column: x => x.CohortID,
                        principalTable: "cohorts",
                        principalColumn: "CohortID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Task_TaskType",
                        column: x => x.TypeID,
                        principalTable: "task_types",
                        principalColumn: "TypeID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Task_Unit",
                        column: x => x.UnitID,
                        principalTable: "topics",
                        principalColumn: "UnitID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "staff",
                columns: new[] { "StaffID", "SuperUser", "UserID" },
                values: new object[] { -1, false, 6 });

            migrationBuilder.InsertData(
                table: "task_types",
                columns: new[] { "TypeID", "Description" },
                values: new object[,]
                {
                    { -1, "Classroom lectures" },
                    { -2, "Online self-study" },
                    { -3, "Break" },
                    { -4, "Practice" },
                    { -5, "Weekend assignment" },
                    { -6, "Milestone" },
                    { -7, "Capstone" }
                });

            migrationBuilder.UpdateData(
                table: "attendance",
                keyColumn: "RecordID",
                keyValue: -2,
                column: "StaffID",
                value: -1);

            migrationBuilder.UpdateData(
                table: "attendance",
                keyColumn: "RecordID",
                keyValue: -1,
                column: "StaffID",
                value: -1);

            migrationBuilder.UpdateData(
                table: "notices",
                keyColumn: "NoticeID",
                keyValue: -2,
                column: "StaffID",
                value: -1);

            migrationBuilder.UpdateData(
                table: "notices",
                keyColumn: "NoticeID",
                keyValue: -1,
                column: "StaffID",
                value: -1);

            migrationBuilder.InsertData(
                table: "tasks",
                columns: new[] { "TaskID", "CohortID", "DocURL", "EndDate", "StartDate", "Title", "TypeID", "UnitID" },
                values: new object[,]
                {
                    { -1, -9999, null, new DateTime(9999, 12, 31, 23, 59, 59, 999, DateTimeKind.Unspecified).AddTicks(9999), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Classroom instruction", -1, -1 },
                    { -2, -9999, null, new DateTime(9999, 12, 31, 23, 59, 59, 999, DateTimeKind.Unspecified).AddTicks(9999), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Online self-study", -2, -1 },
                    { -3, -9999, null, new DateTime(9999, 12, 31, 23, 59, 59, 999, DateTimeKind.Unspecified).AddTicks(9999), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Break", -3, -1 },
                    { -5, -1, "http://does-not-exist.com/", new DateTime(2020, 9, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2020, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "Entity Framework Practice 2: Cars", -4, -13 },
                    { -4, -1, "http://does-not-exist.com/", new DateTime(2020, 6, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2020, 6, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "Tic-Tac-Toe", -5, -2 },
                    { -8, -2, "http://does-not-exist.com/", new DateTime(2020, 9, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2020, 7, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "Javascript Todo App", -5, -4 },
                    { -6, -2, "http://does-not-exist.com/", new DateTime(2020, 9, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2020, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "React/Redux Group Milestone 2", -6, -7 },
                    { -7, -2, "http://does-not-exist.com/", new DateTime(2020, 7, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2020, 7, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Personal Portfolio: Milestone 1", -6, -15 }
                });

            migrationBuilder.CreateIndex(
                name: "FK_Staff_User",
                table: "staff",
                column: "UserID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "FK_Task_Cohort",
                table: "tasks",
                column: "CohortID");

            migrationBuilder.CreateIndex(
                name: "FK_Task_TaskType",
                table: "tasks",
                column: "TypeID");

            migrationBuilder.CreateIndex(
                name: "FK_Task_Unit",
                table: "tasks",
                column: "UnitID");

            migrationBuilder.AddForeignKey(
                name: "FK_Attendance_Staff",
                table: "attendance",
                column: "StaffID",
                principalTable: "staff",
                principalColumn: "StaffID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Notice_Staff",
                table: "notices",
                column: "StaffID",
                principalTable: "staff",
                principalColumn: "StaffID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Timesheet_Task",
                table: "timesheets",
                column: "AssignmentID",
                principalTable: "tasks",
                principalColumn: "TaskID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Attendance_Staff",
                table: "attendance");

            migrationBuilder.DropForeignKey(
                name: "FK_Notice_Staff",
                table: "notices");

            migrationBuilder.DropForeignKey(
                name: "FK_Timesheet_Task",
                table: "timesheets");

            migrationBuilder.DropTable(
                name: "staff");

            migrationBuilder.DropTable(
                name: "tasks");

            migrationBuilder.DropTable(
                name: "task_types");

            migrationBuilder.DropColumn(
                name: "Active",
                table: "users");

            migrationBuilder.DropColumn(
                name: "Comment",
                table: "attendance");

            migrationBuilder.RenameColumn(
                name: "UnitID",
                table: "topics",
                newName: "TopicID");

            migrationBuilder.RenameIndex(
                name: "FK_Timesheet_Task",
                table: "timesheets",
                newName: "FK_Timesheet_Assignment");

            migrationBuilder.RenameIndex(
                name: "FK_Notice_Staff",
                table: "notices",
                newName: "FK_Notice_User");

            migrationBuilder.RenameIndex(
                name: "FK_Attendance_Staff",
                table: "attendance",
                newName: "FK_Attendance_User");

            migrationBuilder.AddColumn<byte>(
                name: "Rights",
                table: "users",
                type: "tinyint",
                nullable: false,
                defaultValue: (byte)0);

            migrationBuilder.AlterColumn<DateTime>(
                name: "Date",
                table: "timesheets",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "date");

            migrationBuilder.CreateTable(
                name: "assignment_types",
                columns: table => new
                {
                    TypeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "varchar(50)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_assignment_types", x => x.TypeID);
                });

            migrationBuilder.CreateTable(
                name: "assignments",
                columns: table => new
                {
                    AssignmentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CohortID = table.Column<int>(type: "int", nullable: false),
                    DocURL = table.Column<string>(type: "varchar(255)", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Title = table.Column<string>(type: "varchar(100)", nullable: false),
                    TopicID = table.Column<int>(type: "int", nullable: false),
                    TypeID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_assignments", x => x.AssignmentID);
                    table.ForeignKey(
                        name: "FK_Assignment_Cohort",
                        column: x => x.CohortID,
                        principalTable: "cohorts",
                        principalColumn: "CohortID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Assignment_Topic",
                        column: x => x.TopicID,
                        principalTable: "topics",
                        principalColumn: "TopicID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Assignment_AssignmentType",
                        column: x => x.TypeID,
                        principalTable: "assignment_types",
                        principalColumn: "TypeID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "assignment_types",
                columns: new[] { "TypeID", "Description" },
                values: new object[,]
                {
                    { -1, "Classroom lectures" },
                    { -2, "Online self-study" },
                    { -3, "Break" },
                    { -4, "Practice" },
                    { -5, "Weekend assignment" },
                    { -6, "Milestone" },
                    { -7, "Capstone" }
                });

            migrationBuilder.UpdateData(
                table: "attendance",
                keyColumn: "RecordID",
                keyValue: -2,
                column: "StaffID",
                value: 6);

            migrationBuilder.UpdateData(
                table: "attendance",
                keyColumn: "RecordID",
                keyValue: -1,
                column: "StaffID",
                value: 6);

            migrationBuilder.UpdateData(
                table: "notices",
                keyColumn: "NoticeID",
                keyValue: -2,
                column: "StaffID",
                value: 6);

            migrationBuilder.UpdateData(
                table: "notices",
                keyColumn: "NoticeID",
                keyValue: -1,
                column: "StaffID",
                value: 6);

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "UserID",
                keyValue: 1,
                column: "Rights",
                value: (byte)1);

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "UserID",
                keyValue: 2,
                column: "Rights",
                value: (byte)1);

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "UserID",
                keyValue: 3,
                column: "Rights",
                value: (byte)1);

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "UserID",
                keyValue: 4,
                column: "Rights",
                value: (byte)1);

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "UserID",
                keyValue: 5,
                column: "Rights",
                value: (byte)1);

            migrationBuilder.UpdateData(
                table: "users",
                keyColumn: "UserID",
                keyValue: 6,
                column: "Rights",
                value: (byte)2);

            migrationBuilder.InsertData(
                table: "assignments",
                columns: new[] { "AssignmentID", "CohortID", "DocURL", "EndDate", "StartDate", "Title", "TopicID", "TypeID" },
                values: new object[,]
                {
                    { -1, -9999, null, new DateTime(9999, 12, 31, 23, 59, 59, 999, DateTimeKind.Unspecified).AddTicks(9999), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Classroom instruction", -1, -1 },
                    { -2, -9999, null, new DateTime(9999, 12, 31, 23, 59, 59, 999, DateTimeKind.Unspecified).AddTicks(9999), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Online self-study", -1, -2 },
                    { -3, -9999, null, new DateTime(9999, 12, 31, 23, 59, 59, 999, DateTimeKind.Unspecified).AddTicks(9999), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Break", -1, -3 },
                    { -5, -1, "http://does-not-exist.com/", new DateTime(2020, 9, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2020, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "Entity Framework Practice 2: Cars", -13, -4 },
                    { -4, -1, "http://does-not-exist.com/", new DateTime(2020, 6, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2020, 6, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "Tic-Tac-Toe", -2, -5 },
                    { -8, -2, "http://does-not-exist.com/", new DateTime(2020, 9, 23, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2020, 7, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "Javascript Todo App", -4, -5 },
                    { -6, -2, "http://does-not-exist.com/", new DateTime(2020, 9, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2020, 9, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "React/Redux Group Milestone 2", -7, -6 },
                    { -7, -2, "http://does-not-exist.com/", new DateTime(2020, 7, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2020, 7, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Personal Portfolio: Milestone 1", -15, -6 }
                });

            migrationBuilder.CreateIndex(
                name: "FK_Assignment_Cohort",
                table: "assignments",
                column: "CohortID");

            migrationBuilder.CreateIndex(
                name: "FK_Assignment_Topic",
                table: "assignments",
                column: "TopicID");

            migrationBuilder.CreateIndex(
                name: "FK_Assignment_AssignmentType",
                table: "assignments",
                column: "TypeID");

            migrationBuilder.AddForeignKey(
                name: "FK_attendance_users_StaffID",
                table: "attendance",
                column: "StaffID",
                principalTable: "users",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Notice_User",
                table: "notices",
                column: "StaffID",
                principalTable: "users",
                principalColumn: "UserID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Timesheet_Assignment",
                table: "timesheets",
                column: "AssignmentID",
                principalTable: "assignments",
                principalColumn: "AssignmentID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
