using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChessWebApp.Migrations
{
    /// <inheritdoc />
    public partial class _19fffffffff : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GameEvent_User_UserId",
                table: "GameEvent");

            migrationBuilder.AddForeignKey(
                name: "FK_GameEvent_User_UserId",
                table: "GameEvent",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GameEvent_User_UserId",
                table: "GameEvent");

            migrationBuilder.AddForeignKey(
                name: "FK_GameEvent_User_UserId",
                table: "GameEvent",
                column: "UserId",
                principalTable: "User",
                principalColumn: "Id");
        }
    }
}
