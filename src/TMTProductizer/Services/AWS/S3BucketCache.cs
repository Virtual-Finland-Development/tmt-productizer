using Amazon.S3;
using Amazon.S3.Model;
using System.IO.Compression;
using System.Text;
using TMTProductizer.Models;
using TMTProductizer.Utils;
namespace TMTProductizer.Services.AWS;

public class S3BucketCache : IS3BucketCache
{
    private IAmazonS3 _s3client { get; set; }

    private string _bucketName;
    private ILogger<S3BucketCache> _logger;

    public S3BucketCache(IConfiguration configuration, ILogger<S3BucketCache> logger)
    {
        _bucketName = configuration.GetSection("S3BucketCacheName").Value;
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
            Key = $"{typedCacheKey}.json.gz",
        };

        // Issue request and remember to dispose of the response
        try
        {
            using (GetObjectResponse response = await _s3client.GetObjectAsync(request))
            {
                if (response.HttpStatusCode != System.Net.HttpStatusCode.OK)
                {
                    _logger.LogWarning($"Bad response for cache key: {typedCacheKey}", typedCacheKey);
                    return default(T);
                }

                using (var decompressedStream = DecompressStream(response.ResponseStream))
                using (StreamReader reader = new StreamReader(decompressedStream, Encoding.UTF8))
                {
                    string contents = reader.ReadToEnd();
                    try
                    {
                        // Try to serialize the cache item from the container
                        var cacheItemContainer = StringUtils.JsonDeserializeObject<CachedDataContainer>(contents);
                        if (cacheItemContainer != null && (cacheItemContainer.TimeToLive == null || cacheItemContainer.TimeToLive > DateTimeOffset.UtcNow.ToUnixTimeSeconds()))
                        {
                            return StringUtils.JsonDeserializeObject<T>(cacheItemContainer.CacheValue, true);
                        }
                        _logger.LogInformation($"Cache miss for {typedCacheKey}", typedCacheKey);
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
        var cachedDataContainer = CachedDataContainer.FromCacheItem<T>(cacheKey, cacheValue, expiresInSeconds, true);
        var cacheTextValue = StringUtils.JsonSerializeObject<CachedDataContainer>(cachedDataContainer);

        using (var decompressed = new MemoryStream(Encoding.UTF8.GetBytes(cacheTextValue)))
        using (var inputStream = CompressSteram(decompressed))
        {
            // Upload the json object
            var request = new PutObjectRequest
            {
                BucketName = _bucketName,
                Key = $"{cachedDataContainer.CacheKey}.json.gz",
                InputStream = inputStream,
                ContentType = "application/json",
            };

            await _s3client.PutObjectAsync(request);
        }
    }

    /// <summary>
    /// Compress stream using GZip. Leave the stream open.
    /// @see: https://stackoverflow.com/a/39157149
    /// </summary>
    private Stream CompressSteram(Stream decompressed)
    {
        var compressed = new MemoryStream();
        using (var zip = new GZipStream(compressed, CompressionLevel.SmallestSize, true))
        {
            decompressed.CopyTo(zip);
        }
        compressed.Seek(0, SeekOrigin.Begin);
        return compressed;
    }

    /// <summary>
    /// Decompress stream using GZip. Leave the stream open.
    /// @see: https://stackoverflow.com/a/39157149
    /// </summary>
    private Stream DecompressStream(Stream compressed)
    {
        var decompressed = new MemoryStream();
        using (var zip = new GZipStream(compressed, CompressionMode.Decompress))
        {
            zip.CopyTo(decompressed);
        }
        decompressed.Seek(0, SeekOrigin.Begin);
        return decompressed;
    }
}