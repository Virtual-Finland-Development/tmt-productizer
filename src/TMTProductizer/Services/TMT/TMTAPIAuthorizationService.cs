using System.Net;
using System.Text;
using System.Text.Json;
using TMTProductizer.Models;
using TMTProductizer.Services.AWS;
using TMTProductizer.Utils.DateUtils;

namespace TMTProductizer.Services.TMT;

public class TMTAPIAuthorizationService : ITMTAPIAuthorizationService
{
    private readonly HttpClient _client;
    private readonly IDynamoDBCache _dynamoDBCache;
    private readonly ITMTSecretsManager _secretsManager;
    private readonly ILogger<TMTAPIAuthorizationService> _logger;
    private TMTAPIAuthorizationDetails? _TMTAPIAuthorizationDetails = null;
    private bool _skipAuthorizationCeck;
    private const string _cacheKey = "TMTAPIAuthorizationDetails";

    public TMTAPIAuthorizationService(HttpClient client, IDynamoDBCache dynamoDBCache, ITMTSecretsManager secretsManager, ILogger<TMTAPIAuthorizationService> logger, IHostEnvironment env)
    {
        _client = client;
        _dynamoDBCache = dynamoDBCache;
        _secretsManager = secretsManager;
        _logger = logger;
        _skipAuthorizationCeck = env.IsEnvironment("Mock");

    }

    public async Task<TMTAPIAuthorizationDetails> GetTMTAPIAuthorizationDetails()
    {
        // Skip on local mock development
        if (_skipAuthorizationCeck) return new TMTAPIAuthorizationDetails();

        // If we have a valid token in the current lambda instance, return it
        _TMTAPIAuthorizationDetails = await GetTMTAPIAuthorizationDetailsFromCache();
        if (_TMTAPIAuthorizationDetails != null && DateUtils.UnixTimeStampToDateTime(_TMTAPIAuthorizationDetails.ExpiresOn) > DateTime.UtcNow)
        {
            return _TMTAPIAuthorizationDetails;
        }

        _logger.LogInformation("Fetching new TMTAPIAuthorizationDetails");
        _TMTAPIAuthorizationDetails = await FetchTMTAPIAuthorizationDetails();
        await SaveTMTAPIAuthorizationDetailsToCache(_TMTAPIAuthorizationDetails);

        return _TMTAPIAuthorizationDetails;
    }

    private async Task<TMTAPIAuthorizationDetails> FetchTMTAPIAuthorizationDetails()
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
        var responseContent = JsonSerializer.Deserialize<TMTAPIAuthorizationResponse>(responseBody);

        if (responseContent == null)
        {
            throw new HttpRequestException("TMT: Bad Response", null, HttpStatusCode.Unauthorized); // Throw 401 if not authorized.
        }

        // Return authorization details
        var details = new TMTAPIAuthorizationDetails
        {
            AccessToken = responseContent.AccessToken,
            ExpiresOn = responseContent.ExpiresOn,
            ProxyAddress = secrets.ProxyAddress,
            ProxyUser = secrets.ProxyUser,
            ProxyPassword = secrets.ProxyPassword
        };

        return details;
    }


    private async Task<TMTAPIAuthorizationDetails?> GetTMTAPIAuthorizationDetailsFromCache()
    {
        return await _dynamoDBCache.GetCacheItem<TMTAPIAuthorizationDetails>(_cacheKey);
    }

    private async Task SaveTMTAPIAuthorizationDetailsToCache(TMTAPIAuthorizationDetails tmtAuthorizationDetails)
    {
        await _dynamoDBCache.SaveCacheItem<TMTAPIAuthorizationDetails>(_cacheKey, tmtAuthorizationDetails);
    }
}

