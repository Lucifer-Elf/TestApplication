using Microsoft.EntityFrameworkCore.Migrations;

namespace Servize.Migrations
{
    public partial class Test2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserClient_OrderSummary_OrderId",
                table: "UserClient");

            migrationBuilder.DropIndex(
                name: "IX_UserClient_OrderId",
                table: "UserClient");

            migrationBuilder.DropColumn(
                name: "OrderId",
                table: "UserClient");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OrderId",
                table: "UserClient",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_UserClient_OrderId",
                table: "UserClient",
                column: "OrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserClient_OrderSummary_OrderId",
                table: "UserClient",
                column: "OrderId",
                principalTable: "OrderSummary",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
