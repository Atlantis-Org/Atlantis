using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;

namespace Followme.AspNet.Core.FastCommon.ThirdParty.Cache
{
    public class InMemoryCache : ICache
    {
        private static readonly Lazy<MemoryCache> memoryCache 
            = new Lazy<MemoryCache>(()=> new MemoryCache(new MemoryCacheOptions()),true);

        public bool Set<T>(string key, T value, TimeSpan span)
            => memoryCache.Value.Set(key, value, span) == null ? false : true;

        public T Get<T>(string key)
            => memoryCache.Value.Get<T>(key);

        public void Remove(string key)
            => memoryCache.Value.Remove(key);
    }
}
