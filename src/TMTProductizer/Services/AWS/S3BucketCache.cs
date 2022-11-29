using Amazon.S3;
using Amazon.S3.Model;
using System.Text;
using System.Text.Json;
using TMTProductizer.Models;
using TMTProductizer.Utils;
namespace TMTProductizer.Services.AWS;

public class S3BucketCache : IS3BucketCache
{
    private IAmazonS3 _client { get; set; }

    private string _bucketName;

    public S3BucketCache(string bucketName)
    {
        _bucketName = bucketName;
        _client = new AmazonS3Client();
    }

    /// <summary>
    /// Search cache by key, filter by TTL.
    /// </summary>
    public async Task<T?> GetCacheItem<T>(string cacheKey)
    {
        var cacheItem = default(T);

        var typedCacheKey = StringUtils.GetTypedCacheKey<T>(cacheKey);

        // Create a GetObject request
        var request = new GetObjectRequest
        {
            BucketName = _bucketName,
            Key = $"{typedCacheKey}.json",
        };

        // Issue request and remember to dispose of the response
        using (GetObjectResponse response = await _client.GetObjectAsync(request))
        {
            using (StreamReader reader = new StreamReader(response.ResponseStream))
            {
                string contents = reader.ReadToEnd();
                try
                {
                    // Try to serialize the cache item from the container
                    var cacheItemContainer = JsonSerializer.Deserialize<CachedDataContainer>(contents);
                    if (cacheItemContainer != null && (cacheItemContainer.TimeToLive == null || cacheItemContainer.TimeToLive > DateTimeOffset.UtcNow.ToUnixTimeSeconds()))
                    {
                        cacheItem = JsonSerializer.Deserialize<T>(cacheItemContainer.CacheValue);
                    }
                }
                catch (JsonException)
                {
                    // Ignore: the cache item is not a container, will be overwritten the next query
                }
            }
        }

        return cacheItem;
    }

    /// <summary>
    /// Save cache item to S3 bucket.
    /// </summary>
    public async Task SaveCacheItem<T>(string cacheKey, T cacheValue, int expiresInSeconds = 0)
    {
        // Transform data value to a known cache container type
        var cachedDataContainer = CachedDataContainer.FromCacheItem<T>(cacheKey, cacheValue, expiresInSeconds);
        var cacheTextValue = JsonSerializer.Serialize<CachedDataContainer>(cachedDataContainer);

        // Upload the json object
        var request = new PutObjectRequest
        {
            BucketName = _bucketName,
            Key = cachedDataContainer.CacheKey,
            InputStream = new MemoryStream(Encoding.UTF8.GetBytes(cacheTextValue)),
            ContentType = "application/json",
        };

        await _client.PutObjectAsync(request);
    }
}