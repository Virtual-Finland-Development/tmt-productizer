using System.Runtime.Serialization;

namespace TMTProductizer.Models;

[DataContract(Name = "TMTSecrets")]
public class TMTSecrets
{
    [DataMember(Name = "CLIENT_ID")]
    public string ClientId { get; set; } = null!;
    [DataMember(Name = "CLIENT_SECRET")]
    public string ClientSecret { get; set; } = null!;
    [DataMember(Name = "PROXY_ADDRESS")]
    public string ProxyAddress { get; set; } = null!;
    [DataMember(Name = "PROXY_USER")]
    public string ProxyUser { get; set; } = null!;
    [DataMember(Name = "PROXY_PASSWORD")]
    public string ProxyPassword { get; set; } = null!;
}

[DataContract(Name = "TMTAPIAuthorizationResponse")]
public class TMTAPIAuthorizationResponse
{
    [DataMember(Name = "access_token")]
    public string? AccessToken { get; set; }
    [DataMember(Name = "token_type")]
    public string? TokenType { get; set; }
    [DataMember(Name = "expires_in")]
    public int ExpiresIn { get; set; }
    [DataMember(Name = "expires_on")]
    public int ExpiresOn { get; set; }
    [DataMember(Name = "not_before")]
    public int NotBefore { get; set; }
    [DataMember(Name = "resource")]
    public string? Resource { get; set; }
}
