using Microsoft.EntityFrameworkCore;
using ChessWebApp.Models;
using System.Reflection.Emit;
using ChessWebApp.Core;

namespace ChessWebApp.Data
{
    public class MvcGameContext : DbContext
    {
        public MvcGameContext(DbContextOptions<MvcGameContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(user =>
            {
                user
                .HasMany(a => a.GamesTop)
                .WithOne(b => b.PlayerTop)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(false);

                user
                .HasMany(a => a.GamesBottom)
                .WithOne(b => b.PlayerBottom)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(false);

                user.HasIndex(x => x.Name).IsUnique(true);

                user.Property(p => p.Admin).HasDefaultValue(false);

                user.Property(p => p.Description).HasDefaultValue("Player").IsRequired(false);

                user.Property(p => p.VariantPawn1).HasDefaultValue(ChessPiecesEnum.ClassicPawn);
                user.Property(p => p.VariantPawn2).HasDefaultValue(ChessPiecesEnum.ClassicPawn);
                user.Property(p => p.VariantPawn3).HasDefaultValue(ChessPiecesEnum.ClassicPawn);
                user.Property(p => p.VariantPawn4).HasDefaultValue(ChessPiecesEnum.ClassicPawn);
                user.Property(p => p.VariantPawn5).HasDefaultValue(ChessPiecesEnum.ClassicPawn);
                user.Property(p => p.VariantPawn6).HasDefaultValue(ChessPiecesEnum.ClassicPawn);
                user.Property(p => p.VariantPawn7).HasDefaultValue(ChessPiecesEnum.ClassicPawn);
                user.Property(p => p.VariantPawn8).HasDefaultValue(ChessPiecesEnum.ClassicPawn);

                user.Property(p => p.VariantRookLeft).HasDefaultValue(ChessPiecesEnum.ClassicRook);
                user.Property(p => p.VariantRookRight).HasDefaultValue(ChessPiecesEnum.ClassicRook);
                user.Property(p => p.VariantKnightLeft).HasDefaultValue(ChessPiecesEnum.ClassicKnight);
                user.Property(p => p.VariantKnightRight).HasDefaultValue(ChessPiecesEnum.ClassicKnight);
                user.Property(p => p.VariantBishopLeft).HasDefaultValue(ChessPiecesEnum.ClassicBishop);
                user.Property(p => p.VariantBishopRight).HasDefaultValue(ChessPiecesEnum.ClassicBishop);
                user.Property(p => p.VariantQueen).HasDefaultValue(ChessPiecesEnum.ClassicQueen);
                user.Property(p => p.VariantKing).HasDefaultValue(ChessPiecesEnum.ClassicKing);
            });

            modelBuilder.Entity<Game>(game => {
                game
                .HasMany(a => a.GameEvents)
                .WithOne(b => b.Game)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(false);
            });
        }

        public DbSet<User> User { get; set; }
        public DbSet<Game> Game { get; set; }
        public DbSet<GameEvent> GameEvent { get; set; }
    }
}
