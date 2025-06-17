using MyAspNetCoreApp.Models;
using Microsoft.EntityFrameworkCore;

namespace MyAspNetCoreApp.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            context.Database.Migrate();

            // Check if there are any users
            if (context.Users.Any())
            {
                return;   // DB has been seeded
            }

            var users = new User[]
            {
                new User
                {
                    Username = "testuser",
                    PasswordHash = "password123", // In a real app, this should be properly hashed
                    Email = "test@example.com"
                }
            };

            context.Users.AddRange(users);
            context.SaveChanges();
        }
    }
}
