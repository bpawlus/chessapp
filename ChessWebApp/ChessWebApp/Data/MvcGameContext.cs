using Microsoft.EntityFrameworkCore;
using ChessWebApp.Models;
using System.Reflection.Emit;
using ChessWebApp.ChessGame;

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

                user
                .HasMany(a => a.GamesWon)
                .WithOne(b => b.PlayerWinner)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(false);

                user
                .HasMany(a => a.GamesLost)
                .WithOne(b => b.PlayerLoser)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(false);

                user.HasIndex(x => x.Name).IsUnique(true);

                user.Property(p => p.Admin).HasDefaultValue(false);

                user.Property(p => p.Description).HasDefaultValue("Player").IsRequired(false);

                user.Property(p => p.VariantPawn1).HasDefaultValue(1);
                user.Property(p => p.VariantPawn2).HasDefaultValue(1);
                user.Property(p => p.VariantPawn3).HasDefaultValue(1);
                user.Property(p => p.VariantPawn4).HasDefaultValue(1);
                user.Property(p => p.VariantPawn5).HasDefaultValue(1);
                user.Property(p => p.VariantPawn6).HasDefaultValue(1);
                user.Property(p => p.VariantPawn7).HasDefaultValue(1);
                user.Property(p => p.VariantPawn8).HasDefaultValue(1);

                user.Property(p => p.VariantRookLeft).HasDefaultValue(2);
                user.Property(p => p.VariantRookRight).HasDefaultValue(2);
                user.Property(p => p.VariantKnightLeft).HasDefaultValue(3);
                user.Property(p => p.VariantKnightRight).HasDefaultValue(3);
                user.Property(p => p.VariantBishopLeft).HasDefaultValue(4);
                user.Property(p => p.VariantBishopRight).HasDefaultValue(4);
                user.Property(p => p.VariantQueen).HasDefaultValue(5);
                user.Property(p => p.VariantKing).HasDefaultValue(6);
            });

            modelBuilder.Entity<Game>(game => {
                game
                .HasMany(a => a.GameEvents)
                .WithOne(b => b.Game)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(false);
            });
        }

        public DbSet<GameEvent> GameEvent { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<Game> Game { get; set; }
    }
}
