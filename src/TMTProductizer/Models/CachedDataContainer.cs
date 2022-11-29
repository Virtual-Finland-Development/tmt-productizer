
using System.Text.Json;
using TMTProductizer.Utils;

namespace TMTProductizer.Models;

public class CachedDataContainer
{
    public string CacheKey { get; set; } = null!;
    public string CacheValue { get; set; } = null!; // JSON string
    public Int64 UpdatedAt { get; set; } // Unix timestamp
    public Int64? TimeToLive { get; set; } // TTL timestamp

    public static CachedDataContainer FromCacheItem<T>(string cacheKey, T cacheValue, int expiresInSeconds = 0)
    {
        var cacheTextValue = JsonSerializer.Serialize<T>(cacheValue);
        var typedCacheKey = StringUtils.GetTypedCacheKey<T>(cacheKey);
        return new CachedDataContainer
        {
            CacheKey = typedCacheKey,
            CacheValue = cacheTextValue,
            UpdatedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
            TimeToLive = expiresInSeconds > 0 ? DateTimeOffset.UtcNow.ToUnixTimeSeconds() + expiresInSeconds : null
        };
    }
}