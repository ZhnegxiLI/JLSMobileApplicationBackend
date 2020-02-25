using Microsoft.EntityFrameworkCore.Migrations;

namespace JLSDataAccess.Migrations
{
    public partial class addEntrepriseInAdressTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EntrepriseName",
                table: "Adress",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EntrepriseName",
                table: "Adress");
        }
    }
}
