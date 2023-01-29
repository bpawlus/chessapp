using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChessWebApp.Migrations
{
    /// <inheritdoc />
    public partial class _8GameEventChangesPlusLoserWinner : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GameEvents_Game_GameId",
                table: "GameEvents");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GameEvents",
                table: "GameEvents");

            migrationBuilder.DropColumn(
                name: "ActionId",
                table: "GameEvents");

            migrationBuilder.RenameTable(
                name: "GameEvents",
                newName: "GameEvent");

            migrationBuilder.RenameColumn(
                name: "ActionValue",
                table: "GameEvent",
                newName: "ActionDescription");

            migrationBuilder.RenameIndex(
                name: "IX_GameEvents_GameId",
                table: "GameEvent",
                newName: "IX_GameEvent_GameId");

            migrationBuilder.AddColumn<int>(
                name: "PlayerLoserId",
                table: "Game",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PlayerWinnerId",
                table: "Game",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_GameEvent",
                table: "GameEvent",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Game_PlayerLoserId",
                table: "Game",
                column: "PlayerLoserId");

            migrationBuilder.CreateIndex(
                name: "IX_Game_PlayerWinnerId",
                table: "Game",
                column: "PlayerWinnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Game_User_PlayerLoserId",
                table: "Game",
                column: "PlayerLoserId",
                principalTable: "User",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Game_User_PlayerWinnerId",
                table: "Game",
                column: "PlayerWinnerId",
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
                name: "FK_Game_User_PlayerLoserId",
                table: "Game");

            migrationBuilder.DropForeignKey(
                name: "FK_Game_User_PlayerWinnerId",
                table: "Game");

            migrationBuilder.DropForeignKey(
                name: "FK_GameEvent_Game_GameId",
                table: "GameEvent");

            migrationBuilder.DropIndex(
                name: "IX_Game_PlayerLoserId",
                table: "Game");

            migrationBuilder.DropIndex(
                name: "IX_Game_PlayerWinnerId",
                table: "Game");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GameEvent",
                table: "GameEvent");

            migrationBuilder.DropColumn(
                name: "PlayerLoserId",
                table: "Game");

            migrationBuilder.DropColumn(
                name: "PlayerWinnerId",
                table: "Game");

            migrationBuilder.RenameTable(
                name: "GameEvent",
                newName: "GameEvents");

            migrationBuilder.RenameColumn(
                name: "ActionDescription",
                table: "GameEvents",
                newName: "ActionValue");

            migrationBuilder.RenameIndex(
                name: "IX_GameEvent_GameId",
                table: "GameEvents",
                newName: "IX_GameEvents_GameId");

            migrationBuilder.AddColumn<short>(
                name: "ActionId",
                table: "GameEvents",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_GameEvents",
                table: "GameEvents",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GameEvents_Game_GameId",
                table: "GameEvents",
                column: "GameId",
                principalTable: "Game",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
