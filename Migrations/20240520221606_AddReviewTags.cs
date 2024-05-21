using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace amazon_backend.Migrations
{
    /// <inheritdoc />
    public partial class AddReviewTags : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ReviewTags",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    Name = table.Column<string>(type: "varchar(255)", nullable: false),
                    Description = table.Column<string>(type: "varchar(255)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewTags", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ReviewReviewTag",
                columns: table => new
                {
                    ReviewTagsId = table.Column<Guid>(type: "char(36)", nullable: false),
                    ReviewsId = table.Column<Guid>(type: "char(36)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReviewReviewTag", x => new { x.ReviewTagsId, x.ReviewsId });
                    table.ForeignKey(
                        name: "FK_ReviewReviewTag_ReviewTags_ReviewTagsId",
                        column: x => x.ReviewTagsId,
                        principalTable: "ReviewTags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReviewReviewTag_Reviews_ReviewsId",
                        column: x => x.ReviewsId,
                        principalTable: "Reviews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_ReviewReviewTag_ReviewsId",
                table: "ReviewReviewTag",
                column: "ReviewsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReviewReviewTag");

            migrationBuilder.DropTable(
                name: "ReviewTags");
        }
    }
}
