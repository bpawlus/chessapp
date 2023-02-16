using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChessWebApp.Migrations
{
    /// <inheritdoc />
    public partial class _20FUCKYOUFUCKYOU : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Game_User_PlayerBottomId",
                table: "Game");

            migrationBuilder.DropForeignKey(
                name: "FK_Game_User_PlayerLoserId",
                table: "Game");

            migrationBuilder.DropForeignKey(
                name: "FK_Game_User_PlayerTopId",
                table: "Game");

            migrationBuilder.DropForeignKey(
                name: "FK_Game_User_PlayerWinnerId",
                table: "Game");

            migrationBuilder.AddForeignKey(
                name: "FK_Game_User_PlayerBottomId",
                table: "Game",
                column: "PlayerBottomId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Game_User_PlayerLoserId",
                table: "Game",
                column: "PlayerLoserId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Game_User_PlayerTopId",
                table: "Game",
                column: "PlayerTopId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Game_User_PlayerWinnerId",
                table: "Game",
                column: "PlayerWinnerId",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Game_User_PlayerBottomId",
                table: "Game");

            migrationBuilder.DropForeignKey(
                name: "FK_Game_User_PlayerLoserId",
                table: "Game");

            migrationBuilder.DropForeignKey(
                name: "FK_Game_User_PlayerTopId",
                table: "Game");

            migrationBuilder.DropForeignKey(
                name: "FK_Game_User_PlayerWinnerId",
                table: "Game");

            migrationBuilder.AddForeignKey(
                name: "FK_Game_User_PlayerBottomId",
                table: "Game",
                column: "PlayerBottomId",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Game_User_PlayerLoserId",
                table: "Game",
                column: "PlayerLoserId",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Game_User_PlayerTopId",
                table: "Game",
                column: "PlayerTopId",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Game_User_PlayerWinnerId",
                table: "Game",
                column: "PlayerWinnerId",
                principalTable: "User",
                principalColumn: "Id");
        }
    }
}
