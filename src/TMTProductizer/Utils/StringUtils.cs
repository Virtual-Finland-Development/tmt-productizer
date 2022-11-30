
using Newtonsoft.Json;

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

    /// <summary>
    /// JSON Serialise object using newtonsoft tools.
    /// </summary>
    public static string JsonSerialiseObject<T>(T obj)
    {
        return JsonConvert.SerializeObject(obj, typeof(T), GetJsonSerializerSettings());
    }

    /// <summary>
    /// JSON Deserialise using newtonsoft tools.
    /// </summary>
    public static T? JsonDeserialiseObject<T>(string json)
    {
        return JsonConvert.DeserializeObject<T>(json, GetJsonSerializerSettings());
    }

    /// <summary>
    /// Get shared newjson serialization settings
    /// </summary>
    public static JsonSerializerSettings GetJsonSerializerSettings()
    {
        return new JsonSerializerSettings
        {
            Error = (sender, args) =>
            {
                if (System.Diagnostics.Debugger.IsAttached)
                {
                    System.Diagnostics.Debugger.Break();
                }
            }
        };
    }
}