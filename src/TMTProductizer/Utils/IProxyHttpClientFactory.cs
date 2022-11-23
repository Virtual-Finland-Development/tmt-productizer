namespace TMTProductizer.Utils;
using TMTProductizer.Models;

public interface IProxyHttpClientFactory
{
    public Uri BaseAddress { get; set; }
    public HttpClient GetProxyClient(APIAuthorizationPackage authorizationPackage);
}
