using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChessWebApp.Migrations
{
    /// <inheritdoc />
    public partial class _2Beta : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Admin",
                table: "User",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "User",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "UserChessboardId",
                table: "User",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<short>(
                name: "VariantBishopLeft",
                table: "User",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<short>(
                name: "VariantBishopRight",
                table: "User",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<short>(
                name: "VariantKing",
                table: "User",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<short>(
                name: "VariantKnightLeft",
                table: "User",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<short>(
                name: "VariantKnightRight",
                table: "User",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<short>(
                name: "VariantPawn1",
                table: "User",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<short>(
                name: "VariantPawn2",
                table: "User",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<short>(
                name: "VariantPawn3",
                table: "User",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<short>(
                name: "VariantPawn4",
                table: "User",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<short>(
                name: "VariantPawn5",
                table: "User",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<short>(
                name: "VariantPawn6",
                table: "User",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<short>(
                name: "VariantPawn7",
                table: "User",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<short>(
                name: "VariantPawn8",
                table: "User",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<short>(
                name: "VariantQueen",
                table: "User",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<short>(
                name: "VariantRookLeft",
                table: "User",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddColumn<short>(
                name: "VariantRookRight",
                table: "User",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.CreateTable(
                name: "Game",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlayerBottomId = table.Column<int>(type: "int", nullable: false),
                    PlayerTopId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Game", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Game_User_PlayerBottomId",
                        column: x => x.PlayerBottomId,
                        principalTable: "User",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Game_User_PlayerTopId",
                        column: x => x.PlayerTopId,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "GameEvent",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ActionId = table.Column<short>(type: "smallint", nullable: false),
                    GameId = table.Column<int>(type: "int", nullable: false),
                    ActionValue = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TimeStamp = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameEvent", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameEvent_Game_GameId",
                        column: x => x.GameId,
                        principalTable: "Game",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_User_Name",
                table: "User",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Game_PlayerBottomId",
                table: "Game",
                column: "PlayerBottomId");

            migrationBuilder.CreateIndex(
                name: "IX_Game_PlayerTopId",
                table: "Game",
                column: "PlayerTopId");

            migrationBuilder.CreateIndex(
                name: "IX_GameEvent_GameId",
                table: "GameEvent",
                column: "GameId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameEvent");

            migrationBuilder.DropTable(
                name: "Game");

            migrationBuilder.DropIndex(
                name: "IX_User_Name",
                table: "User");

            migrationBuilder.DropColumn(
                name: "Admin",
                table: "User");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "User");

            migrationBuilder.DropColumn(
                name: "UserChessboardId",
                table: "User");

            migrationBuilder.DropColumn(
                name: "VariantBishopLeft",
                table: "User");

            migrationBuilder.DropColumn(
                name: "VariantBishopRight",
                table: "User");

            migrationBuilder.DropColumn(
                name: "VariantKing",
                table: "User");

            migrationBuilder.DropColumn(
                name: "VariantKnightLeft",
                table: "User");

            migrationBuilder.DropColumn(
                name: "VariantKnightRight",
                table: "User");

            migrationBuilder.DropColumn(
                name: "VariantPawn1",
                table: "User");

            migrationBuilder.DropColumn(
                name: "VariantPawn2",
                table: "User");

            migrationBuilder.DropColumn(
                name: "VariantPawn3",
                table: "User");

            migrationBuilder.DropColumn(
                name: "VariantPawn4",
                table: "User");

            migrationBuilder.DropColumn(
                name: "VariantPawn5",
                table: "User");

            migrationBuilder.DropColumn(
                name: "VariantPawn6",
                table: "User");

            migrationBuilder.DropColumn(
                name: "VariantPawn7",
                table: "User");

            migrationBuilder.DropColumn(
                name: "VariantPawn8",
                table: "User");

            migrationBuilder.DropColumn(
                name: "VariantQueen",
                table: "User");

            migrationBuilder.DropColumn(
                name: "VariantRookLeft",
                table: "User");

            migrationBuilder.DropColumn(
                name: "VariantRookRight",
                table: "User");
        }
    }
}
