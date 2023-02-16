using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChessWebApp.Migrations
{
    /// <inheritdoc />
    public partial class _24GFDFG : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Game_User_PlayerTopId",
                table: "Game");

            migrationBuilder.DropForeignKey(
                name: "FK_GameEvent_Game_GameId",
                table: "GameEvent");

            migrationBuilder.AddForeignKey(
                name: "FK_Game_User_PlayerTopId",
                table: "Game",
                column: "PlayerTopId",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GameEvent_Game_GameId",
                table: "GameEvent",
                column: "GameId",
                principalTable: "Game",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Game_User_PlayerTopId",
                table: "Game");

            migrationBuilder.DropForeignKey(
                name: "FK_GameEvent_Game_GameId",
                table: "GameEvent");

            migrationBuilder.AddForeignKey(
                name: "FK_Game_User_PlayerTopId",
                table: "Game",
                column: "PlayerTopId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_GameEvent_Game_GameId",
                table: "GameEvent",
                column: "GameId",
                principalTable: "Game",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
