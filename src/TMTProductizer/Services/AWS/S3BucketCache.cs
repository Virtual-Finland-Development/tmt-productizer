using Amazon.S3;
using Amazon.S3.Model;
using System.Text;
using TMTProductizer.Models.Cache;
using TMTProductizer.Utils;

namespace TMTProductizer.Services.AWS;

public class S3BucketCache : IS3BucketCache
{
    private readonly IAmazonS3 _s3client;
    private readonly string _bucketName;
    private readonly ILogger<S3BucketCache> _logger;
    private readonly ILocalFileCache _localFileCache;

    public S3BucketCache(IConfiguration configuration, ILogger<S3BucketCache> logger, ILocalFileCache localFileCache)
    {
        _bucketName = configuration.GetSection("S3BucketCacheName").Value;
        _logger = logger;
        _s3client = new AmazonS3Client();
        _localFileCache = localFileCache;
    }

    /// <summary>
    /// Search cache by key, filter by TTL.
    /// </summary>
    public async Task<T?> GetCacheItem<T>(string cacheKey)
    {
        // First try from the faster local cache
        var response = await _localFileCache.GetCacheItem<T>(cacheKey);
        if (response != null)
        {
            return response;
        }

        // If not found, try from S3
        return await GetCacheItemFromS3Bucket<T>(cacheKey);
    }

    /// <summary>
    /// Fetch cache item from S3 bucket, store to local cache if success
    /// </summary>
    private async Task<T?> GetCacheItemFromS3Bucket<T>(string cacheKey)
    {
        var typedCacheKey = CacheUtils.GetTypedCacheKey<T>(cacheKey);
        var cacheFileName = $"{typedCacheKey}.json.gz";

        // Create a GetObject request
        var request = new GetObjectRequest
        {
            BucketName = _bucketName,
            Key = cacheFileName,
        };

        _logger.LogInformation("Get from S3 cache: {cacheFileName}", cacheFileName);

        try
        {
            using (GetObjectResponse response = await _s3client.GetObjectAsync(request))
            {
                if (response.HttpStatusCode != System.Net.HttpStatusCode.OK)
                {
                    _logger.LogWarning("Bad response for cache key: {cacheFileName}", cacheFileName);
                    return default(T);
                }

                var cacheContainer = CacheUtils.ReadCachedContentStream(response.ResponseStream);
                if (cacheContainer != null)
                {
                    var cacheItem = CacheUtils.GetCacheItemFromContainer<T>(cacheContainer);
                    if (cacheItem != null)
                    {
                        // Save to local cache for faster access next time
                        await _localFileCache.SaveCacheItem<T>(cacheKey, cacheItem);
                    }
                    return cacheItem;
                }

                return default(T);
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error when fetching cache item from S3: {cacheFileName}", cacheFileName);
            return default(T);
        }
    }

    /// <summary>
    /// Save cache item to S3 bucket.
    /// </summary>
    public async Task SaveCacheItem<T>(string cacheKey, T cacheValue, int expiresInSeconds = 0)
    {
        // Transform data value to a known cache container type
        var cachedDataContainer = CachedDataContainer.FromCacheItem<T>(cacheKey, cacheValue, expiresInSeconds, true);
        var cacheFileName = $"{cachedDataContainer.CacheKey}.json.gz";
        var cacheTextValue = StringUtils.JsonSerializeObject<CachedDataContainer>(cachedDataContainer);

        using (var decompressed = new MemoryStream(Encoding.UTF8.GetBytes(cacheTextValue)))
        using (var compressed = CacheUtils.CompressSteram(decompressed))
        {
            // Upload the json object
            var request = new PutObjectRequest
            {
                BucketName = _bucketName,
                Key = cacheFileName,
                InputStream = compressed,
                ContentType = "application/json",
            };

            await _s3client.PutObjectAsync(request);
        }
    }
}