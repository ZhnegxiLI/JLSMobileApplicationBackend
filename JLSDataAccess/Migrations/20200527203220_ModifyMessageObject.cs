using Microsoft.EntityFrameworkCore.Migrations;

namespace JLSDataAccess.Migrations
{
    public partial class ModifyMessageObject : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SenderEmail",
                table: "Message",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SenderName",
                table: "Message",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SenderEmail",
                table: "Message");

            migrationBuilder.DropColumn(
                name: "SenderName",
                table: "Message");
        }
    }
}
