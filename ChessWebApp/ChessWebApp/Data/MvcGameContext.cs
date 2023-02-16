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
                .HasMany(a => a.Comments)
                .WithOne(b => b.User)
                .OnDelete(DeleteBehavior.Cascade)
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
                .HasOne(a => a.PlayerTop)
                .WithMany(b => b.GamesTop)
                .HasForeignKey(g => g.PlayerTopId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(false);

                game
                .HasOne(b => b.PlayerBottom)
                .WithMany(b => b.GamesBottom)
                .HasForeignKey(g => g.PlayerBottomId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(false);

                game
                .HasOne(b => b.PlayerWinner)
                .WithMany(b => b.GamesWon)
                .HasForeignKey(g => g.PlayerWinnerId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(false);

                game
                .HasOne(b => b.PlayerLoser)
                .WithMany(b => b.GamesLost)
                .HasForeignKey(g => g.PlayerLoserId)
                .OnDelete(DeleteBehavior.NoAction)
                .IsRequired(false);

                game
                .HasMany(a => a.GameEvents)
                .WithOne(b => b.Game)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(false);
            });


           modelBuilder.Entity<GameEvent>(gameevent => {
               gameevent
                .HasOne(a => a.User)
                .WithMany(b => b.GameEvents)
                .HasForeignKey(g => g.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired(false);

               gameevent
                .HasMany(a => a.GameEventComments)
                .WithOne(b => b.GameEvent)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(false);
            });
        }

        public DbSet<GameEvent> GameEvent { get; set; }
        public DbSet<User> User { get; set; }
        public DbSet<Game> Game { get; set; }
        public DbSet<GameEventComment> GameEventComment { get; set; }
        public DbSet<AllowedPiece> AllowedPiece { get; set; }
    }
}
