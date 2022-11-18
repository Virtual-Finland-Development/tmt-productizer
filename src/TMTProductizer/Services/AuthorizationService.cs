using System.Net;

namespace TMTProductizer.Services;

public class AuthorizationService : IAuthorizationService
{
    private readonly HttpClient _client;
    private readonly bool _skipAuthorizationCheck;

    public AuthorizationService(HttpClient client, IHostEnvironment env)
    {
        _client = client;
        _skipAuthorizationCheck = env.IsDevelopment() || env.IsEnvironment("Mock");
    }

    public async Task Authorize(HttpRequest request)
    {
        // Get the auth headers from the origin request
        var originHeaders = request.Headers.ToDictionary(x => x.Key.ToLowerInvariant(), x => x.Value.ToString());
        
        // Prep authorization request
        if (!originHeaders.ContainsKey("authorization") || !originHeaders.ContainsKey("x-authorization-provider")) {
            throw new HttpRequestException("Missing headers", null, HttpStatusCode.Unauthorized); // Throws 401 if no auth headers
        }

        // Skip on local development, but require headers present (above)
        if (_skipAuthorizationCheck) return;

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
