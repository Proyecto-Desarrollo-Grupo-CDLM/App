using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MundiFavs.Migrations
{
    /// <inheritdoc />
    public partial class AddApiMetricsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppApiMetrics",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Endpoint = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    HttpMethod = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    RequestUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ResponseTimeMs = table.Column<long>(type: "bigint", nullable: false),
                    StatusCode = table.Column<int>(type: "int", nullable: false),
                    IsSuccess = table.Column<bool>(type: "bit", nullable: false),
                    ErrorMessage = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    ErrorType = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    RequestDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ResultCount = table.Column<int>(type: "int", nullable: true),
                    RequestParameters = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    ExtraProperties = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppApiMetrics", x => x.Id);
                });

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppApiMetrics");
        }
    }
}
