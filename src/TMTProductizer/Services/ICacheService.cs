namespace TMTProductizer.Services;

public interface ICacheService
{
    /// <summary>
    /// Get a cache item from the implementing service, or null if it doesn't exist.
    /// </summary>
    Task<T?> GetCacheItem<T>(string cacheKey);

    /// <summary>
    /// Converts item to json string and save to DynamoDB
    /// <param>expiresInSeconds: 0 means no expiration</param>
    /// </summary>
    Task<bool> SaveCacheItem<T>(string cacheKey, T cacheValue, int expiresInSeconds = 0);
}
