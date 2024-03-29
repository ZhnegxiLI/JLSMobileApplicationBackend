﻿using Microsoft.EntityFrameworkCore.Migrations;

namespace JLSDataAccess.Migrations
{
    public partial class AddVisitorCounterPerDay : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Day",
                table: "VisitorCounter",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Day",
                table: "VisitorCounter");
        }
    }
}
