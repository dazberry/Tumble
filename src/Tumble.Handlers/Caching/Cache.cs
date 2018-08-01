using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Tumble.Core;
using Tumble.Core.Providers;

namespace Tumble.Handlers.Caching
{

    public class CacheConfiguration
    {
        public TimeSpan CacheItemTotalLifeTime { get; set; }
        public TimeSpan CacheItemStaleTime { get; set; }
    }
    
    public enum CacheEnum { NoCache, FromCache };    

    public class Cache : IPipelineHandler
    {
        private readonly IMemoryCache _memoryCache;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly CacheConfiguration _cacheConfiguration;

        public Cache(IMemoryCache memoryCache, IDateTimeProvider dateTimeProvider, CacheConfiguration cacheConfiguration)
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
                  
        public async Task InvokeAsync(PipelineContext context, PipelineDelegate next)
        {
            if (context.GetFirst(out HttpRequestMessage httpRequestMessage))
            {
                var isCacheable =
                    (httpRequestMessage.Method == HttpMethod.Get) &&
                    (!context.Get<CacheEnum>().Any(x => x == CacheEnum.FromCache));

                if (isCacheable)
                {
                    var cacheKey = GenerateCachekey(httpRequestMessage);
                    var (isCached, isStale, cacheEntry) = GetCacheEntry(cacheKey);

                    if (isCached && !isStale)
                    {
                        context
                            .Add(cacheEntry.HttpResponseMessage)
                            .Add(CacheEnum.FromCache);
                        
                        return;
                    }

                    await next();

                    if (context.GetFirst(out HttpResponseMessage httpResponseMessage))
                    {
                        if (!httpResponseMessage.IsSuccessStatusCode)
                        {
                            if (isCached)
                            {
                                context
                                    .AddOrReplace(cacheEntry.HttpResponseMessage)
                                    .Add(CacheEnum.FromCache);
                            }
                        }
                        else
                        {
                            UpdateCacheEntry(cacheKey, httpResponseMessage);                            
                        }
                    }
                    return;
                }
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
