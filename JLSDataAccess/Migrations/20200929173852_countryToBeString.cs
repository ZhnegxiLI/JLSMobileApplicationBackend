using Microsoft.EntityFrameworkCore.Migrations;

namespace JLSDataAccess.Migrations
{
    public partial class countryToBeString : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CountryId",
                table: "Adress");

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "Adress",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Country",
                table: "Adress");

            migrationBuilder.AddColumn<long>(
                name: "CountryId",
                table: "Adress",
                nullable: true);
        }
    }
}