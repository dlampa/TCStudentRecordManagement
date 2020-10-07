using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TCStudentRecordManagement.Migrations
{
    public partial class Assignments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                name: "topics",
                columns: table => new
                {
                    TopicID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "varchar(50)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_topics", x => x.TopicID);
                });

            migrationBuilder.CreateTable(
                name: "assignments",
                columns: table => new
                {
                    AssignmentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TopicID = table.Column<int>(type: "int", nullable: false),
                    TypeID = table.Column<int>(type: "int", nullable: false),
                    CohortID = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "varchar(100)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocURL = table.Column<string>(type: "varchar(255)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_assignments", x => x.AssignmentID);
                    table.ForeignKey(
                        name: "FK_Assignment_Cohort",
                        column: x => x.CohortID,
                        principalTable: "cohorts",
                        principalColumn: "CohortID",
                        onDelete: ReferentialAction.Restrict);
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
                    { -1, "Practice" },
                    { -2, "Weekend assignment" },
                    { -3, "Milestone" },
                    { -4, "Capstone" }
                });

            migrationBuilder.InsertData(
                table: "topics",
                columns: new[] { "TopicID", "Description" },
                values: new object[,]
                {
                    { -13, "C# MVC" },
                    { -12, "C# Entity Framework" },
                    { -11, "C# OOP" },
                    { -10, "PHP APIs" },
                    { -9, "PHP fundamentals" },
                    { -8, "Wordpress" },
                    { -6, "React/Redux" },
                    { -14, "C# WebAPI" },
                    { -5, "React fundamentals" },
                    { -4, "Javascript AJAX and API" },
                    { -3, "Javascript fundamentals" },
                    { -2, "HTML5 and CSS" },
                    { -1, "C# fundamentals" },
                    { -7, "SQL fundamentals" },
                    { -15, "C# WebAPI and React" }
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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "assignments");

            migrationBuilder.DropTable(
                name: "topics");

            migrationBuilder.DropTable(
                name: "assignment_types");
        }
    }
}
