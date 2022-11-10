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
        if (!originHeaders.ContainsKey("authorization") || !originHeaders.ContainsKey("x-authorization-provider")) {
            throw new HttpRequestException(); // Throws 401 if no auth headers.
        }

        // Prep authorization request
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
            throw new HttpRequestException(); // Throw 401 if not authorized.
        }

        return null;
    }
}
