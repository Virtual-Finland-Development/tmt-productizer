
namespace TMTProductizer.Utils;

public static class StringUtils
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
}