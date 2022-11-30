using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using TMTProductizer.Models;
using TMTProductizer.Utils;

namespace TMTProductizer.Services.AWS;


public class DynamoDBCache : IDynamoDBCache
{
    private IDynamoDBContext _DDBContext { get; set; }

    public DynamoDBCache(string dynamoDbCacheName)
    {
        if (string.IsNullOrEmpty(dynamoDbCacheName))
        {
            throw new ArgumentNullException("DynamoDB cache table name is null or empty.");
        }

        AWSConfigsDynamoDB.Context.TypeMappings[typeof(CachedDataContainer)] = new Amazon.Util.TypeMapping(typeof(CachedDataContainer), dynamoDbCacheName);
        var config = new DynamoDBContextConfig { Conversion = DynamoDBEntryConversion.V2 };
        _DDBContext = new DynamoDBContext(new AmazonDynamoDBClient(), config);
    }

    /// <summary>
    /// Search cache by key, filter by TTL.
    /// </summary>
    public async Task<T?> GetCacheItem<T>(string cacheKey)
    {
        var typedCacheKey = StringUtils.GetTypedCacheKey<T>(cacheKey);

        var search = _DDBContext.FromScanAsync<CachedDataContainer>(new ScanOperationConfig
        {
            Limit = 1,
            FilterExpression = new Expression
            {
                ExpressionStatement = "CacheKey = :cacheKey AND (attribute_not_exists(TimeToLive) OR TimeToLive = :neverExpires OR TimeToLive > :now)",
                ExpressionAttributeValues = new Dictionary<string, DynamoDBEntry?> {
                    {":cacheKey", typedCacheKey},
                    {":now", DateTimeOffset.UtcNow.ToUnixTimeSeconds()},
                    {":neverExpires", null}
                }
            },
        });

        var cacheItem = default(T);
        do
        {
            var items = await search.GetNextSetAsync();
            if (items.Any<CachedDataContainer>())
            {
                cacheItem = StringUtils.JsonDeserialiseObject<T>(items[0].CacheValue);
                break;
            }
        } while (!search.IsDone);
        return cacheItem;
    }

    public async Task SaveCacheItem<T>(string cacheKey, T cacheValue, int expiresInSeconds = 0)
    {
        // Transform data value to a known cache container type
        var cachedDataContainer = CachedDataContainer.FromCacheItem<T>(cacheKey, cacheValue, expiresInSeconds);
        // Save to DynamoDB
        await _DDBContext.SaveAsync<CachedDataContainer>(cachedDataContainer);
    }
}