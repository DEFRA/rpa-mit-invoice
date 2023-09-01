using EST.MIT.Invoice.Api.Services.Api.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace EST.MIT.Invoice.Api.Services.Api;

public class CacheService : ICacheService
{
    private readonly IMemoryCache _memoryCache;
    private readonly MemoryCacheEntryOptions _cacheEntryOptions;

    public CacheService(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
        _cacheEntryOptions = new MemoryCacheEntryOptions()
            .SetSlidingExpiration(TimeSpan.FromMinutes(30))
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(60))
            .SetPriority(CacheItemPriority.Normal);
    }

    public T GetData<T>(object key)
    {
        return (T)_memoryCache.Get(key);
    }
    public void SetData<T>(object key, T value)
    {
        _memoryCache.Set(key, value, _cacheEntryOptions);
    }

    public void RemoveData(object key)
    {
        _memoryCache.Remove(key);
    }
}
