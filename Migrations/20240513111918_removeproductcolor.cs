using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace amazon_backend.Migrations
{
    /// <inheritdoc />
    public partial class removeproductcolor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductColors");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductColors",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false),
                    ProductId = table.Column<Guid>(type: "char(36)", nullable: false),
                    AlbumReference = table.Column<string>(type: "varchar(255)", nullable: false),
                    HashColor = table.Column<string>(type: "varchar(255)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Name = table.Column<string>(type: "varchar(255)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductColors", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductColors_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_ProductColors_ProductId",
                table: "ProductColors",
                column: "ProductId");
        }
    }
}
