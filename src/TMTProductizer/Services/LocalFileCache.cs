using TMTProductizer.Models.Cache;
using TMTProductizer.Utils;

namespace TMTProductizer.Services;

public class LocalFileCache : ILocalFileCache
{
    private readonly ILogger<LocalFileCache> _logger;

    public LocalFileCache(ILogger<LocalFileCache> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Search cache by key, filter by TTL.
    /// </summary>
    public async Task<T?> GetCacheItem<T>(string cacheKey)
    {
        var typedCacheKey = CacheUtils.GetTypedCacheKey<T>(cacheKey);
        var cacheFileName = $"{typedCacheKey}.json";

        _logger.LogInformation("Get from local cache: {cacheFileName}", cacheFileName);
        var cachePath = Path.Combine(Path.GetTempPath(), cacheFileName);
        if (File.Exists(cachePath))
        {
            try
            {
                using (var fs = new StreamReader(cachePath))
                {
                    var contents = fs.ReadToEnd();
                    var cacheContainer = StringUtils.JsonDeserializeObject<CachedDataContainer>(contents);
                    var cacheItem = CacheUtils.GetCacheItemFromContainer<T>(cacheContainer);
                    if (cacheItem != null)
                    {
                        _logger.LogInformation("Local cache match: {cacheFileName}", cacheFileName);
                        return await Task.FromResult(cacheItem);
                    }

                    _logger.LogInformation("Local cache file {cacheFileName} is too old, delete it", cacheFileName);
                    File.Delete(cachePath);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error operating on a local cache file");
            }
        }

        _logger.LogInformation("No local cache layer for: {cacheFileName}", cacheFileName);
        return await Task.FromResult(default(T));
    }

    /// <summary>
    /// Save cache item to local tmp file.
    /// </summary>
    public async Task SaveCacheItem<T>(string cacheKey, T cacheValue, int expiresInSeconds = 0)
    {
        var typedCacheKey = CacheUtils.GetTypedCacheKey<T>(cacheKey);
        var cacheFileName = $"{typedCacheKey}.json";

        _logger.LogInformation("Saving to local cache: {cacheFileName}", cacheFileName);

        // Transform data value to a known cache container type
        var cachedDataContainer = CachedDataContainer.FromCacheItem<T>(cacheKey, cacheValue, expiresInSeconds);
        var cacheTextValue = StringUtils.JsonSerializeObject<CachedDataContainer>(cachedDataContainer);
        var cachePath = Path.Combine(Path.GetTempPath(), cacheFileName);

        try
        {
            // Delete the file if it exists.
            if (File.Exists(cachePath))
            {
                File.Delete(cachePath);
            }

            using (StreamWriter fileStream = new StreamWriter(cachePath))
            {
                await fileStream.WriteAsync(cacheTextValue);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error writing on a local cache file");
        }
    }
}