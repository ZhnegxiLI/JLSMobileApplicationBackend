using Microsoft.EntityFrameworkCore.Migrations;

namespace JLSDataAccess.Migrations
{
    public partial class initial1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReferenceLabel_ReferenceItem_ReferenceItemId1",
                table: "ReferenceLabel");

            migrationBuilder.DropIndex(
                name: "IX_ReferenceLabel_ReferenceItemId1",
                table: "ReferenceLabel");

            migrationBuilder.DropColumn(
                name: "ReferenceItemId1",
                table: "ReferenceLabel");

            migrationBuilder.AlterColumn<long>(
                name: "ReferenceItemId",
                table: "ReferenceLabel",
                nullable: false,
                oldClrType: typeof(int));

            migrationBuilder.CreateIndex(
                name: "IX_ReferenceLabel_ReferenceItemId",
                table: "ReferenceLabel",
                column: "ReferenceItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_ReferenceLabel_ReferenceItem_ReferenceItemId",
                table: "ReferenceLabel",
                column: "ReferenceItemId",
                principalTable: "ReferenceItem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReferenceLabel_ReferenceItem_ReferenceItemId",
                table: "ReferenceLabel");

            migrationBuilder.DropIndex(
                name: "IX_ReferenceLabel_ReferenceItemId",
                table: "ReferenceLabel");

            migrationBuilder.AlterColumn<int>(
                name: "ReferenceItemId",
                table: "ReferenceLabel",
                nullable: false,
                oldClrType: typeof(long));

            migrationBuilder.AddColumn<long>(
                name: "ReferenceItemId1",
                table: "ReferenceLabel",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ReferenceLabel_ReferenceItemId1",
                table: "ReferenceLabel",
                column: "ReferenceItemId1");

            migrationBuilder.AddForeignKey(
                name: "FK_ReferenceLabel_ReferenceItem_ReferenceItemId1",
                table: "ReferenceLabel",
                column: "ReferenceItemId1",
                principalTable: "ReferenceItem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
