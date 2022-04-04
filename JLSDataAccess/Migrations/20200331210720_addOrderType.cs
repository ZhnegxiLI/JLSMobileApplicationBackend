using Microsoft.EntityFrameworkCore.Migrations;

namespace JLSDataAccess.Migrations
{
    public partial class addOrderType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderType",
                table: "OrderInfo");

            migrationBuilder.AddColumn<long>(
                name: "OrderTypeId",
                table: "OrderInfo",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderTypeId",
                table: "OrderInfo");

            migrationBuilder.AddColumn<string>(
                name: "OrderType",
                table: "OrderInfo",
                nullable: true);
        }
    }
}