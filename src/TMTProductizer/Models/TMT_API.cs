using System.Text.Json.Serialization;

namespace TMTProductizer.Models;

public class TMTSecrets
{
    [JsonPropertyName("CLIENT_ID")]
    public string? ClientId { get; set; }
    [JsonPropertyName("CLIENT_SECRET")]
    public string? ClientSecret { get; set; }
    [JsonPropertyName("PROXY_ADDRESS")]
    public string? ProxyAddress { get; set; }
    [JsonPropertyName("PROXY_USER")]
    public string? ProxyUser { get; set; }
    [JsonPropertyName("PROXY_PASSWORD")]
    public string? ProxyPassword { get; set; }
}

public class TMTAuthorizationResponse
{
    [JsonPropertyName("access_token")]
    public string? AccessToken { get; set; }
    [JsonPropertyName("token_type")]
    public string? TokenType { get; set; }
    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }
    [JsonPropertyName("expires_on")]
    public int ExpiresOn { get; set; }
    [JsonPropertyName("not_before")]
    public int NotBefore { get; set; }
    [JsonPropertyName("resource")]
    public string? Resource { get; set; }
}


public class TMTAuthorizationDetails
{
    public string? AccessToken { get; set; }
    public string? ProxyAddress { get; set; }
    public string? ProxyUser { get; set; }
    public string? ProxyPassword { get; set; }
    
}
