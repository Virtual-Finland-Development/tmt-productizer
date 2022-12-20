using System.Runtime.Serialization;

namespace TMTProductizer.Models;

[DataContract(Name = "APIAuthorizationPackage")]
public class APIAuthorizationPackage
{
    [DataMember(Name = "accessToken")]
    public string? AccessToken { get; set; }
    [DataMember(Name = "expiresOn")]
    public int ExpiresOn { get; set; }
    [DataMember(Name = "proxyAddress")]
    public string? ProxyAddress { get; set; }
    [DataMember(Name = "proxyUser")]
    public string? ProxyUser { get; set; }
    [DataMember(Name = "proxyPassword")]
    public string? ProxyPassword { get; set; }
}