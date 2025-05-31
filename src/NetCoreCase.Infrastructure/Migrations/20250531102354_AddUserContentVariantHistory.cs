using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NetCoreCase.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserContentVariantHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserContentVariantHistories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ContentId = table.Column<Guid>(type: "uuid", nullable: false),
                    VariantId = table.Column<Guid>(type: "uuid", nullable: false),
                    ViewedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastAccessedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ViewCount = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserContentVariantHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserContentVariantHistories_ContentVariants_VariantId",
                        column: x => x.VariantId,
                        principalTable: "ContentVariants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserContentVariantHistories_Contents_ContentId",
                        column: x => x.ContentId,
                        principalTable: "Contents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserContentVariantHistories_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserContentVariantHistories_ContentId",
                table: "UserContentVariantHistories",
                column: "ContentId");

            migrationBuilder.CreateIndex(
                name: "IX_UserContentVariantHistories_LastAccessedAt",
                table: "UserContentVariantHistories",
                column: "LastAccessedAt");

            migrationBuilder.CreateIndex(
                name: "IX_UserContentVariantHistories_UserId",
                table: "UserContentVariantHistories",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserContentVariantHistories_UserId_ContentId",
                table: "UserContentVariantHistories",
                columns: new[] { "UserId", "ContentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserContentVariantHistories_VariantId",
                table: "UserContentVariantHistories",
                column: "VariantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserContentVariantHistories");
        }
    }
}
