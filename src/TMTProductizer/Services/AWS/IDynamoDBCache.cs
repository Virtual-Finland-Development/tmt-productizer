namespace TMTProductizer.Services.AWS;

public interface IDynamoDBCache
{
    /// <summary>
    /// Get a cache item from DynamoDB, or null if it doesn't exist.
    /// </summary>
    Task<T?> GetCacheItem<T>(string cacheKey);

    /// <summary>
    /// Converts item to json string and save to DynamoDB
    /// <param>expiresInSeconds: 0 means no expiration, >0 means that after expiration the key is removed by dynamodb in the next 48h</param>
    /// </summary>
    Task SaveCacheItem<T>(string cacheKey, T cacheValue, int expiresInSeconds = 0);
}
