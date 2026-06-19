using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using CRNAssessment.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CRNAssessment.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //applies configurations
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            //index for performance optimization
            modelBuilder.Entity<Product>()
                .HasIndex(p => p.ProductName)
                .HasDatabaseName("IX_Products_ProductName");
        }

        
    }
}
