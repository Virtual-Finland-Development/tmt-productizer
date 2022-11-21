namespace TMTProductizer.Services.AWS;

public interface IDynamoDBCache
{
    Task<T?> GetCacheItem<T>(string cacheKey);

    Task SaveCacheItem<T>(string cacheKey, T cacheValue);
}
