using System.Text.Json;
using Newtonsoft.Json;
using TMTProductizer.Exceptions;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace TMTProductizer.Utils;

public static class StringUtils
{
    /// <summary>
    /// JSON serialize object using newtonsoft tools.
    /// </summary>
    public static string JsonSerializeObject<T>(T obj, bool useNewtonsoftJsonSerializer = false)
    {
        if (useNewtonsoftJsonSerializer)
        {
            return JsonConvert.SerializeObject(obj, typeof(T), GetNewtonsoftJsonSerializerSettings());
        }
        return JsonSerializer.Serialize<T>(obj, GetJsonSerializerOptions());
    }

    /// <summary>
    /// JSON deserialize using newtonsoft tools.
    /// </summary>
    public static T JsonDeserializeObject<T>(string json, bool useNewtonsoftJsonSerializer = false)
    {
        if (useNewtonsoftJsonSerializer)
        {
            return JsonConvert.DeserializeObject<T>(json, GetNewtonsoftJsonSerializerSettings()) ?? throw new JSONParseException("Failed to deserialize json");
        }
        return JsonSerializer.Deserialize<T>(json, GetJsonSerializerOptions()) ?? throw new JSONParseException("Failed to deserialize json");
    }

    /// <summary>
    /// Get shared newjson serialization settings
    /// </summary>
    public static JsonSerializerOptions GetJsonSerializerOptions()
    {
        return new JsonSerializerOptions
        {
            WriteIndented = false,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };
    }

    /// <summary>
    /// Get shared newjson serialization settings
    /// </summary>
    public static JsonSerializerSettings GetNewtonsoftJsonSerializerSettings()
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