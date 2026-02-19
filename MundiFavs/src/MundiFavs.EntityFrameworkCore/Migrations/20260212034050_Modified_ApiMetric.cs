using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MundiFavs.Migrations
{
    /// <inheritdoc />
    public partial class Modified_ApiMetric : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AppApiMetrics_Endpoint",
                table: "AppApiMetrics");

            migrationBuilder.DropIndex(
                name: "IX_AppApiMetrics_Endpoint_RequestDateTime",
                table: "AppApiMetrics");

            migrationBuilder.DropIndex(
                name: "IX_AppApiMetrics_IsSuccess",
                table: "AppApiMetrics");

            migrationBuilder.DropIndex(
                name: "IX_AppApiMetrics_RequestDateTime",
                table: "AppApiMetrics");

            migrationBuilder.DropColumn(
                name: "ConcurrencyStamp",
                table: "AppApiMetrics");

            migrationBuilder.DropColumn(
                name: "CreationTime",
                table: "AppApiMetrics");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "AppApiMetrics");

            migrationBuilder.DropColumn(
                name: "ErrorMessage",
                table: "AppApiMetrics");

            migrationBuilder.DropColumn(
                name: "ErrorType",
                table: "AppApiMetrics");

            migrationBuilder.DropColumn(
                name: "HttpMethod",
                table: "AppApiMetrics");

            migrationBuilder.DropColumn(
                name: "RequestParameters",
                table: "AppApiMetrics");

            migrationBuilder.DropColumn(
                name: "RequestUrl",
                table: "AppApiMetrics");

            migrationBuilder.DropColumn(
                name: "ResponseTimeMs",
                table: "AppApiMetrics");

            migrationBuilder.DropColumn(
                name: "ResultCount",
                table: "AppApiMetrics");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "AppApiMetrics");

            migrationBuilder.RenameColumn(
                name: "RequestDateTime",
                table: "AppApiMetrics",
                newName: "ExecutionTime");

            migrationBuilder.RenameColumn(
                name: "ExtraProperties",
                table: "AppApiMetrics",
                newName: "ApiName");

            migrationBuilder.AlterColumn<string>(
                name: "Endpoint",
                table: "AppApiMetrics",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AddColumn<int>(
                name: "DurationMs",
                table: "AppApiMetrics",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DurationMs",
                table: "AppApiMetrics");

            migrationBuilder.RenameColumn(
                name: "ExecutionTime",
                table: "AppApiMetrics",
                newName: "RequestDateTime");

            migrationBuilder.RenameColumn(
                name: "ApiName",
                table: "AppApiMetrics",
                newName: "ExtraProperties");

            migrationBuilder.AlterColumn<string>(
                name: "Endpoint",
                table: "AppApiMetrics",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "ConcurrencyStamp",
                table: "AppApiMetrics",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationTime",
                table: "AppApiMetrics",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatorId",
                table: "AppApiMetrics",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ErrorMessage",
                table: "AppApiMetrics",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ErrorType",
                table: "AppApiMetrics",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HttpMethod",
                table: "AppApiMetrics",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RequestParameters",
                table: "AppApiMetrics",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RequestUrl",
                table: "AppApiMetrics",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "ResponseTimeMs",
                table: "AppApiMetrics",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<int>(
                name: "ResultCount",
                table: "AppApiMetrics",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "AppApiMetrics",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_AppApiMetrics_Endpoint",
                table: "AppApiMetrics",
                column: "Endpoint");

            migrationBuilder.CreateIndex(
                name: "IX_AppApiMetrics_Endpoint_RequestDateTime",
                table: "AppApiMetrics",
                columns: new[] { "Endpoint", "RequestDateTime" });

            migrationBuilder.CreateIndex(
                name: "IX_AppApiMetrics_IsSuccess",
                table: "AppApiMetrics",
                column: "IsSuccess");

            migrationBuilder.CreateIndex(
                name: "IX_AppApiMetrics_RequestDateTime",
                table: "AppApiMetrics",
                column: "RequestDateTime");
        }
    }
}
