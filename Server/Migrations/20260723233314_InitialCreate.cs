using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Server.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MediaFiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FilePath = table.Column<string>(type: "TEXT", nullable: false),
                    FileName = table.Column<string>(type: "TEXT", nullable: false),
                    FileSizeBytes = table.Column<long>(type: "INTEGER", nullable: false),
                    Container = table.Column<string>(type: "TEXT", nullable: true),
                    VideoCodec = table.Column<string>(type: "TEXT", nullable: true),
                    DolbyVisionProfile = table.Column<int>(type: "INTEGER", nullable: true),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    DiscoveredAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastScannedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MediaFiles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Jobs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MediaFileId = table.Column<int>(type: "INTEGER", nullable: false),
                    Type = table.Column<string>(type: "TEXT", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: false),
                    StartedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CompletedAtUtc = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ErrorMessage = table.Column<string>(type: "TEXT", nullable: true),
                    OriginalSizeBytes = table.Column<long>(type: "INTEGER", nullable: true),
                    ResultSizeBytes = table.Column<long>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jobs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Jobs_MediaFiles_MediaFileId",
                        column: x => x.MediaFileId,
                        principalTable: "MediaFiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_MediaFileId",
                table: "Jobs",
                column: "MediaFileId");

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_Status",
                table: "Jobs",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_MediaFiles_FilePath",
                table: "MediaFiles",
                column: "FilePath",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Jobs");

            migrationBuilder.DropTable(
                name: "MediaFiles");
        }
    }
}
