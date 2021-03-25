using Microsoft.EntityFrameworkCore.Migrations;

namespace Servize.Migrations
{
    public partial class ChangesInProduct : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Amount",
                table: "CartItem");

            migrationBuilder.RenameColumn(
                name: "ServiceName",
                table: "Product",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "ProductName",
                table: "Product",
                newName: "Description");

            migrationBuilder.AlterColumn<string>(
                name: "Discount",
                table: "Product",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AddColumn<decimal>(
                name: "Quantity",
                table: "Product",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Total",
                table: "Product",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "UnitPrice",
                table: "Product",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "VAT",
                table: "Product",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "Total",
                table: "CartItem",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "Total",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "UnitPrice",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "VAT",
                table: "Product");

            migrationBuilder.DropColumn(
                name: "Total",
                table: "CartItem");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Product",
                newName: "ServiceName");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Product",
                newName: "ProductName");

            migrationBuilder.AlterColumn<double>(
                name: "Discount",
                table: "Product",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Amount",
                table: "CartItem",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
