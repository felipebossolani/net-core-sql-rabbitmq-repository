using Domain.Models;
using Domain.Repositories;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Threading.Tasks;

namespace Repository.Cache.Redis.Repositories
{
    public class ProductWriteCachingDecorator : IProductWriteRepository
    {
        private readonly IProductWriteRepository _dbProductWriteRepository;
        private readonly CacheService _cache;

        private const string _keyAll = "all-products";
        private const string _keyById = "product-{0}";

        public ProductWriteCachingDecorator(IProductWriteRepository dbProductWriteRepository, IDistributedCache cache)
        {
            _dbProductWriteRepository = dbProductWriteRepository;
            _cache = new CacheService(cache);
        }

        public async Task<Product> AddAsync(Product product)
        {
            _cache.Remove(_keyAll);
            _cache.Set(string.Format(_keyById, product.Id), product);
            return await _dbProductWriteRepository.AddAsync(product);
        }

        public async Task DeleteAsync(Product product)
        {
            _cache.Remove(_keyAll);
            _cache.Set(string.Format(_keyById, product.Id), product);
            await _dbProductWriteRepository.DeleteAsync(product);
        }

        public async Task UpdateAsync(Product product)
        {
            _cache.Remove(_keyAll);
            _cache.Remove(string.Format(_keyById, product.Id));
            await _dbProductWriteRepository.UpdateAsync(product);
        }
    }
}
