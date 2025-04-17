using Inspire.Erp.Domain.Constants;
using Inspire.Erp.Domain.Entities;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;

namespace Inspire.Erp.Application.Common
{
    public class CacheMemoryService
    {
        private readonly IMemoryCache _cache;

        public CacheMemoryService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public void SaveUserObject(object obj, string key)
        {
            if (GetUserObject(key) != null)
                RemoveObject(key);
            _cache.Set(key, obj, new MemoryCacheEntryOptions
            {
                SlidingExpiration = TimeSpan.FromDays(30)
            });
        }

        public object GetUserObject(string key)
        {
            _cache.TryGetValue(key, out object ojb);
            return ojb;
        }

        public void RemoveObject(string key)
        {
            _cache.Remove(key);
        }
    }
}
