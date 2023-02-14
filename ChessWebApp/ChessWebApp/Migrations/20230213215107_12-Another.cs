using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChessWebApp.Migrations
{
    /// <inheritdoc />
    public partial class _12Another : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "GameEvent",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_GameEvent_UserId",
                table: "GameEvent",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_GameEvent_User_UserId",
                table: "GameEvent",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GameEvent_User_UserId",
                table: "GameEvent");

            migrationBuilder.DropIndex(
                name: "IX_GameEvent_UserId",
                table: "GameEvent");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "GameEvent");
        }
    }
}
