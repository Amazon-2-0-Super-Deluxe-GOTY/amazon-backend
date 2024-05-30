using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace amazon_backend.Migrations
{
    /// <inheritdoc />
    public partial class ReviewImagesManyToMany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReviewImages_Reviews_ReviewId",
                table: "ReviewImages");

            migrationBuilder.DropIndex(
                name: "IX_ReviewImages_ReviewId",
                table: "ReviewImages");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "ReviewImages",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "ReviewReviewImage",
                columns: table => new
                {
                    ReviewId = table.Column<Guid>(type: "char(36)", nullable: false),
                    ReviewImagesId = table.Column<Guid>(type: "char(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewReviewImage", x => new { x.ReviewId, x.ReviewImagesId });
                    table.ForeignKey(
                        name: "FK_ReviewReviewImage_ReviewImages_ReviewImagesId",
                        column: x => x.ReviewImagesId,
                        principalTable: "ReviewImages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReviewReviewImage_Reviews_ReviewId",
                        column: x => x.ReviewId,
                        principalTable: "Reviews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewImages_UserId",
                table: "ReviewImages",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewReviewImage_ReviewImagesId",
                table: "ReviewReviewImage",
                column: "ReviewImagesId");

            migrationBuilder.AddForeignKey(
                name: "FK_ReviewImages_Users_UserId",
                table: "ReviewImages",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReviewImages_Users_UserId",
                table: "ReviewImages");

            migrationBuilder.DropTable(
                name: "ReviewReviewImage");

            migrationBuilder.DropIndex(
                name: "IX_ReviewImages_UserId",
                table: "ReviewImages");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ReviewImages");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewImages_ReviewId",
                table: "ReviewImages",
                column: "ReviewId");

            migrationBuilder.AddForeignKey(
                name: "FK_ReviewImages_Reviews_ReviewId",
                table: "ReviewImages",
                column: "ReviewId",
                principalTable: "Reviews",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
