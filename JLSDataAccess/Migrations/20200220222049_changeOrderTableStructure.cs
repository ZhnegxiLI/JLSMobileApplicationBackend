using Microsoft.EntityFrameworkCore.Migrations;

namespace JLSDataAccess.Migrations
{
    public partial class changeOrderTableStructure : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderProduct_Product_ProductId",
                table: "OrderProduct");

            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "OrderProduct",
                newName: "ReferenceId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderProduct_ProductId",
                table: "OrderProduct",
                newName: "IX_OrderProduct_ReferenceId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderProduct_ReferenceItem_ReferenceId",
                table: "OrderProduct",
                column: "ReferenceId",
                principalTable: "ReferenceItem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderProduct_ReferenceItem_ReferenceId",
                table: "OrderProduct");

            migrationBuilder.RenameColumn(
                name: "ReferenceId",
                table: "OrderProduct",
                newName: "ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_OrderProduct_ReferenceId",
                table: "OrderProduct",
                newName: "IX_OrderProduct_ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderProduct_Product_ProductId",
                table: "OrderProduct",
                column: "ProductId",
                principalTable: "Product",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}