using Microsoft.EntityFrameworkCore.Migrations;

namespace JLSDataAccess.Migrations
{
    public partial class addColumnTaxRateIdInOrderInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TaxRate",
                table: "OrderInfo");

            migrationBuilder.AddColumn<long>(
                name: "TaxRateId",
                table: "OrderInfo",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TaxRateId",
                table: "OrderInfo");

            migrationBuilder.AddColumn<float>(
                name: "TaxRate",
                table: "OrderInfo",
                nullable: true);
        }
    }
}