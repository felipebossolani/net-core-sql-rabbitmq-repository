using Domain.Models;
using Domain.Repositories;
using Microsoft.Extensions.Caching.Distributed;

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Repository.Cache.Redis.Repositories
{
    public class ProductReadCachingDecorator : IProductReadRepository
    {
        private readonly IProductReadRepository _dbProductReadRepository;
        private readonly CacheService _cache;

        public ProductReadCachingDecorator(IProductReadRepository dbProductReadRepository, IDistributedCache cache)
        {
            _dbProductReadRepository = dbProductReadRepository;
            _cache = new CacheService(cache);
        }

        public async Task<IReadOnlyList<Product>> GetAllAsync()
        {
            var cacheKey = "all-products";

            var data = _cache.Get<IReadOnlyList<Product>>(cacheKey);
            if (data is null)
            {
                data = await _dbProductReadRepository.GetAllAsync();
                _cache.Set(cacheKey, data);
            }
            return data;
        }

        public async Task<Product> GetByIdAsync(Guid id)
        {
            var cacheKey = $"product-{id}";
            var data = _cache.Get<Product>(cacheKey);
            if (data is null)
            {
                data = await _dbProductReadRepository.GetByIdAsync(id);
                _cache.Set(cacheKey, data);
            }
            return data;
        }
    }
}
