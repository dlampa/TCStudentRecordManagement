using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TCStudentRecordManagement.Migrations
{
    public partial class NewInit : Migration
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
                name: "cohorts",
                columns: table => new
                {
                    CohortID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_cohorts", x => x.CohortID);
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
                name: "units",
                columns: table => new
                {
                    UnitID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "varchar(50)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_units", x => x.UnitID);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    UserID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Firstname = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    Lastname = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    Email = table.Column<string>(type: "varchar(320)", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.UserID);
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
                        principalTable: "units",
                        principalColumn: "UnitID",
                        onDelete: ReferentialAction.Restrict);
                });

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
                name: "students",
                columns: table => new
                {
                    StudentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    CohortID = table.Column<int>(type: "int", nullable: false),
                    BearTracksID = table.Column<string>(type: "varchar(10)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_students", x => x.StudentID);
                    table.ForeignKey(
                        name: "FK_Student_Cohort",
                        column: x => x.CohortID,
                        principalTable: "cohorts",
                        principalColumn: "CohortID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Student_User",
                        column: x => x.UserID,
                        principalTable: "users",
                        principalColumn: "UserID",
                        onDelete: ReferentialAction.Cascade);
                });

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
                        name: "FK_Notice_Staff",
                        column: x => x.StaffID,
                        principalTable: "staff",
                        principalColumn: "StaffID",
                        onDelete: ReferentialAction.Restrict);
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
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
                        name: "FK_Attendance_Staff",
                        column: x => x.StaffID,
                        principalTable: "staff",
                        principalColumn: "StaffID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Attendance_Student",
                        column: x => x.StudentID,
                        principalTable: "students",
                        principalColumn: "StudentID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "timesheets",
                columns: table => new
                {
                    RecordID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentID = table.Column<int>(type: "int", nullable: false),
                    AssignmentID = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "date", nullable: false),
                    TimeAllocation = table.Column<decimal>(type: "decimal(3,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_timesheets", x => x.RecordID);
                    table.ForeignKey(
                        name: "FK_Timesheet_Task",
                        column: x => x.AssignmentID,
                        principalTable: "tasks",
                        principalColumn: "TaskID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Timesheet_Student",
                        column: x => x.StudentID,
                        principalTable: "students",
                        principalColumn: "StudentID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "FK_Attendance_AttendanceState",
                table: "attendance",
                column: "AttendanceStateID");

            migrationBuilder.CreateIndex(
                name: "FK_Attendance_Staff",
                table: "attendance",
                column: "StaffID");

            migrationBuilder.CreateIndex(
                name: "FK_Attendance_Student",
                table: "attendance",
                column: "StudentID");

            migrationBuilder.CreateIndex(
                name: "FK_Notice_Cohort",
                table: "notices",
                column: "CohortID");

            migrationBuilder.CreateIndex(
                name: "FK_Notice_Staff",
                table: "notices",
                column: "StaffID");

            migrationBuilder.CreateIndex(
                name: "FK_Staff_User",
                table: "staff",
                column: "UserID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "FK_Student_Cohort",
                table: "students",
                column: "CohortID");

            migrationBuilder.CreateIndex(
                name: "FK_Student_User",
                table: "students",
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

            migrationBuilder.CreateIndex(
                name: "FK_Timesheet_Task",
                table: "timesheets",
                column: "AssignmentID");

            migrationBuilder.CreateIndex(
                name: "FK_Timesheet_Student",
                table: "timesheets",
                column: "StudentID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "attendance");

            migrationBuilder.DropTable(
                name: "notices");

            migrationBuilder.DropTable(
                name: "timesheets");

            migrationBuilder.DropTable(
                name: "attendance_states");

            migrationBuilder.DropTable(
                name: "staff");

            migrationBuilder.DropTable(
                name: "tasks");

            migrationBuilder.DropTable(
                name: "students");

            migrationBuilder.DropTable(
                name: "task_types");

            migrationBuilder.DropTable(
                name: "units");

            migrationBuilder.DropTable(
                name: "cohorts");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
