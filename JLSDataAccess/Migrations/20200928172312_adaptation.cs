using Microsoft.EntityFrameworkCore.Migrations;

namespace JLSDataAccess.Migrations
{
    public partial class adaptation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Colissage",
                table: "OrderProduct",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<float>(
                name: "TotalPriceHT",
                table: "OrderInfo",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Colissage",
                table: "OrderProduct");

            migrationBuilder.DropColumn(
                name: "TotalPriceHT",
                table: "OrderInfo");
        }
    }
}
