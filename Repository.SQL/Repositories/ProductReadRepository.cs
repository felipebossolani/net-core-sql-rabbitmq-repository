using Domain.Models;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Repository.SQL.Repositories
{
    public class ProductReadRepository : IProductReadRepository
    {
        private readonly StoreContext _context;

        public ProductReadRepository(StoreContext context)
        {
            _context = context;
        }

        public async Task<IReadOnlyList<Product>> GetAllAsync() => 
            await _context
                .Products
                .OrderBy(x => x.Description)
                .ToListAsync();

        public async Task<Product> GetByIdAsync(Guid id) => 
            await _context.Products.FindAsync(id);
    }
}
