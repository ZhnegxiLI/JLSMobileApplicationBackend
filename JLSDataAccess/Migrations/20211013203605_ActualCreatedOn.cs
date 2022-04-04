using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace JLSDataAccess.Migrations
{
    public partial class ActualCreatedOn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "QuantityPerParecel",
                table: "Product",
                newName: "QuantityPerParcel");

            migrationBuilder.AddColumn<DateTime>(
                name: "ActualCreatedOn",
                table: "Product",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ActualCreatedOn",
                table: "Product");

            migrationBuilder.RenameColumn(
                name: "QuantityPerParcel",
                table: "Product",
                newName: "QuantityPerParecel");
        }
    }
}