using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRNAssessment.Application.Interfaces;
using CRNAssessment.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CRNAssessment.Infrastructure.Data.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;
        public ProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<(IEnumerable<Product>items,int total)> GetAllProducts(int pageNumber, int pageSize)
        {
          var totalCount=await _context.Products.CountAsync();

            var items = await _context.Products
               .Include(p => p.Items)
               .AsNoTracking()
               .Skip((pageNumber - 1) * pageSize)
               .Take(pageSize)
               .ToListAsync();
            return (items, totalCount);
        }
        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await _context.Products
                .Include(p => p.Items)
                .FirstOrDefaultAsync(p => p.Id == id);
        }
        public async Task<Product> AddProduct(Product product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
            return product;
        }
        public async Task UpdateProduct(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteProduct(Product product)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }

    }
}
