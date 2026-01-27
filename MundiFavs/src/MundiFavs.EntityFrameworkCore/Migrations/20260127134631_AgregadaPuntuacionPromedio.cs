using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MundiFavs.Migrations
{
    /// <inheritdoc />
    public partial class AgregadaPuntuacionPromedio : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "PuntuacionPromedio",
                table: "AppDestinos",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PuntuacionPromedio",
                table: "AppDestinos");
        }
    }
}
