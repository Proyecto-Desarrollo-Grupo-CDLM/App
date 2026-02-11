using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MundiFavs.Migrations
{
    /// <inheritdoc />
    public partial class ModifiedDestino : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExternalId",
                table: "AppDestinos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExternalId",
                table: "AppDestinos");
        }
    }
}
