using ChessWebApp.Models;
using Microsoft.EntityFrameworkCore;

namespace ChessWebApp.Data
{
    public static class Seed
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new MvcGameContext(serviceProvider.GetRequiredService<DbContextOptions<MvcGameContext>>()))
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
