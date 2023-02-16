using ChessWebApp.Models;
using Microsoft.EntityFrameworkCore;

namespace ChessWebApp.Data
{
    public static class Seed
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            var args = serviceProvider.GetRequiredService<DbContextOptions<MvcGameContext>>();
            using (var context = new MvcGameContext(args))
            {
                if (!context.User.Any())
                {
                    context.User.AddRange(
                        new User
                        {
                            Name = "admin",
                            Password = "admin",
                            Admin = true,
                        }
                    );
                    context.SaveChanges();
                }

                if (!context.AllowedPiece.Any())
                {
                    context.AllowedPiece.AddRange(
                        new AllowedPiece
                        {
                            VariantKey = "VariantKing",
                            AllowedFactoryId = 6,
                        },
                        new AllowedPiece
                        {
                            VariantKey = "VariantQueen",
                            AllowedFactoryId = 5,
                        },
                        new AllowedPiece
                        {
                            VariantKey = "VariantQueen",
                            AllowedFactoryId = 4,
                            ConditionName = "Wins10"
                        },
                        new AllowedPiece
                        {
                            VariantKey = "VariantQueen",
                            AllowedFactoryId = 3,
                            ConditionName = "Wins10"
                        },
                        new AllowedPiece
                        {
                            VariantKey = "VariantQueen",
                            AllowedFactoryId = 2,
                            ConditionName = "Wins10"
                        }
                    );

                    string[] sides = new string[] { "Left", "Right" };
                    short[] allowedIds = new short[] { 2, 3, 4 };
                    foreach (var side in sides)
                    {
                        foreach (var allowedId in allowedIds)
                        {
                            context.AllowedPiece.AddRange(
                                new AllowedPiece
                                {
                                    VariantKey = "VariantBishop" + side,
                                    AllowedFactoryId = allowedId,
                                    ConditionName = allowedId == 4 ? null : "Wins5"
                                },
                                new AllowedPiece
                                {
                                    VariantKey = "VariantKnight" + side,
                                    AllowedFactoryId = allowedId,
                                    ConditionName = allowedId == 3 ? null : "Wins5"
                                },
                                new AllowedPiece
                                {
                                    VariantKey = "VariantRook" + side,
                                    AllowedFactoryId = allowedId,
                                    ConditionName = allowedId == 2 ? null : "Wins5"
                                }
                            );
                        }
                    }

                    for (int i = 1; i <= 8; i++)
                    {
                        context.AllowedPiece.AddRange(
                            new AllowedPiece
                            {
                                VariantKey = "VariantPawn" + i,
                                AllowedFactoryId = 1,
                            }
                        );
                    }
                    context.SaveChanges();
                }
            }
        }
    }
}
