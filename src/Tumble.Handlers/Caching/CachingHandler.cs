using System;
using System.Net.Http;
using System.Threading.Tasks;
using Tumble.Core;
using Tumble.Core.Providers;
using Microsoft.Extensions.Caching.Memory;
using Tumble.Handlers.Caching.Contexts;

namespace Tumble.Handlers.Caching
{
    public class CacheConfiguration
    {
        public TimeSpan CacheItemTotalLifeTime { get; set; }
        public TimeSpan CacheItemStaleTime { get; set; }
    }

    public enum CacheEnum { NoCache, FromCache };

    public class CachingHandler : IPipelineHandler<ICachingContext>
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly CacheConfiguration _cacheConfiguration;

        public CachingHandler(IMemoryCache memoryCache, IDateTimeProvider dateTimeProvider, CacheConfiguration cacheConfiguration)
        {
            _memoryCache = memoryCache;
            _dateTimeProvider = dateTimeProvider;
            _cacheConfiguration = cacheConfiguration;
        }

        private string GenerateCachekey(HttpRequestMessage httpRequestMessage) =>
            $"{httpRequestMessage.Method} {httpRequestMessage.RequestUri}".ToLower();

        private (bool, bool, CacheEntry) GetCacheEntry(string cacheKey) =>
            (_memoryCache.TryGetValue(cacheKey, out CacheEntry cacheEntry),
             cacheEntry?.IsStale(_dateTimeProvider) ?? false,
             cacheEntry);

        private void UpdateCacheEntry(string cacheKey, HttpResponseMessage httpResponseMessage)
        {
            var cacheEntry = new CacheEntry(
                                    httpResponseMessage,
                                    _dateTimeProvider.UtcNow(),
                                    _cacheConfiguration.CacheItemStaleTime);
            var cacheItem = _memoryCache.CreateEntry(cacheEntry);
            _memoryCache.Set(cacheKey, cacheItem);
        }

        public async Task InvokeAsync(PipelineDelegate next, ICachingContext context)
        {
            var req = context.HttpRequestMessage;
            var isCacheable = req.Method == HttpMethod.Get && context.UseCache;                

            if (isCacheable)
            {
                var cacheKey = GenerateCachekey(req);
                var (isCached, isStale, cacheEntry) = GetCacheEntry(cacheKey);

                if (isCached && !isStale)
                {
                    context.FromCache = true;
                    context.HttpResponseMessage = cacheEntry.HttpResponseMessage;                    
                    return;
                }

                await next();

                var resp = context.HttpResponseMessage;                
                if (resp?.IsSuccessStatusCode ?? false)
                {
                    if (isCached)
                    {
                        context.FromCache = true;
                        context.HttpResponseMessage = cacheEntry.HttpResponseMessage;
                    }
                    else
                    {
                        UpdateCacheEntry(cacheKey, resp);
                    }
                }

                return;
            }
            
            await next();
        }
    }

    internal class CacheEntry
    {
        public HttpResponseMessage HttpResponseMessage { get; set; }
        public DateTime Created { get; set; }
        public TimeSpan StaleTimeout { get; set; }

        public CacheEntry(HttpResponseMessage httpResponseMessage, DateTime created, TimeSpan staleTimeout)
        {
            HttpResponseMessage = httpResponseMessage;
            Created = created;
            StaleTimeout = staleTimeout;
        }

        public bool IsStale(IDateTimeProvider dateTimeProvider) =>
            Created + StaleTimeout > dateTimeProvider.UtcNow();
    }
}
