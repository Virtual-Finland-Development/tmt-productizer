namespace TMTProductizer.Services;

public class AuthorizationService : IAuthorizationService
{
    private readonly HttpClient _client;
    
    public AuthorizationService(HttpClient client)
    {
        _client = client;
    }

    public async Task<object?> Authorize(HttpRequest request)
    {
        // Get the auth headers from the origin request
        var originHeaders = request.Headers.ToDictionary(x => x.Key.ToLowerInvariant(), x => x.Value.ToString());
        
        // Check CORS origin against allowed origins
        if (!originHeaders.TryGetValue("origin", out var origin) || !IsAllowedOrigin(origin))
        {
            throw new HttpRequestException("Unknown origin", null, System.Net.HttpStatusCode.Unauthorized); // Throws 401 if bad origin
        }

        // Prep authorization request
        if (!originHeaders.ContainsKey("authorization") || !originHeaders.ContainsKey("x-authorization-provider")) {
            throw new HttpRequestException("Missing headers", null, System.Net.HttpStatusCode.Unauthorized); // Throws 401 if no auth headers
        }

        var authorizeRequest = new HttpRequestMessage() {
            RequestUri = new Uri($"{_client.BaseAddress}/authorize"),
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
            throw new HttpRequestException("Access Denied", null, System.Net.HttpStatusCode.Unauthorized); // Throw 401 if not authorized.
        }

        return null;
    }

    /**
     * Check if the origin is allowed.
     */
    private bool IsAllowedOrigin(string origin)
    {
        return origin == "https://gateway.testbed.fi";
    }
}
