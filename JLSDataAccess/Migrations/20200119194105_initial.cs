using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace JLSDataAccess.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DiscountActivity",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: false),
                    Title = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    DiscountPercentage = table.Column<float>(nullable: false),
                    Validity = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscountActivity", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReferenceCategory",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: false),
                    ShortLabel = table.Column<string>(nullable: true),
                    LongLabel = table.Column<string>(nullable: true),
                    Validity = table.Column<bool>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReferenceCategory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ReferenceItem",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: false),
                    Code = table.Column<string>(nullable: true),
                    ParentId = table.Column<long>(nullable: true),
                    Value = table.Column<string>(nullable: true),
                    Order = table.Column<int>(nullable: true),
                    Validity = table.Column<bool>(nullable: true),
                    ReferenceCategoryId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReferenceItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReferenceItem_ReferenceCategory_ReferenceCategoryId",
                        column: x => x.ReferenceCategoryId,
                        principalTable: "ReferenceCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderInfo",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: false),
                    OrderReferenceCode = table.Column<string>(nullable: true),
                    ContactTelephone = table.Column<string>(nullable: true),
                    PaymentInfo = table.Column<string>(nullable: true),
                    ClientRemark = table.Column<string>(nullable: true),
                    AdminRemark = table.Column<string>(nullable: true),
                    TotalPrice = table.Column<float>(nullable: true),
                    TaxRate = table.Column<float>(nullable: true),
                    StatusReferenceItemId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderInfo", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderInfo_ReferenceItem_StatusReferenceItemId",
                        column: x => x.StatusReferenceItemId,
                        principalTable: "ReferenceItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Product",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: false),
                    ProductReferenceCode = table.Column<string>(nullable: true),
                    Price = table.Column<float>(nullable: true),
                    QuantityPerBox = table.Column<int>(nullable: true),
                    MinQuantity = table.Column<int>(nullable: true),
                    Color = table.Column<string>(nullable: true),
                    Material = table.Column<string>(nullable: true),
                    Size = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Validity = table.Column<bool>(nullable: true),
                    ReferenceItemId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Product", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Product_ReferenceItem_ReferenceItemId",
                        column: x => x.ReferenceItemId,
                        principalTable: "ReferenceItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReferenceLabel",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: false),
                    Label = table.Column<string>(nullable: true),
                    Lang = table.Column<string>(nullable: true),
                    ReferenceItemId = table.Column<int>(nullable: false),
                    ReferenceItemId1 = table.Column<long>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReferenceLabel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReferenceLabel_ReferenceItem_ReferenceItemId1",
                        column: x => x.ReferenceItemId1,
                        principalTable: "ReferenceItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrderInfoLog",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: false),
                    ChangedDescription = table.Column<string>(nullable: true),
                    OrderInfoId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderInfoLog", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderInfoLog_OrderInfo_OrderInfoId",
                        column: x => x.OrderInfoId,
                        principalTable: "OrderInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderInfoShipping",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: false),
                    ShippingTelephone = table.Column<string>(nullable: true),
                    ShippingCity = table.Column<string>(nullable: true),
                    ShippingPostCode = table.Column<string>(nullable: true),
                    ShippingAdress = table.Column<string>(nullable: true),
                    ShippingAdressDetail = table.Column<string>(nullable: true),
                    CountryReferenceItemId = table.Column<long>(nullable: false),
                    OrderInfoId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderInfoShipping", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderInfoShipping_ReferenceItem_CountryReferenceItemId",
                        column: x => x.CountryReferenceItemId,
                        principalTable: "ReferenceItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderInfoShipping_OrderInfo_OrderInfoId",
                        column: x => x.OrderInfoId,
                        principalTable: "OrderInfo",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "DiscountActivityProduct",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: false),
                    DiscountActivityId = table.Column<long>(nullable: false),
                    ProductId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DiscountActivityProduct", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DiscountActivityProduct_DiscountActivity_DiscountActivityId",
                        column: x => x.DiscountActivityId,
                        principalTable: "DiscountActivity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DiscountActivityProduct_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductPhotoPath",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    CreatedOn = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<long>(nullable: false),
                    UpdatedOn = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<long>(nullable: false),
                    Path = table.Column<string>(nullable: true),
                    ProductId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductPhotoPath", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductPhotoPath_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DiscountActivityProduct_DiscountActivityId",
                table: "DiscountActivityProduct",
                column: "DiscountActivityId");

            migrationBuilder.CreateIndex(
                name: "IX_DiscountActivityProduct_ProductId",
                table: "DiscountActivityProduct",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderInfo_StatusReferenceItemId",
                table: "OrderInfo",
                column: "StatusReferenceItemId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderInfoLog_OrderInfoId",
                table: "OrderInfoLog",
                column: "OrderInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderInfoShipping_CountryReferenceItemId",
                table: "OrderInfoShipping",
                column: "CountryReferenceItemId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderInfoShipping_OrderInfoId",
                table: "OrderInfoShipping",
                column: "OrderInfoId");

            migrationBuilder.CreateIndex(
                name: "IX_Product_ReferenceItemId",
                table: "Product",
                column: "ReferenceItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductPhotoPath_ProductId",
                table: "ProductPhotoPath",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_ReferenceItem_ReferenceCategoryId",
                table: "ReferenceItem",
                column: "ReferenceCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_ReferenceLabel_ReferenceItemId1",
                table: "ReferenceLabel",
                column: "ReferenceItemId1");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DiscountActivityProduct");

            migrationBuilder.DropTable(
                name: "OrderInfoLog");

            migrationBuilder.DropTable(
                name: "OrderInfoShipping");

            migrationBuilder.DropTable(
                name: "ProductPhotoPath");

            migrationBuilder.DropTable(
                name: "ReferenceLabel");

            migrationBuilder.DropTable(
                name: "DiscountActivity");

            migrationBuilder.DropTable(
                name: "OrderInfo");

            migrationBuilder.DropTable(
                name: "Product");

            migrationBuilder.DropTable(
                name: "ReferenceItem");

            migrationBuilder.DropTable(
                name: "ReferenceCategory");
        }
    }
}
