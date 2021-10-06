using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SeleniumDotNet.Migrations
{
    public partial class Second : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Processed",
                table: "ReviewUrl",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "_Created",
                table: "ReviewUrl",
                nullable: false,
                defaultValueSql: "GETUTCDATE()");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Processed",
                table: "ReviewUrl");

            migrationBuilder.DropColumn(
                name: "_Created",
                table: "ReviewUrl");
        }
    }
}
