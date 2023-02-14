using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChessWebApp.Migrations
{
    /// <inheritdoc />
    public partial class _11GameEventTweaksAndGameEventComments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ActionDescription",
                table: "GameEvent",
                newName: "Status");

            migrationBuilder.AddColumn<string>(
                name: "Notation",
                table: "GameEvent",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "GameEventComment",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GameEventId = table.Column<int>(type: "int", nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameEventComment", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameEventComment_GameEvent_GameEventId",
                        column: x => x.GameEventId,
                        principalTable: "GameEvent",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GameEventComment_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GameEventComment_GameEventId",
                table: "GameEventComment",
                column: "GameEventId");

            migrationBuilder.CreateIndex(
                name: "IX_GameEventComment_UserId",
                table: "GameEventComment",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameEventComment");

            migrationBuilder.DropColumn(
                name: "Notation",
                table: "GameEvent");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "GameEvent",
                newName: "ActionDescription");
        }
    }
}
