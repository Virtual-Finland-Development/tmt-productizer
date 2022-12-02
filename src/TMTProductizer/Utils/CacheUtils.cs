using System.IO.Compression;
using System.Text;
using TMTProductizer.Exceptions;
using TMTProductizer.Models.Cache;

namespace TMTProductizer.Utils;

public static class CacheUtils
{
    /// <summary>
    /// Get a cache key that is unique to the type of the cache value.
    /// </summary>
    public static string GetTypedCacheKey<T>(string cacheKey)
    {
        Type t = typeof(T);
        var typedKey = cacheKey + "::" + t.ToString();
        return typedKey;
    }

    /// <summary>
    /// Read cached content stream.
    /// </summary>
    public static CachedDataContainer? ReadCachedContentStream(Stream responseStream)
    {
        using (var decompressedStream = DecompressStream(responseStream))
        using (StreamReader reader = new StreamReader(decompressedStream, Encoding.UTF8))
        {
            string contents = reader.ReadToEnd();
            try
            {
                // Try to serialize the cache item from the container
                return StringUtils.JsonDeserializeObject<CachedDataContainer>(contents);
            }
            catch (JSONParseException)
            {
                // Ignore: the cache item is not a container, will be overwritten the next query where saving succeeds
                return default(CachedDataContainer);
            }
        }
    }

    /// <summary>
    /// Resolve cache item from the cache container.
    /// </summary>
    public static T? GetCacheItemFromContainer<T>(CachedDataContainer cacheItemContainer)
    {
        try
        {
            if (cacheItemContainer != null && (cacheItemContainer.TimeToLive == null || cacheItemContainer.TimeToLive > DateTimeOffset.UtcNow.ToUnixTimeSeconds()))
            {
                return StringUtils.JsonDeserializeObject<T>(cacheItemContainer.CacheValue, true);
            }
            return default(T);
        }
        catch (JSONParseException)
        {
            // Ignore: the cache item is not a container, will be overwritten the next query where saving succeeds
            return default(T);
        }
    }

    /// <summary>
    /// Compress stream using GZip. Leave the stream open.
    /// @see: https://stackoverflow.com/a/39157149
    /// </summary>
    public static Stream CompressSteram(Stream decompressed)
    {
        var compressed = new MemoryStream();
        using (var zip = new GZipStream(compressed, CompressionLevel.Optimal, true))
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
    public static Stream DecompressStream(Stream compressed)
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