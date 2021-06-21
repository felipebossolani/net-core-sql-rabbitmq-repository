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
        private readonly IDistributedCache _cache;

        public ProductReadCachingDecorator(IProductReadRepository dbProductReadRepository, IDistributedCache cache)
        {
            _dbProductReadRepository = dbProductReadRepository;
            _cache = cache;
        }

        private T Get<T>(string key)
        {
            var value = _cache.GetString(key);
            if (value != null)
            {
                return JsonSerializer.Deserialize<T>(value);
            }
            return default;
        }

        private T Set<T>(string key, T value)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1),
                SlidingExpiration = TimeSpan.FromMinutes(10)
            };
            _cache.SetString(key, JsonSerializer.Serialize(value), options);
            return value;
        }

        public async Task<IReadOnlyList<Product>> GetAllAsync()
        {
            var cacheKey = "all-products";

            var data = Get<IReadOnlyList<Product>>(cacheKey);
            if (data is null)
            {
                data = await _dbProductReadRepository.GetAllAsync();
                Set(cacheKey, data);
            }
            return data;
        }

        public async Task<Product> GetByIdAsync(Guid id)
        {
            var cacheKey = $"product-{id}";

            var data = Get<Product>(cacheKey);
            if (data is null)
            {
                data = await _dbProductReadRepository.GetByIdAsync(id);
                Set(cacheKey, data);
            }
            return data;
        }
    }
}
