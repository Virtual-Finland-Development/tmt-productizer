using System.Net.Http.Headers;
using CodeGen.Api.TMT.Model;
using TMTProductizer.Exceptions;
using TMTProductizer.Models;
using TMTProductizer.Models.Cache.TMT;
using TMTProductizer.Services.AWS;
using TMTProductizer.Utils;

namespace TMTProductizer.Services;

public class TMTJobsFetcher : ITMTJobsFetcher
{

    private readonly IProxyHttpClientFactory _clientFactory;
    private readonly IAPIAuthorizationService _tmtApiAuthorizationService;
    private readonly IS3BucketCache _tmtApiResultsCacheService;
    private readonly ILogger<TMTJobsFetcher> _logger;
    private readonly string _tmtCacheKey = "TMTJobResults";
    private readonly int _tmtCacheTTL = 24 * 60 * 60; // 24 h

    public TMTJobsFetcher(IProxyHttpClientFactory clientFactory, IAPIAuthorizationService tmtApiAuthorizationService, IS3BucketCache tmtApiResultsCacheService, ILogger<TMTJobsFetcher> logger)
    {
        _clientFactory = clientFactory;
        _tmtApiAuthorizationService = tmtApiAuthorizationService;
        _tmtApiResultsCacheService = tmtApiResultsCacheService;
        _logger = logger;
    }

    /// <summary>
    /// Fetches the results from the cache or from the TMT API if the cache is empty.
    /// </summary>
    public async Task<CachedHakutulos> FetchTMTAPIResults()
    {
        var cachedResults = await _tmtApiResultsCacheService.GetCacheItem<CachedHakutulos>(_tmtCacheKey);
        if (cachedResults != null)
        {
            _logger.LogInformation("Found TMT API results from cache");
            return cachedResults;
        }

        _logger.LogError("TMT API results not found from cache, respond with an empty list"); // Cache can't be rebuilt from an API call
        return new CachedHakutulos(new Hakutulos(new List<Tyopaikkailmoitus>(), 0));
    }

    /// <summary>
    /// Updates the cache with the latest results from the TMT API.
    /// </summary>
    public async Task UpdateTMTAPICache()
    {
        _logger.LogInformation("Fetching TMT API results from TMT API");
        var results = await GetTMTResultsFromAPI();
        if (results.IlmoituksienMaara > 0)
        {
            _logger.LogInformation("Saving results to cache");
            // Transform the Hakutulos to CachedHakutulos
            var cachedResults = new CachedHakutulos(results);
            // Save the results to cache
            await _tmtApiResultsCacheService.SaveCacheItem(_tmtCacheKey, cachedResults, _tmtCacheTTL);
            _logger.LogInformation("Saving complete");
        }
        else
        {
            _logger.LogWarning("No results found from TMT API");
        }
    }

    /// <summary>
    /// Fetches the results from the TMT API
    /// </summary>
    private async Task<Hakutulos> GetTMTResultsFromAPI()
    {
        // Get TMT Authorization Details
        APIAuthorizationPackage authorizationPackage = await _tmtApiAuthorizationService.GetAPIAuthorizationPackage(); // Throws HttpRequestException;

        // Build a proxy client
        var httpClient = _clientFactory.GetProxyClient(authorizationPackage);

        // Fetch all the pages with a loop, merge to a single Hakutulos object
        var results = new Hakutulos(new List<Tyopaikkailmoitus>(), 0);
        var pagingOffset = 0;
        var pagingLimit = 500;
        Hakutulos? pageResults = null;

        do
        {
            pageResults = await GetTMTAPIResultPage(httpClient, authorizationPackage, pagingOffset, pagingLimit);
            if (pageResults != null)
            {
                results.Ilmoitukset.AddRange(pageResults.Ilmoitukset);
                pagingOffset = pagingOffset + pagingLimit;
            }

        } while (pageResults != null && pageResults.IlmoituksienMaara == pagingLimit); // have results and not be on the last page

        results.IlmoituksienMaara = results.Ilmoitukset.Count;
        return results;
    }

    /// <summary>
    /// Fetches a single page of unfiltered results from the TMT API.
    /// </summary>
    private async Task<Hakutulos?> GetTMTAPIResultPage(HttpClient httpClient, APIAuthorizationPackage authorizationPackage, int pagingOffset, int pagingLimit)
    {
        var pageNumber = GetPageNumberFromOffsetAndLimit(pagingOffset, pagingLimit);
        var queryParamsString = "maara=" + pagingLimit + "&sivu=" + pageNumber;

        // Form the request
        var requestMessage = new HttpRequestMessage
        {
            RequestUri = new Uri($"{_clientFactory.BaseAddress}?{queryParamsString}"),
            Method = HttpMethod.Get,
        };
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authorizationPackage.AccessToken);

        // Send request
        _logger.LogInformation("Fetching TMT API results from TMT API, page: {pageNumber}, offset: {pagingLimit}, limit: {pagingLimit}", pageNumber, pagingOffset, pagingLimit);
        var response = await httpClient.SendAsync(requestMessage);
        var responseAsString = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("TMT API responded with: {StatusCode}", response.StatusCode);
            _logger.LogError("Content: {Content}", responseAsString);
            return null;
        }

        try
        {
            return StringUtils.JsonDeserializeObject<Hakutulos>(responseAsString);
        }
        catch (JSONParseException e)
        {
            if (pageNumber == 0)
            {
                _logger.LogError(e, "Failed to parse TMT API response");
                await File.WriteAllTextAsync("TMTResponseOutputDebug.txt", responseAsString);
                _logger.LogError("Wrote a response output debug file to TMTResponseOutputDebug.txt");
            }
        }

        return null;
    }

    private int GetPageNumberFromOffsetAndLimit(int pagingOffset, int pagingLimit)
    {
        var (quotient, _) = Math.DivRem(pagingOffset, pagingLimit);

        return quotient;
    }
}