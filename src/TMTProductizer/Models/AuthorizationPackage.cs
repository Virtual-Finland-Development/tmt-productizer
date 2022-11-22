namespace TMTProductizer.Models;

public class AuthorizationPackage
{
    public string? AccessToken { get; set; }
    public int ExpiresOn { get; set; }
    public string? ProxyAddress { get; set; }
    public string? ProxyUser { get; set; }
    public string? ProxyPassword { get; set; }
}