using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using System.Text.Json;
using TMTProductizer.Utils;

namespace TMTProductizer.Services.AWS;

public class DynamoDBCacheTableItem
{
    public string CacheKey { get; set; } = null!;
    public string CacheValue { get; set; } = null!; // JSON string
    public Int64 UpdatedAt { get; set; } // Unix timestamp
}

public class DynamoDBCache : IDynamoDBCache
{
    IDynamoDBContext _DDBContext { get; set; }

    public DynamoDBCache(string dynamoDbCacheName)
    {
        if (string.IsNullOrEmpty(dynamoDbCacheName))
        {
            throw new ArgumentNullException("DynamoDB cache table name is null or empty.");
        }

        AWSConfigsDynamoDB.Context.TypeMappings[typeof(DynamoDBCacheTableItem)] = new Amazon.Util.TypeMapping(typeof(DynamoDBCacheTableItem), dynamoDbCacheName);
        var config = new DynamoDBContextConfig { Conversion = DynamoDBEntryConversion.V2 };
        _DDBContext = new DynamoDBContext(new AmazonDynamoDBClient(), config);
    }

    public async Task<T?> GetCacheItem<T>(string cacheKey)
    {
        var typedCacheKey = StringUtils.GetTypedCacheKey<T>(cacheKey);

        var cacheItem = await _DDBContext.LoadAsync<DynamoDBCacheTableItem>(typedCacheKey);
        if (cacheItem != null)
        {
            return JsonSerializer.Deserialize<T>(cacheItem.CacheValue);
        }
        return default(T);
    }

    public async Task SaveCacheItem<T>(string cacheKey, T cacheValue)
    {
        var cacheTextValue = JsonSerializer.Serialize(cacheValue);
        var typedCacheKey = StringUtils.GetTypedCacheKey<T>(cacheKey);

        await _DDBContext.SaveAsync<DynamoDBCacheTableItem>(new DynamoDBCacheTableItem
        {
            CacheKey = typedCacheKey,
            CacheValue = cacheTextValue,
            UpdatedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        });
    }
}