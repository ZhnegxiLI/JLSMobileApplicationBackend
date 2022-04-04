using Microsoft.EntityFrameworkCore.Migrations;

namespace JLSDataAccess.Migrations
{
    public partial class modifyProductTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TaxRate",
                table: "Product");

            migrationBuilder.AddColumn<long>(
                name: "TaxRateId",
                table: "Product",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TaxRateId",
                table: "Product");

            migrationBuilder.AddColumn<float>(
                name: "TaxRate",
                table: "Product",
                nullable: true);
        }
    }
}