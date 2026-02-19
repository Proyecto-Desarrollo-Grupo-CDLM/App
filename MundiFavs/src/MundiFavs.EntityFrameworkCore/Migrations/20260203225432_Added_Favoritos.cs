using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MundiFavs.Migrations
{
    /// <inheritdoc />
    public partial class Added_Favoritos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppFavoritos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DestinoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppFavoritos", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AppFavoritos_CreatorId_DestinoId",
                table: "AppFavoritos",
                columns: new[] { "CreatorId", "DestinoId" },
                unique: true,
                filter: "[CreatorId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppFavoritos");
        }
    }
}
