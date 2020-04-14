using Microsoft.EntityFrameworkCore.Migrations;

namespace FacultyMVC.Migrations
{
    public partial class AddAtrIndexToStudent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Index",
                table: "Student",
                maxLength: 10,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Index",
                table: "Student");
        }
    }
}
