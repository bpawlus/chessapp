using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChessWebApp.Migrations
{
    /// <inheritdoc />
    public partial class _6UpdatePreTests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserChessboardId",
                table: "User");

            migrationBuilder.AlterColumn<short>(
                name: "VariantRookRight",
                table: "User",
                type: "smallint",
                nullable: false,
                defaultValue: (short)2,
                oldClrType: typeof(short),
                oldType: "smallint",
                oldDefaultValue: (short)1);

            migrationBuilder.AlterColumn<short>(
                name: "VariantRookLeft",
                table: "User",
                type: "smallint",
                nullable: false,
                defaultValue: (short)2,
                oldClrType: typeof(short),
                oldType: "smallint",
                oldDefaultValue: (short)1);

            migrationBuilder.AlterColumn<short>(
                name: "VariantQueen",
                table: "User",
                type: "smallint",
                nullable: false,
                defaultValue: (short)5,
                oldClrType: typeof(short),
                oldType: "smallint",
                oldDefaultValue: (short)4);

            migrationBuilder.AlterColumn<short>(
                name: "VariantPawn8",
                table: "User",
                type: "smallint",
                nullable: false,
                defaultValue: (short)1,
                oldClrType: typeof(short),
                oldType: "smallint",
                oldDefaultValue: (short)0);

            migrationBuilder.AlterColumn<short>(
                name: "VariantPawn7",
                table: "User",
                type: "smallint",
                nullable: false,
                defaultValue: (short)1,
                oldClrType: typeof(short),
                oldType: "smallint",
                oldDefaultValue: (short)0);

            migrationBuilder.AlterColumn<short>(
                name: "VariantPawn6",
                table: "User",
                type: "smallint",
                nullable: false,
                defaultValue: (short)1,
                oldClrType: typeof(short),
                oldType: "smallint",
                oldDefaultValue: (short)0);

            migrationBuilder.AlterColumn<short>(
                name: "VariantPawn5",
                table: "User",
                type: "smallint",
                nullable: false,
                defaultValue: (short)1,
                oldClrType: typeof(short),
                oldType: "smallint",
                oldDefaultValue: (short)0);

            migrationBuilder.AlterColumn<short>(
                name: "VariantPawn4",
                table: "User",
                type: "smallint",
                nullable: false,
                defaultValue: (short)1,
                oldClrType: typeof(short),
                oldType: "smallint",
                oldDefaultValue: (short)0);

            migrationBuilder.AlterColumn<short>(
                name: "VariantPawn3",
                table: "User",
                type: "smallint",
                nullable: false,
                defaultValue: (short)1,
                oldClrType: typeof(short),
                oldType: "smallint",
                oldDefaultValue: (short)0);

            migrationBuilder.AlterColumn<short>(
                name: "VariantPawn2",
                table: "User",
                type: "smallint",
                nullable: false,
                defaultValue: (short)1,
                oldClrType: typeof(short),
                oldType: "smallint",
                oldDefaultValue: (short)0);

            migrationBuilder.AlterColumn<short>(
                name: "VariantPawn1",
                table: "User",
                type: "smallint",
                nullable: false,
                defaultValue: (short)1,
                oldClrType: typeof(short),
                oldType: "smallint",
                oldDefaultValue: (short)0);

            migrationBuilder.AlterColumn<short>(
                name: "VariantKnightRight",
                table: "User",
                type: "smallint",
                nullable: false,
                defaultValue: (short)3,
                oldClrType: typeof(short),
                oldType: "smallint",
                oldDefaultValue: (short)2);

            migrationBuilder.AlterColumn<short>(
                name: "VariantKnightLeft",
                table: "User",
                type: "smallint",
                nullable: false,
                defaultValue: (short)3,
                oldClrType: typeof(short),
                oldType: "smallint",
                oldDefaultValue: (short)2);

            migrationBuilder.AlterColumn<short>(
                name: "VariantKing",
                table: "User",
                type: "smallint",
                nullable: false,
                defaultValue: (short)6,
                oldClrType: typeof(short),
                oldType: "smallint",
                oldDefaultValue: (short)5);

            migrationBuilder.AlterColumn<short>(
                name: "VariantBishopRight",
                table: "User",
                type: "smallint",
                nullable: false,
                defaultValue: (short)4,
                oldClrType: typeof(short),
                oldType: "smallint",
                oldDefaultValue: (short)3);

            migrationBuilder.AlterColumn<short>(
                name: "VariantBishopLeft",
                table: "User",
                type: "smallint",
                nullable: false,
                defaultValue: (short)4,
                oldClrType: typeof(short),
                oldType: "smallint",
                oldDefaultValue: (short)3);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<short>(
                name: "VariantRookRight",
                table: "User",
                type: "smallint",
                nullable: false,
                defaultValue: (short)1,
                oldClrType: typeof(short),
                oldType: "smallint",
                oldDefaultValue: (short)2);

            migrationBuilder.AlterColumn<short>(
                name: "VariantRookLeft",
                table: "User",
                type: "smallint",
                nullable: false,
                defaultValue: (short)1,
                oldClrType: typeof(short),
                oldType: "smallint",
                oldDefaultValue: (short)2);

            migrationBuilder.AlterColumn<short>(
                name: "VariantQueen",
                table: "User",
                type: "smallint",
                nullable: false,
                defaultValue: (short)4,
                oldClrType: typeof(short),
                oldType: "smallint",
                oldDefaultValue: (short)5);

            migrationBuilder.AlterColumn<short>(
                name: "VariantPawn8",
                table: "User",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0,
                oldClrType: typeof(short),
                oldType: "smallint",
                oldDefaultValue: (short)1);

            migrationBuilder.AlterColumn<short>(
                name: "VariantPawn7",
                table: "User",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0,
                oldClrType: typeof(short),
                oldType: "smallint",
                oldDefaultValue: (short)1);

            migrationBuilder.AlterColumn<short>(
                name: "VariantPawn6",
                table: "User",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0,
                oldClrType: typeof(short),
                oldType: "smallint",
                oldDefaultValue: (short)1);

            migrationBuilder.AlterColumn<short>(
                name: "VariantPawn5",
                table: "User",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0,
                oldClrType: typeof(short),
                oldType: "smallint",
                oldDefaultValue: (short)1);

            migrationBuilder.AlterColumn<short>(
                name: "VariantPawn4",
                table: "User",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0,
                oldClrType: typeof(short),
                oldType: "smallint",
                oldDefaultValue: (short)1);

            migrationBuilder.AlterColumn<short>(
                name: "VariantPawn3",
                table: "User",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0,
                oldClrType: typeof(short),
                oldType: "smallint",
                oldDefaultValue: (short)1);

            migrationBuilder.AlterColumn<short>(
                name: "VariantPawn2",
                table: "User",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0,
                oldClrType: typeof(short),
                oldType: "smallint",
                oldDefaultValue: (short)1);

            migrationBuilder.AlterColumn<short>(
                name: "VariantPawn1",
                table: "User",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0,
                oldClrType: typeof(short),
                oldType: "smallint",
                oldDefaultValue: (short)1);

            migrationBuilder.AlterColumn<short>(
                name: "VariantKnightRight",
                table: "User",
                type: "smallint",
                nullable: false,
                defaultValue: (short)2,
                oldClrType: typeof(short),
                oldType: "smallint",
                oldDefaultValue: (short)3);

            migrationBuilder.AlterColumn<short>(
                name: "VariantKnightLeft",
                table: "User",
                type: "smallint",
                nullable: false,
                defaultValue: (short)2,
                oldClrType: typeof(short),
                oldType: "smallint",
                oldDefaultValue: (short)3);

            migrationBuilder.AlterColumn<short>(
                name: "VariantKing",
                table: "User",
                type: "smallint",
                nullable: false,
                defaultValue: (short)5,
                oldClrType: typeof(short),
                oldType: "smallint",
                oldDefaultValue: (short)6);

            migrationBuilder.AlterColumn<short>(
                name: "VariantBishopRight",
                table: "User",
                type: "smallint",
                nullable: false,
                defaultValue: (short)3,
                oldClrType: typeof(short),
                oldType: "smallint",
                oldDefaultValue: (short)4);

            migrationBuilder.AlterColumn<short>(
                name: "VariantBishopLeft",
                table: "User",
                type: "smallint",
                nullable: false,
                defaultValue: (short)3,
                oldClrType: typeof(short),
                oldType: "smallint",
                oldDefaultValue: (short)4);

            migrationBuilder.AddColumn<int>(
                name: "UserChessboardId",
                table: "User",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
