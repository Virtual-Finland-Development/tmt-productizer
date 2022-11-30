using Amazon.S3;
using Amazon.S3.Model;
using System.Text;
using TMTProductizer.Models;
using TMTProductizer.Utils;
namespace TMTProductizer.Services.AWS;

public class S3BucketCache : IS3BucketCache
{
    private IAmazonS3 _s3client { get; set; }

    private string _bucketName;
    private ILogger<S3BucketCache> _logger;

    public S3BucketCache(string bucketName, ILogger<S3BucketCache> logger)
    {
        _bucketName = bucketName;
        _logger = logger;
        _s3client = new AmazonS3Client();
    }

    /// <summary>
    /// Search cache by key, filter by TTL.
    /// </summary>
    public async Task<T?> GetCacheItem<T>(string cacheKey)
    {
        var typedCacheKey = StringUtils.GetTypedCacheKey<T>(cacheKey);

        // Create a GetObject request
        var request = new GetObjectRequest
        {
            BucketName = _bucketName,
            Key = $"{typedCacheKey}.json",
        };

        // Issue request and remember to dispose of the response
        try
        {
            using (GetObjectResponse response = await _s3client.GetObjectAsync(request))
            {
                if (response.HttpStatusCode != System.Net.HttpStatusCode.OK)
                {
                    _logger.LogWarning($"Bad response for cache key: {typedCacheKey}");
                    return default(T);
                }

                using (StreamReader reader = new StreamReader(response.ResponseStream, Encoding.UTF8))
                {
                    string contents = reader.ReadToEnd();
                    try
                    {
                        // Try to serialize the cache item from the container
                        var cacheItemContainer = StringUtils.JsonDeserialiseObject<CachedDataContainer>(contents);
                        if (cacheItemContainer != null && (cacheItemContainer.TimeToLive == null || cacheItemContainer.TimeToLive > DateTimeOffset.UtcNow.ToUnixTimeSeconds()))
                        {
                            return StringUtils.JsonDeserialiseObject<T>(cacheItemContainer.CacheValue);
                        }
                        _logger.LogInformation($"Cache miss for {typedCacheKey}");
                        return default(T);
                    }
                    catch (Exception)
                    {
                        // Ignore: the cache item is not a container, will be overwritten the next query where saving succeeds
                        _logger.LogInformation("Bad cache item, will be overwritten.");
                        return default(T);
                    }
                }
            }
        }
        catch (AmazonS3Exception)
        {
            _logger.LogWarning("Error when fetching cache item from S3");
            return default(T);
        }
    }

    /// <summary>
    /// Save cache item to S3 bucket.
    /// </summary>
    public async Task SaveCacheItem<T>(string cacheKey, T cacheValue, int expiresInSeconds = 0)
    {
        // Transform data value to a known cache container type
        var cachedDataContainer = CachedDataContainer.FromCacheItem<T>(cacheKey, cacheValue, expiresInSeconds);
        var cacheTextValue = StringUtils.JsonSerialiseObject<CachedDataContainer>(cachedDataContainer);

        // Upload the json object
        var request = new PutObjectRequest
        {
            BucketName = _bucketName,
            Key = $"{cachedDataContainer.CacheKey}.json",
            InputStream = new MemoryStream(Encoding.UTF8.GetBytes(cacheTextValue)),
            ContentType = "application/json",
        };

        await _s3client.PutObjectAsync(request);
    }
}