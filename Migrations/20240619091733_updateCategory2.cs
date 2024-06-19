using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace amazon_backend.Migrations
{
    /// <inheritdoc />
    public partial class updateCategory2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_CategoryImages_CategoryImageId",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Categories_CategoryImageId",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "CategoryImageId",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "Image",
                table: "Categories");

            migrationBuilder.AlterColumn<string>(
                name: "Logo",
                table: "Categories",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext");

            migrationBuilder.AddColumn<Guid>(
                name: "ImageId",
                table: "Categories",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Categories_ImageId",
                table: "Categories",
                column: "ImageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_CategoryImages_ImageId",
                table: "Categories",
                column: "ImageId",
                principalTable: "CategoryImages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_CategoryImages_ImageId",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Categories_ImageId",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "ImageId",
                table: "Categories");

            migrationBuilder.AlterColumn<string>(
                name: "Logo",
                table: "Categories",
                type: "longtext",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CategoryImageId",
                table: "Categories",
                type: "char(36)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "Categories",
                type: "varchar(255)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_CategoryImageId",
                table: "Categories",
                column: "CategoryImageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_CategoryImages_CategoryImageId",
                table: "Categories",
                column: "CategoryImageId",
                principalTable: "CategoryImages",
                principalColumn: "Id");
        }
    }
}
