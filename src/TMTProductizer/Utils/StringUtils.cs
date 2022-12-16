using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using TMTProductizer.Exceptions;

namespace TMTProductizer.Utils;

public static class StringUtils
{
    /// <summary>
    /// JSON serialize object using newtonsoft tools.
    /// </summary>
    public static string JsonSerializeObject<T>(T obj)
    {
        return JsonConvert.SerializeObject(obj, typeof(T), GetNewtonsoftJsonSerializerSettings());
    }

    /// <summary>
    /// JSON deserialize using newtonsoft tools.
    /// </summary>
    public static T JsonDeserializeObject<T>(string json)
    {
        return JsonConvert.DeserializeObject<T>(json, GetNewtonsoftJsonSerializerSettings()) ?? throw new JSONParseException("Failed to deserialize json");
    }

    /// <summary>
    /// Get shared newjson serialization settings
    /// </summary>
    public static JsonSerializerSettings GetNewtonsoftJsonSerializerSettings()
    {
        return new JsonSerializerSettings
        {
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
            MissingMemberHandling = MissingMemberHandling.Ignore,
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy
                {
                    OverrideSpecifiedNames = false
                }
            }
        };
    }
}