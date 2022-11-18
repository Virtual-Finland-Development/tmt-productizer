using System.Net;
using System.Text;
using System.Text.Json;
using TMTProductizer.Models;
using TMTProductizer.Services.AWS;
using TMTProductizer.Utils.DateUtils;

namespace TMTProductizer.Services.TMT;

public class TMTAuthorizationService : ITMTAuthorizationService
{
    private readonly HttpClient _client;
    private readonly ISecretsManager _secretsManager;
    private readonly ILogger<TMTAuthorizationService> _logger;
    private TMTAuthorizationDetails? _TMTAuthorizationDetails = null;
    private bool _skipAuthorizationCeck;

    public TMTAuthorizationService(HttpClient client, ISecretsManager secretsManager, ILogger<TMTAuthorizationService> logger, IHostEnvironment env)
    {
        _client = client;
        _secretsManager = secretsManager;
        _logger = logger;
        _skipAuthorizationCeck = env.IsEnvironment("Mock");

    }

    public async Task<TMTAuthorizationDetails> GetTMTAuthorizationDetails()
    {
        // Skip on local mock development
        if (_skipAuthorizationCeck) return new TMTAuthorizationDetails();

        // If we have a valid token in the current lambda instance, return it
        // @TODO: cache the token for time period over the lambda instance lifetime
        if (_TMTAuthorizationDetails != null && DateUtils.UnixTimeStampToDateTime(_TMTAuthorizationDetails.ExpiresOn) > DateTime.UtcNow)
        {
            return _TMTAuthorizationDetails;
        }
        _TMTAuthorizationDetails = await FetchTMTAuthorizationDetails();
        return _TMTAuthorizationDetails;
    }

    private async Task<TMTAuthorizationDetails> FetchTMTAuthorizationDetails()
    {
        // Fetch secrets
        TMTSecrets secrets = await _secretsManager.GetTMTSecrets(); // throws HttpRequestException

        // Prep authorization request
        // @see: https://learn.microsoft.com/en-us/azure/active-directory-b2c/authorization-code-flow#2-get-an-access-token
        var contentData = new Dictionary<string, string> {
            {"grant_type", "client_credentials"},
            {"client_id", secrets.ClientId},
            {"client_secret", secrets.ClientSecret},
            {"scope", "https://tedigib2c.onmicrosoft.com/fad9328b-e852-45e4-951b-6d142430e89d/.default"},
            {"response_type", "token"}
        };
        var formUrlEncodedContent = new FormUrlEncodedContent(contentData);
        var urlEncodedString = await formUrlEncodedContent.ReadAsStringAsync();
        var httpContent = new StringContent(urlEncodedString, Encoding.UTF8, "application/x-www-form-urlencoded");

        var authorizeRequest = new HttpRequestMessage
        {
            RequestUri = new Uri("https://tedigib2c.b2clogin.com/tedigib2c.onmicrosoft.com/B2C_1A_SIGNIN/oauth2/v2.0/token"),
            Method = HttpMethod.Post,
            Content = httpContent
        };

        // Authorize
        var response = await _client.SendAsync(authorizeRequest);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Bad response code: {code}", response.StatusCode.ToString());
            _logger.LogError("Bad content: {content}", await response.Content.ReadAsStringAsync());
            throw new HttpRequestException("TMT: Access Denied", null, HttpStatusCode.Unauthorized); // Throw 401 if not authorized.
        }

        // Parse response
        var responseBody = await response.Content.ReadAsStringAsync();
        var responseContent = JsonSerializer.Deserialize<TMTAuthorizationResponse>(responseBody);

        if (responseContent == null)
        {
            throw new HttpRequestException("TMT: Bad Response", null, HttpStatusCode.Unauthorized); // Throw 401 if not authorized.
        }

        // Return authorization details
        var details = new TMTAuthorizationDetails
        {
            AccessToken = responseContent.AccessToken,
            ExpiresOn = responseContent.ExpiresOn,
            ProxyAddress = secrets.ProxyAddress,
            ProxyUser = secrets.ProxyUser,
            ProxyPassword = secrets.ProxyPassword
        };

        return details;
    }
}

