using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace amazon_backend.Migrations
{
    /// <inheritdoc />
    public partial class Category : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CategoryCategoryPropertyKey");

            migrationBuilder.AddColumn<uint>(
                name: "CategoryId",
                table: "CategoryPropertyKeys",
                type: "int unsigned",
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<string>(
                name: "NameCategory",
                table: "CategoryPropertyKeys",
                type: "longtext",
                nullable: false);

            migrationBuilder.AddColumn<Guid>(
                name: "CategoryPropertyKeyId",
                table: "Categories",
                type: "char(36)",
                nullable: true);

            migrationBuilder.AddColumn<uint>(
                name: "Logo",
                table: "Categories",
                type: "int unsigned",
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<string>(
                name: "ParentCategoryName",
                table: "Categories",
                type: "longtext",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CategoryPropertyKeys_CategoryId",
                table: "CategoryPropertyKeys",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_CategoryPropertyKeyId",
                table: "Categories",
                column: "CategoryPropertyKeyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_CategoryPropertyKeys_CategoryPropertyKeyId",
                table: "Categories",
                column: "CategoryPropertyKeyId",
                principalTable: "CategoryPropertyKeys",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CategoryPropertyKeys_Categories_CategoryId",
                table: "CategoryPropertyKeys",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_CategoryPropertyKeys_CategoryPropertyKeyId",
                table: "Categories");

            migrationBuilder.DropForeignKey(
                name: "FK_CategoryPropertyKeys_Categories_CategoryId",
                table: "CategoryPropertyKeys");

            migrationBuilder.DropIndex(
                name: "IX_CategoryPropertyKeys_CategoryId",
                table: "CategoryPropertyKeys");

            migrationBuilder.DropIndex(
                name: "IX_Categories_CategoryPropertyKeyId",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "CategoryPropertyKeys");

            migrationBuilder.DropColumn(
                name: "NameCategory",
                table: "CategoryPropertyKeys");

            migrationBuilder.DropColumn(
                name: "CategoryPropertyKeyId",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "Logo",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "ParentCategoryName",
                table: "Categories");

            migrationBuilder.CreateTable(
                name: "CategoryCategoryPropertyKey",
                columns: table => new
                {
                    CategoriesId = table.Column<uint>(type: "int unsigned", nullable: false),
                    CategoryPropertyKeysId = table.Column<Guid>(type: "char(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryCategoryPropertyKey", x => new { x.CategoriesId, x.CategoryPropertyKeysId });
                    table.ForeignKey(
                        name: "FK_CategoryCategoryPropertyKey_Categories_CategoriesId",
                        column: x => x.CategoriesId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CategoryCategoryPropertyKey_CategoryPropertyKeys_CategoryPro~",
                        column: x => x.CategoryPropertyKeysId,
                        principalTable: "CategoryPropertyKeys",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_CategoryCategoryPropertyKey_CategoryPropertyKeysId",
                table: "CategoryCategoryPropertyKey",
                column: "CategoryPropertyKeysId");
        }
    }
}
