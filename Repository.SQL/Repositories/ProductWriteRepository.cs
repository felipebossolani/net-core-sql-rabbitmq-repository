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
    public class ProductWriteRepository : IProductWriteRepository
    {
        private readonly StoreContext _context;

        public ProductWriteRepository(StoreContext context)
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

        public async Task UpdateAsync(Product product)
        {
            _context.Entry(product).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}
