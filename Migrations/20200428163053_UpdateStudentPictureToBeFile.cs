using Microsoft.EntityFrameworkCore.Migrations;

namespace FacultyMVC.Migrations
{
    public partial class UpdateStudentPictureToBeFile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProfilePicture",
                table: "Teacher",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProfilePicture",
                table: "Student",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfilePicture",
                table: "Teacher");

            migrationBuilder.DropColumn(
                name: "ProfilePicture",
                table: "Student");
        }
    }
}
