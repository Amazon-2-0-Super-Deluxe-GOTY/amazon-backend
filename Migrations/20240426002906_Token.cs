using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace amazon_backend.Migrations
{
    /// <inheritdoc />
    public partial class Token : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tokens",
                columns: table => new
                {
                    token_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    token = table.Column<string>(type: "longtext", nullable: false),
                    expiration_date = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tokens", x => x.token_id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "token_journals",
                columns: table => new
                {
                    token_journal_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    token_id = table.Column<Guid>(type: "char(36)", nullable: false),
                    user_id = table.Column<Guid>(type: "char(36)", nullable: true),
                    is_active = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    activated_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    deactivated_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_token_journals", x => x.token_journal_id);
                    table.ForeignKey(
                        name: "FK_token_journals_Users_user_id",
                        column: x => x.user_id,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_token_journals_tokens_token_id",
                        column: x => x.token_id,
                        principalTable: "tokens",
                        principalColumn: "token_id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_token_journals_token_id",
                table: "token_journals",
                column: "token_id");

            migrationBuilder.CreateIndex(
                name: "IX_token_journals_user_id",
                table: "token_journals",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "token_journals");

            migrationBuilder.DropTable(
                name: "tokens");
        }
    }
}
