using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace amazon_backend.Migrations
{
    /// <inheritdoc />
    public partial class FixCategoryProps : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<uint>(
                name: "CategoryId",
                table: "CategoryPropertyKeys",
                type: "int unsigned",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "char(36)");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "CategoryPropertyKeys",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsFilter",
                table: "CategoryPropertyKeys",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsRequired",
                table: "CategoryPropertyKeys",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "CategoryPropertyKeys");

            migrationBuilder.DropColumn(
                name: "IsFilter",
                table: "CategoryPropertyKeys");

            migrationBuilder.DropColumn(
                name: "IsRequired",
                table: "CategoryPropertyKeys");

            migrationBuilder.AlterColumn<Guid>(
                name: "CategoryId",
                table: "CategoryPropertyKeys",
                type: "char(36)",
                nullable: false,
                oldClrType: typeof(uint),
                oldType: "int unsigned");
        }
    }
}
