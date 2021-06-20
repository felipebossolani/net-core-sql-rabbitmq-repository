using Domain.Models;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository.SQL.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly StoreContext _context;

        public ProductRepository(StoreContext context)
        {
            _context = context;
        }

        public async Task<Product> AddAsync(Product product)
        {
            await _context.AddAsync(product);
            await _context.SaveChangesAsync();

            return product;
        }

        public async Task DeleteAsync(Product product)
        {
            _context.Set<Product>().Remove(product);
            await _context.SaveChangesAsync();
        }

        public async Task<IReadOnlyList<Product>> GetAllAsync() => 
            await _context
                .Products
                .OrderBy(x => x.Description)
                .ToListAsync();

        public async Task<Product> GetByIdAsync(Guid id) => 
            await _context.Products.FindAsync(id);

        public async Task UpdateAsync(Product product)
        {
            _context.Entry(product).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}
