using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace amazon_backend.Migrations
{
    /// <inheritdoc />
    public partial class UserCategoryPropKeyRefactor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_CategoryPropertyKeys_CategoryPropertyKeyId",
                table: "Categories");

            migrationBuilder.DropForeignKey(
                name: "FK_ClientProfiles_Users_UserId",
                table: "ClientProfiles");

            migrationBuilder.DropIndex(
                name: "IX_ClientProfiles_UserId",
                table: "ClientProfiles");

            migrationBuilder.DropIndex(
                name: "IX_Categories_CategoryPropertyKeyId",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "CategoryPropertyKeys");

            migrationBuilder.DropColumn(
                name: "CategoryPropertyKeyId",
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CategoryCategoryPropertyKey");

            migrationBuilder.AddColumn<uint>(
                name: "CategoryId",
                table: "CategoryPropertyKeys",
                type: "int unsigned",
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<Guid>(
                name: "CategoryPropertyKeyId",
                table: "Categories",
                type: "char(36)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ClientProfiles_UserId",
                table: "ClientProfiles",
                column: "UserId",
                unique: true);

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
                name: "FK_ClientProfiles_Users_UserId",
                table: "ClientProfiles",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
