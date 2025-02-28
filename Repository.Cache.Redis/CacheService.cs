﻿using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Text;
using System.Text.Json;

namespace Repository.Cache.Redis
{
    internal class CacheService
    {
        private readonly IDistributedCache _cache;

        internal CacheService(IDistributedCache cache)
        {
            _cache = cache;
        }

        internal T Get<T>(string key)
        {
            var value = _cache.GetString(key);
            if (value != null)
            {
                return JsonSerializer.Deserialize<T>(value);
            }
            return default;
        }

        internal T Set<T>(string key, T value)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
                SlidingExpiration = TimeSpan.FromMinutes(10)
            };
            _cache.SetString(key, value: JsonSerializer.Serialize(value), options);
            return value;
        }

        internal void Remove(string key)
        {
            _cache.Remove(key);
        }
    }
}
