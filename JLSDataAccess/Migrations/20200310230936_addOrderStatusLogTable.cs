using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace JLSDataAccess.Migrations
{
    public partial class addOrderStatusLogTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "TotalPrice",
                table: "OrderProduct",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "UnitPrice",
                table: "OrderProduct",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "OrderInfoStatusLog",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedOn = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<long>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    UpdatedBy = table.Column<long>(nullable: true),
                    OrderInfoId = table.Column<long>(nullable: false),
                    OldStatusId = table.Column<long>(nullable: true),
                    NewStatusId = table.Column<long>(nullable: true),
                    ActionTime = table.Column<DateTime>(nullable: true),
                    UserId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderInfoStatusLog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderInfoStatusLog_OrderInfo_OrderInfoId",
                        column: x => x.OrderInfoId,
                        principalTable: "OrderInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderInfoStatusLog_OrderInfoId",
                table: "OrderInfoStatusLog",
                column: "OrderInfoId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrderInfoStatusLog");

            migrationBuilder.DropColumn(
                name: "TotalPrice",
                table: "OrderProduct");

            migrationBuilder.DropColumn(
                name: "UnitPrice",
                table: "OrderProduct");
        }
    }
}