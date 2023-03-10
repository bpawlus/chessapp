// <auto-generated />
using System;
using ChessWebApp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace ChessWebApp.Migrations
{
    [DbContext(typeof(MvcGameContext))]
    [Migration("20230213151444_11-GameEventTweaksAndGameEventComments")]
    partial class _11GameEventTweaksAndGameEventComments
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("ChessWebApp.Models.Game", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("PlayerBottomId")
                        .HasColumnType("int");

                    b.Property<int?>("PlayerLoserId")
                        .HasColumnType("int");

                    b.Property<int?>("PlayerTopId")
                        .HasColumnType("int");

                    b.Property<int?>("PlayerWinnerId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("TimeEnd")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("TimeStart")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("PlayerBottomId");

                    b.HasIndex("PlayerLoserId");

                    b.HasIndex("PlayerTopId");

                    b.HasIndex("PlayerWinnerId");

                    b.ToTable("Game");
                });

            modelBuilder.Entity("ChessWebApp.Models.GameEvent", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("GameId")
                        .HasColumnType("int");

                    b.Property<string>("Notation")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("Time")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("GameId");

                    b.ToTable("GameEvent");
                });

            modelBuilder.Entity("ChessWebApp.Models.GameEventComment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Comment")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("GameEventId")
                        .HasColumnType("int");

                    b.Property<int?>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("GameEventId");

                    b.HasIndex("UserId");

                    b.ToTable("GameEventComment");
                });

            modelBuilder.Entity("ChessWebApp.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("Admin")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false);

                    b.Property<string>("Description")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)")
                        .HasDefaultValue("Player");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(32)
                        .HasColumnType("nvarchar(32)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasMaxLength(64)
                        .HasColumnType("nvarchar(64)");

                    b.Property<short>("VariantBishopLeft")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("smallint")
                        .HasDefaultValue((short)4);

                    b.Property<short>("VariantBishopRight")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("smallint")
                        .HasDefaultValue((short)4);

                    b.Property<short>("VariantKing")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("smallint")
                        .HasDefaultValue((short)6);

                    b.Property<short>("VariantKnightLeft")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("smallint")
                        .HasDefaultValue((short)3);

                    b.Property<short>("VariantKnightRight")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("smallint")
                        .HasDefaultValue((short)3);

                    b.Property<short>("VariantPawn1")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("smallint")
                        .HasDefaultValue((short)1);

                    b.Property<short>("VariantPawn2")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("smallint")
                        .HasDefaultValue((short)1);

                    b.Property<short>("VariantPawn3")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("smallint")
                        .HasDefaultValue((short)1);

                    b.Property<short>("VariantPawn4")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("smallint")
                        .HasDefaultValue((short)1);

                    b.Property<short>("VariantPawn5")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("smallint")
                        .HasDefaultValue((short)1);

                    b.Property<short>("VariantPawn6")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("smallint")
                        .HasDefaultValue((short)1);

                    b.Property<short>("VariantPawn7")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("smallint")
                        .HasDefaultValue((short)1);

                    b.Property<short>("VariantPawn8")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("smallint")
                        .HasDefaultValue((short)1);

                    b.Property<short>("VariantQueen")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("smallint")
                        .HasDefaultValue((short)5);

                    b.Property<short>("VariantRookLeft")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("smallint")
                        .HasDefaultValue((short)2);

                    b.Property<short>("VariantRookRight")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("smallint")
                        .HasDefaultValue((short)2);

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("User");
                });

            modelBuilder.Entity("ChessWebApp.Models.Game", b =>
                {
                    b.HasOne("ChessWebApp.Models.User", "PlayerBottom")
                        .WithMany("GamesBottom")
                        .HasForeignKey("PlayerBottomId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.HasOne("ChessWebApp.Models.User", "PlayerLoser")
                        .WithMany("GamesLost")
                        .HasForeignKey("PlayerLoserId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.HasOne("ChessWebApp.Models.User", "PlayerTop")
                        .WithMany("GamesTop")
                        .HasForeignKey("PlayerTopId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.HasOne("ChessWebApp.Models.User", "PlayerWinner")
                        .WithMany("GamesWon")
                        .HasForeignKey("PlayerWinnerId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.Navigation("PlayerBottom");

                    b.Navigation("PlayerLoser");

                    b.Navigation("PlayerTop");

                    b.Navigation("PlayerWinner");
                });

            modelBuilder.Entity("ChessWebApp.Models.GameEvent", b =>
                {
                    b.HasOne("ChessWebApp.Models.Game", "Game")
                        .WithMany("GameEvents")
                        .HasForeignKey("GameId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("Game");
                });

            modelBuilder.Entity("ChessWebApp.Models.GameEventComment", b =>
                {
                    b.HasOne("ChessWebApp.Models.GameEvent", "GameEvent")
                        .WithMany("GameEventComments")
                        .HasForeignKey("GameEventId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("ChessWebApp.Models.User", "User")
                        .WithMany("Comments")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.Navigation("GameEvent");

                    b.Navigation("User");
                });

            modelBuilder.Entity("ChessWebApp.Models.Game", b =>
                {
                    b.Navigation("GameEvents");
                });

            modelBuilder.Entity("ChessWebApp.Models.GameEvent", b =>
                {
                    b.Navigation("GameEventComments");
                });

            modelBuilder.Entity("ChessWebApp.Models.User", b =>
                {
                    b.Navigation("Comments");

                    b.Navigation("GamesBottom");

                    b.Navigation("GamesLost");

                    b.Navigation("GamesTop");

                    b.Navigation("GamesWon");
                });
#pragma warning restore 612, 618
        }
    }
}
