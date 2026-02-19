using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MundiFavs.Migrations
{
    /// <inheritdoc />
    public partial class Agregar_Tablas_Notificaciones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Link",
                table: "AppNotificaciones");

            migrationBuilder.DropColumn(
                name: "Mensaje",
                table: "AppNotificaciones");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "AppNotificaciones",
                newName: "UsuarioId");

            migrationBuilder.RenameColumn(
                name: "Titulo",
                table: "AppNotificaciones",
                newName: "TituloDestino");

            migrationBuilder.RenameColumn(
                name: "Tipo",
                table: "AppNotificaciones",
                newName: "CambioDetectado");

            migrationBuilder.RenameIndex(
                name: "IX_AppNotificaciones_UserId",
                table: "AppNotificaciones",
                newName: "IX_AppNotificaciones_UsuarioId");

            migrationBuilder.AddColumn<DateTime>(
                name: "Fecha",
                table: "AppNotificaciones",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<TimeSpan>(
                name: "Hora",
                table: "AppNotificaciones",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Fecha",
                table: "AppNotificaciones");

            migrationBuilder.DropColumn(
                name: "Hora",
                table: "AppNotificaciones");

            migrationBuilder.RenameColumn(
                name: "UsuarioId",
                table: "AppNotificaciones",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "TituloDestino",
                table: "AppNotificaciones",
                newName: "Titulo");

            migrationBuilder.RenameColumn(
                name: "CambioDetectado",
                table: "AppNotificaciones",
                newName: "Tipo");

            migrationBuilder.RenameIndex(
                name: "IX_AppNotificaciones_UsuarioId",
                table: "AppNotificaciones",
                newName: "IX_AppNotificaciones_UserId");

            migrationBuilder.AddColumn<string>(
                name: "Link",
                table: "AppNotificaciones",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Mensaje",
                table: "AppNotificaciones",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
