using Microsoft.EntityFrameworkCore.Migrations;

namespace JLSDataAccess.Migrations
{
    public partial class addBodyInEmail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Template",
                table: "EmailTemplate",
                newName: "Title");

            migrationBuilder.AddColumn<string>(
                name: "Body",
                table: "EmailTemplate",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Body",
                table: "EmailTemplate");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "EmailTemplate",
                newName: "Template");
        }
    }
}
