using Amazon.S3;
using Amazon.S3.Model;
using System.Text;
using System.Text.Json;
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
                    cacheItem = JsonSerializer.Deserialize<T>(contents);
                }
                catch (JsonException)
                {
                    // Ignore
                }
            }
        }

        return cacheItem;
    }

    public async Task<bool> SaveCacheItem<T>(string cacheKey, T cacheValue, int expiresInSeconds = 0)
    {
        var cacheTextValue = JsonSerializer.Serialize(cacheValue);
        var typedCacheKey = StringUtils.GetTypedCacheKey<T>(cacheKey);

        var request = new PutObjectRequest
        {
            BucketName = _bucketName,
            Key = typedCacheKey,
            InputStream = new MemoryStream(Encoding.UTF8.GetBytes(cacheTextValue)),
            ContentType = "application/json",
        };

        var response = await _client.PutObjectAsync(request);

        return response.HttpStatusCode == System.Net.HttpStatusCode.OK;
    }
}