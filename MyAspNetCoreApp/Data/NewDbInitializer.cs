using MyAspNetCoreApp.Models;
using Microsoft.EntityFrameworkCore;

namespace MyAspNetCoreApp.Data
{
    public static class NewDbInitializer
    {
        public static void Initialize(NewApplicationDbContext context)
        {
            context.Database.Migrate();

            // Check if there are any clients
            if (context.Clients.Any())
            {
                return;   // DB has been seeded
            }

            var clients = new Client[]
            {
                new Client
                {
                    ClientName = "Tech Solutions Inc",
                    MainBusiness = "Software Development",
                    Address = "123 Tech Street, Silicon Valley",
                    Website = "www.techsolutions.com",
                    ContactPerson = "John Smith",
                    ExpDate = DateTime.Now.AddYears(1),
                    Active = true
                },
                new Client
                {
                    ClientName = "Global Manufacturing Ltd",
                    MainBusiness = "Manufacturing",
                    Address = "456 Industry Road, Detroit",
                    Website = "www.globalmanufacturing.com",
                    ContactPerson = "Sarah Johnson",
                    ExpDate = DateTime.Now.AddYears(2),
                    Active = true
                },
                new Client
                {
                    ClientName = "Retail Dynamics",
                    MainBusiness = "Retail",
                    Address = "789 Market Ave, New York",
                    Website = "www.retaildynamics.com",
                    ContactPerson = "Michael Brown",
                    ExpDate = DateTime.Now.AddMonths(6),
                    Active = true
                }
            };

            context.Clients.AddRange(clients);
            context.SaveChanges();
        }
    }
}
