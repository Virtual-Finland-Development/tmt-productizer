using System.Net;

namespace TMTProductizer.Services;

public class AuthorizationService : IAuthorizationService
{
    private readonly HttpClient _client;
    private readonly bool _skipAuthorizationCeck;

    public AuthorizationService(HttpClient client, IWebHostEnvironment env)
    {
        _client = client;
        _skipAuthorizationCeck = env.IsDevelopment();
    }

    public async Task Authorize(HttpRequest request)
    {
        // Skip on local development
        if (this._skipAuthorizationCeck) return;

        // Get the auth headers from the origin request
        var originHeaders = request.Headers.ToDictionary(x => x.Key.ToLowerInvariant(), x => x.Value.ToString());
        
        // Prep authorization request
        if (!originHeaders.ContainsKey("authorization") || !originHeaders.ContainsKey("x-authorization-provider")) {
            throw new HttpRequestException("Missing headers", null, HttpStatusCode.Unauthorized); // Throws 401 if no auth headers
        }

        var authorizeRequest = new HttpRequestMessage {
            RequestUri = new Uri($"{_client.BaseAddress}authorize"),
            Method = HttpMethod.Post,
            Headers = {
                { "authorization", originHeaders["authorization"] },
                { "x-authorization-provider", originHeaders["x-authorization-provider"] },
                { "x-authorization-context", "TMT-productizer" },
            }
        };
        
        // Engage
        var response = await _client.SendAsync(authorizeRequest); 
        if (!response.IsSuccessStatusCode) {
            throw new HttpRequestException("Access Denied", null, HttpStatusCode.Unauthorized); // Throw 401 if not authorized.
        }
    }
}
