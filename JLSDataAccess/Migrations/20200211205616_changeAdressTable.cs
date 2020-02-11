using Microsoft.EntityFrameworkCore.Migrations;

namespace JLSDataAccess.Migrations
{
    public partial class changeAdressTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Adress_ReferenceItem_CountryId",
                table: "Adress");

            migrationBuilder.DropIndex(
                name: "IX_Adress_CountryId",
                table: "Adress");

            migrationBuilder.DropColumn(
                name: "CountryId",
                table: "Adress");

            migrationBuilder.RenameColumn(
                name: "StreeName",
                table: "Adress",
                newName: "ZipCode");

            migrationBuilder.RenameColumn(
                name: "PostCode",
                table: "Adress",
                newName: "SecondLineAddress");

            migrationBuilder.RenameColumn(
                name: "AdressDetail",
                table: "Adress",
                newName: "FirstLineAddress");

            migrationBuilder.AddColumn<string>(
                name: "Country",
                table: "Adress",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Country",
                table: "Adress");

            migrationBuilder.RenameColumn(
                name: "ZipCode",
                table: "Adress",
                newName: "StreeName");

            migrationBuilder.RenameColumn(
                name: "SecondLineAddress",
                table: "Adress",
                newName: "PostCode");

            migrationBuilder.RenameColumn(
                name: "FirstLineAddress",
                table: "Adress",
                newName: "AdressDetail");

            migrationBuilder.AddColumn<long>(
                name: "CountryId",
                table: "Adress",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Adress_CountryId",
                table: "Adress",
                column: "CountryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Adress_ReferenceItem_CountryId",
                table: "Adress",
                column: "CountryId",
                principalTable: "ReferenceItem",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
