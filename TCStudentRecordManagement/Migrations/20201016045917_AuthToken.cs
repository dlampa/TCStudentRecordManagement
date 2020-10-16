using Microsoft.EntityFrameworkCore.Migrations;

namespace TCStudentRecordManagement.Migrations
{
    public partial class AuthToken : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ActiveToken",
                table: "users",
                type: "varchar(2048)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActiveToken",
                table: "users");
        }
    }
}
