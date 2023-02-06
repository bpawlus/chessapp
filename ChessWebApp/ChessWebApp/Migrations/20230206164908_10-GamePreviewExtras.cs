using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChessWebApp.Migrations
{
    /// <inheritdoc />
    public partial class _10GamePreviewExtras : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TimeStamp",
                table: "GameEvent");

            migrationBuilder.AddColumn<DateTime>(
                name: "Time",
                table: "GameEvent",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TimeEnd",
                table: "Game",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TimeStart",
                table: "Game",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Time",
                table: "GameEvent");

            migrationBuilder.DropColumn(
                name: "TimeEnd",
                table: "Game");

            migrationBuilder.DropColumn(
                name: "TimeStart",
                table: "Game");

            migrationBuilder.AddColumn<byte[]>(
                name: "TimeStamp",
                table: "GameEvent",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);
        }
    }
}
