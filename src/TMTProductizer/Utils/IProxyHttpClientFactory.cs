using TMTProductizer.Models;

namespace TMTProductizer.Utils;

public interface IProxyHttpClientFactory
{
    public Uri BaseAddress { get; set; }
    public HttpClient GetProxyClient(AuthorizationPackage authorizationPackage);
}
