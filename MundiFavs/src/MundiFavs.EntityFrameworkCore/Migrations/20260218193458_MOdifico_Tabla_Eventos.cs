using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MundiFavs.Migrations
{
    /// <inheritdoc />
    public partial class MOdifico_Tabla_Eventos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AppPreferenciasNotificaciones_UserId",
                table: "AppPreferenciasNotificaciones");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AppNotificaciones",
                table: "AppNotificaciones");

            migrationBuilder.DropIndex(
                name: "IX_AppNotificaciones_UsuarioId",
                table: "AppNotificaciones");

            migrationBuilder.DropColumn(
                name: "CreationTime",
                table: "AppEventos");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "AppEventos");

            migrationBuilder.DropColumn(
                name: "FechaFin",
                table: "AppEventos");

            migrationBuilder.DropColumn(
                name: "LastModificationTime",
                table: "AppEventos");

            migrationBuilder.DropColumn(
                name: "LastModifierId",
                table: "AppEventos");

            migrationBuilder.RenameTable(
                name: "AppNotificaciones",
                newName: "Notificaciones");

            migrationBuilder.RenameColumn(
                name: "Titulo",
                table: "AppEventos",
                newName: "Url");

            migrationBuilder.RenameColumn(
                name: "Descripcion",
                table: "AppEventos",
                newName: "ImagenUrl");

            migrationBuilder.AlterColumn<string>(
                name: "Etiquetas",
                table: "AppExperiencias",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AddColumn<string>(
                name: "ConcurrencyStamp",
                table: "AppEventos",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ExternalId",
                table: "AppEventos",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ExtraProperties",
                table: "AppEventos",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Nombre",
                table: "AppEventos",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Comentario",
                table: "AppCalificaciones",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Notificaciones",
                table: "Notificaciones",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_AppPreferenciasNotificaciones_UserId",
                table: "AppPreferenciasNotificaciones",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AppEventos_AppDestinos_DestinoId",
                table: "AppEventos",
                column: "DestinoId",
                principalTable: "AppDestinos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppEventos_AppDestinos_DestinoId",
                table: "AppEventos");

            migrationBuilder.DropIndex(
                name: "IX_AppPreferenciasNotificaciones_UserId",
                table: "AppPreferenciasNotificaciones");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Notificaciones",
                table: "Notificaciones");

            migrationBuilder.DropColumn(
                name: "ConcurrencyStamp",
                table: "AppEventos");

            migrationBuilder.DropColumn(
                name: "ExternalId",
                table: "AppEventos");

            migrationBuilder.DropColumn(
                name: "ExtraProperties",
                table: "AppEventos");

            migrationBuilder.DropColumn(
                name: "Nombre",
                table: "AppEventos");

            migrationBuilder.RenameTable(
                name: "Notificaciones",
                newName: "AppNotificaciones");

            migrationBuilder.RenameColumn(
                name: "Url",
                table: "AppEventos",
                newName: "Titulo");

            migrationBuilder.RenameColumn(
                name: "ImagenUrl",
                table: "AppEventos",
                newName: "Descripcion");

            migrationBuilder.AlterColumn<string>(
                name: "Etiquetas",
                table: "AppExperiencias",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationTime",
                table: "AppEventos",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatorId",
                table: "AppEventos",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaFin",
                table: "AppEventos",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModificationTime",
                table: "AppEventos",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LastModifierId",
                table: "AppEventos",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Comentario",
                table: "AppCalificaciones",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(250)",
                oldMaxLength: 250,
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_AppNotificaciones",
                table: "AppNotificaciones",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_AppPreferenciasNotificaciones_UserId",
                table: "AppPreferenciasNotificaciones",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AppNotificaciones_UsuarioId",
                table: "AppNotificaciones",
                column: "UsuarioId");
        }
    }
}
