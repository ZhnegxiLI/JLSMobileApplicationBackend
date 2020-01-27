using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace JLSDataAccess.Migrations
{
    public partial class AddUserShippingAndUserPreferenceCategroy1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserPreferenceCategory",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedOn = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<long>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    UpdatedBy = table.Column<long>(nullable: true),
                    ReferenceCategoryId = table.Column<long>(nullable: false),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPreferenceCategory", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserPreferenceCategory_ReferenceCategory_ReferenceCategoryId",
                        column: x => x.ReferenceCategoryId,
                        principalTable: "ReferenceCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserPreferenceCategory_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserShippingAdress",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedOn = table.Column<DateTime>(nullable: true),
                    CreatedBy = table.Column<long>(nullable: true),
                    UpdatedOn = table.Column<DateTime>(nullable: true),
                    UpdatedBy = table.Column<long>(nullable: true),
                    ShippingName = table.Column<string>(nullable: true),
                    ShippingAdress = table.Column<string>(nullable: true),
                    ShippingTelephone = table.Column<string>(nullable: true),
                    ShippingContact = table.Column<string>(nullable: true),
                    Order = table.Column<int>(nullable: true),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserShippingAdress", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserShippingAdress_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserPreferenceCategory_ReferenceCategoryId",
                table: "UserPreferenceCategory",
                column: "ReferenceCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPreferenceCategory_UserId",
                table: "UserPreferenceCategory",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserShippingAdress_UserId",
                table: "UserShippingAdress",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserPreferenceCategory");

            migrationBuilder.DropTable(
                name: "UserShippingAdress");
        }
    }
}
