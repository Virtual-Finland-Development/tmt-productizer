using System.Net;
using TMTProductizer.Models;

namespace TMTProductizer.Utils;

public class ProxyHttpClientFactory : IProxyHttpClientFactory
{
    public Uri BaseAddress { get; set; }
    public ProxyHttpClientFactory(IConfiguration configuration)
    {
        BaseAddress = new Uri(configuration.GetSection("TmtApiEndpoint").Value);
    }

    public HttpClient GetProxyClient(APIAuthorizationPackage authorizationPackage)
    {
        WebProxy? proxy = null;

        if (authorizationPackage.ProxyAddress != null)
        {
            proxy = new WebProxy
            {
                Address = new Uri(authorizationPackage.ProxyAddress),
                BypassProxyOnLocal = false,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(userName: authorizationPackage.ProxyUser, password: authorizationPackage.ProxyPassword)
            };
        }

        var httpClientHandler = new HttpClientHandler
        {
            Proxy = proxy,
            UseProxy = true
        };
        var httpClient = new HttpClient(httpClientHandler);

        return httpClient;
    }
}