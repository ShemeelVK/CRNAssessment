using CRNAssessment.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CRNAssessment.Infrastructure.Data
{
    public static class DatabaseInitializer
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope(); //used manual scope to use scoped service outside the request

            var context = scope.ServiceProvider
                .GetRequiredService<ApplicationDbContext>();

            var logger = scope.ServiceProvider
                .GetRequiredService<ILogger<ApplicationDbContext>>();

            try
            {
                logger.LogInformation("Applying pending database migrations...");
                await context.Database.MigrateAsync();

                // Seed Admin User
                if (!await context.Users.AnyAsync(u => u.Role == "Admin"))
                {
                    logger.LogInformation("Seeding default Admin user...");
                    var adminUser = new User
                    {
                        Username = "admin",
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin@123"),
                        Role = "Admin"
                    };
                    await context.Users.AddAsync(adminUser);
                    await context.SaveChangesAsync();
                }

                // Seed Initial Products
                if (!await context.Products.AnyAsync())
                {
                    logger.LogInformation("Seeding default products...");
                    var products = new List<Product>
                    {
                        new Product 
                        { 
                            ProductName = "Apple MacBook Pro M3 Max", 
                            CreatedBy = "System", 
                            CreatedOn = DateTime.UtcNow,
                            Items = new List<Item> { new Item { Quantity = 10 } }
                        },
                        new Product 
                        { 
                            ProductName = "Dell XPS 15", 
                            CreatedBy = "System", 
                            CreatedOn = DateTime.UtcNow,
                            Items = new List<Item> { new Item { Quantity = 25 } }
                        }
                    };
                    await context.Products.AddRangeAsync(products);
                    await context.SaveChangesAsync();
                }

                logger.LogInformation("Database initialization and seeding completed successfully.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while initializing the database.");
                throw;
            }
        }
    }
}
