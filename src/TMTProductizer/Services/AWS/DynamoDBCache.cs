using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using System.Text.Json;

using TMTProductizer.Models;

namespace TMTProductizer.Services.AWS;

public class DynamoDBCache : IDynamoDBCache
{
    // This const is the name of the environment variable that the serverless.template will use to set
    // the name of the DynamoDB table used to store blog posts.
    const string TABLENAME_ENVIRONMENT_VARIABLE_LOOKUP = "DYNAMODB_CACHE_TABLE_NAME";

    IDynamoDBContext _DDBContext { get; set; }

    public DynamoDBCache()
    {
        // Check to see if a table name was passed in through environment variables and if so 
        // add the table mapping.
        var tableName = System.Environment.GetEnvironmentVariable(TABLENAME_ENVIRONMENT_VARIABLE_LOOKUP);
        if (!string.IsNullOrEmpty(tableName))
        {
            AWSConfigsDynamoDB.Context.TypeMappings[typeof(DynamoDBCacheTableItem)] = new Amazon.Util.TypeMapping(typeof(DynamoDBCacheTableItem), tableName);
        }

        var config = new DynamoDBContextConfig { Conversion = DynamoDBEntryConversion.V2 };
        _DDBContext = new DynamoDBContext(new AmazonDynamoDBClient(), config);
    }

    public async Task<T?> GetCacheItem<T>(string cacheKey)
    {
        var cacheItem = await _DDBContext.LoadAsync<DynamoDBCacheTableItem>(cacheKey);
        if (cacheItem != null)
        {
            return JsonSerializer.Deserialize<T>(cacheItem.CacheValue);
        }
        return default(T);
    }

    public async Task SaveCacheItem<T>(string cacheKey, T cacheValue)
    {
        var cacheTextValue = JsonSerializer.Serialize(cacheValue);
        await _DDBContext.SaveAsync<DynamoDBCacheTableItem>(new DynamoDBCacheTableItem
        {
            CacheKey = cacheKey,
            CacheValue = cacheTextValue,
            CreatedAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        });
    }
}