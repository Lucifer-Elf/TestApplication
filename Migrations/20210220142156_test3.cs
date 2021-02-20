using Microsoft.EntityFrameworkCore.Migrations;

namespace Servize.Migrations
{
    public partial class test3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServizeCategory_ServizeProvider_ServizeProviderId",
                table: "ServizeCategory");

            migrationBuilder.DropForeignKey(
                name: "FK_ServizeSubCategory_ServizeCategory_ServizeCategoryId",
                table: "ServizeSubCategory");

            migrationBuilder.DropIndex(
                name: "IX_ServizeCategory_ServizeProviderId",
                table: "ServizeCategory");

            migrationBuilder.DropColumn(
                name: "PickAndDrop",
                table: "ServizeCategory");

            migrationBuilder.DropColumn(
                name: "ServizeProviderId",
                table: "ServizeCategory");

            migrationBuilder.AddColumn<string>(
                name: "ProfilePicture",
                table: "UserClient",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ServizeCategoryId",
                table: "ServizeSubCategory",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "ServizeProvider",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "ServizeProvider",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "ProviderId",
                table: "ServizeCategory",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ServizeCategory_ProviderId",
                table: "ServizeCategory",
                column: "ProviderId");

            migrationBuilder.AddForeignKey(
                name: "FK_ServizeCategory_ServizeProvider_ProviderId",
                table: "ServizeCategory",
                column: "ProviderId",
                principalTable: "ServizeProvider",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServizeSubCategory_ServizeCategory_ServizeCategoryId",
                table: "ServizeSubCategory",
                column: "ServizeCategoryId",
                principalTable: "ServizeCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ServizeCategory_ServizeProvider_ProviderId",
                table: "ServizeCategory");

            migrationBuilder.DropForeignKey(
                name: "FK_ServizeSubCategory_ServizeCategory_ServizeCategoryId",
                table: "ServizeSubCategory");

            migrationBuilder.DropIndex(
                name: "IX_ServizeCategory_ProviderId",
                table: "ServizeCategory");

            migrationBuilder.DropColumn(
                name: "ProfilePicture",
                table: "UserClient");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "ServizeProvider");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "ServizeProvider");

            migrationBuilder.DropColumn(
                name: "ProviderId",
                table: "ServizeCategory");

            migrationBuilder.AlterColumn<int>(
                name: "ServizeCategoryId",
                table: "ServizeSubCategory",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<bool>(
                name: "PickAndDrop",
                table: "ServizeCategory",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ServizeProviderId",
                table: "ServizeCategory",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServizeCategory_ServizeProviderId",
                table: "ServizeCategory",
                column: "ServizeProviderId");

            migrationBuilder.AddForeignKey(
                name: "FK_ServizeCategory_ServizeProvider_ServizeProviderId",
                table: "ServizeCategory",
                column: "ServizeProviderId",
                principalTable: "ServizeProvider",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ServizeSubCategory_ServizeCategory_ServizeCategoryId",
                table: "ServizeSubCategory",
                column: "ServizeCategoryId",
                principalTable: "ServizeCategory",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
