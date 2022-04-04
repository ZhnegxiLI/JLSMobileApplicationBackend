using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace JLSDataAccess.Migrations
{
    public partial class modifyOrderTableStructure : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdminRemark",
                table: "OrderInfo");

            migrationBuilder.DropColumn(
                name: "ClientRemark",
                table: "OrderInfo");

            migrationBuilder.AddColumn<long>(
                name: "AdminRemarkId",
                table: "OrderInfo",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ClientRemarkId",
                table: "OrderInfo",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ShipmentInfoId",
                table: "OrderInfo",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Remark",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedOn = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<long>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    UpdatedBy = table.Column<long>(nullable: true),
                    UserId = table.Column<int>(nullable: true),
                    Text = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Remark", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShipmentInfo",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedOn = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<long>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    UpdatedBy = table.Column<long>(nullable: true),
                    ShipmentNumber = table.Column<string>(nullable: true),
                    Weight = table.Column<string>(nullable: true),
                    Fee = table.Column<float>(nullable: true),
                    Date = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShipmentInfo", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Remark");

            migrationBuilder.DropTable(
                name: "ShipmentInfo");

            migrationBuilder.DropColumn(
                name: "AdminRemarkId",
                table: "OrderInfo");

            migrationBuilder.DropColumn(
                name: "ClientRemarkId",
                table: "OrderInfo");

            migrationBuilder.DropColumn(
                name: "ShipmentInfoId",
                table: "OrderInfo");

            migrationBuilder.AddColumn<string>(
                name: "AdminRemark",
                table: "OrderInfo",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClientRemark",
                table: "OrderInfo",
                nullable: true);
        }
    }
}
