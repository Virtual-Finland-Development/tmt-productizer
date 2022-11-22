using System.Net;
using TMTProductizer.Models;

namespace TMTProductizer.Utils;

public class ProxyHttpClientFactory : IProxyHttpClientFactory
{
    public Uri BaseAddress { get; set; }
    public ProxyHttpClientFactory(Uri baseAddress)
    {
        BaseAddress = baseAddress;
    }

    public HttpClient GetTMTProxyClient(TMTAPIAuthorizationDetails tmtAuthorizationDetails)
    {
        WebProxy? proxy = null;

        if (tmtAuthorizationDetails.ProxyAddress != null)
        {
            proxy = new WebProxy
            {
                Address = new Uri(tmtAuthorizationDetails.ProxyAddress),
                BypassProxyOnLocal = false,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(userName: tmtAuthorizationDetails.ProxyUser, password: tmtAuthorizationDetails.ProxyPassword)
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