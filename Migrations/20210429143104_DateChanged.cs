using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace Servize.Migrations
{
    public partial class DateChanged : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ServiceRequestDateTime",
                table: "OrderSummary");

            migrationBuilder.AddColumn<DateTime>(
                name: "OrderPlacedDate",
                table: "OrderItem",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrderPlacedDate",
                table: "OrderItem");

            migrationBuilder.AddColumn<DateTime>(
                name: "ServiceRequestDateTime",
                table: "OrderSummary",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
