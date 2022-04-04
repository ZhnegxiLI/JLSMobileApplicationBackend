using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace JLSDataAccess.Migrations
{
    public partial class changeAdressTableStructure : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderInfo_Adress_AdressId",
                table: "OrderInfo");

            migrationBuilder.DropColumn(
                name: "ContactTelephone",
                table: "OrderInfo");

            migrationBuilder.RenameColumn(
                name: "AdressId",
                table: "OrderInfo",
                newName: "ShippingAdressId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderInfo_AdressId",
                table: "OrderInfo",
                newName: "IX_OrderInfo_ShippingAdressId");

            migrationBuilder.AddColumn<long>(
                name: "FacturationAdressId",
                table: "OrderInfo",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "OrderProduct",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedOn = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<long>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    UpdatedBy = table.Column<long>(nullable: true),
                    Quantity = table.Column<int>(nullable: false),
                    OrderId = table.Column<long>(nullable: false),
                    OrderInfoId = table.Column<long>(nullable: true),
                    ProductId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderProduct", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderProduct_OrderInfo_OrderInfoId",
                        column: x => x.OrderInfoId,
                        principalTable: "OrderInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_OrderProduct_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderInfo_FacturationAdressId",
                table: "OrderInfo",
                column: "FacturationAdressId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderProduct_OrderInfoId",
                table: "OrderProduct",
                column: "OrderInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderProduct_ProductId",
                table: "OrderProduct",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderInfo_Adress_FacturationAdressId",
                table: "OrderInfo",
                column: "FacturationAdressId",
                principalTable: "Adress",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderInfo_Adress_ShippingAdressId",
                table: "OrderInfo",
                column: "ShippingAdressId",
                principalTable: "Adress",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderInfo_Adress_FacturationAdressId",
                table: "OrderInfo");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderInfo_Adress_ShippingAdressId",
                table: "OrderInfo");

            migrationBuilder.DropTable(
                name: "OrderProduct");

            migrationBuilder.DropIndex(
                name: "IX_OrderInfo_FacturationAdressId",
                table: "OrderInfo");

            migrationBuilder.DropColumn(
                name: "FacturationAdressId",
                table: "OrderInfo");

            migrationBuilder.RenameColumn(
                name: "ShippingAdressId",
                table: "OrderInfo",
                newName: "AdressId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderInfo_ShippingAdressId",
                table: "OrderInfo",
                newName: "IX_OrderInfo_AdressId");

            migrationBuilder.AddColumn<string>(
                name: "ContactTelephone",
                table: "OrderInfo",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderInfo_Adress_AdressId",
                table: "OrderInfo",
                column: "AdressId",
                principalTable: "Adress",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
