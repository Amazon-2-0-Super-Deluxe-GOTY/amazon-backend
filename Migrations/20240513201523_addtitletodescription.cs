using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace amazon_backend.Migrations
{
    /// <inheritdoc />
    public partial class addtitletodescription : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "AboutProductItems",
                type: "varchar(255)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Title",
                table: "AboutProductItems");
        }
    }
}
