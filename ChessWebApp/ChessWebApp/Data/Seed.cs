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
                if (context.User.Any())
                {
                    return;
                }

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
        }
    }
}
