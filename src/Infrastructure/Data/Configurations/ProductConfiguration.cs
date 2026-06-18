using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRNAssessment.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRNAssessment.Infrastructure.Data.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Product");
            builder.HasKey(p => p.Id);

            builder.Property(p => p.ProductName)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(p=>p.CreatedBy)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.CreatedOn)
                .IsRequired();

            builder.Property(p => p.ModifiedBy)
                 .HasMaxLength(100);

            builder.Property(p => p.ModifiedOn);

            builder.HasMany(p=>p.Items)
                .WithOne(i=>i.Product)
                .HasForeignKey(i => i.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
