using System.Net.Http.Headers;
using CodeGen.Api.TMT.Model;
using TMTProductizer.Models;
using TMTProductizer.Services.AWS;
using TMTProductizer.Utils;

namespace TMTProductizer.Services;

public class JobService : IJobService
{
    private readonly IProxyHttpClientFactory _clientFactory;
    private readonly IAPIAuthorizationService _tmtApiAuthorizationService;
    private readonly IS3BucketCache _tmtApiResultsCacheService;
    private readonly ILogger<JobService> _logger;
    private string _tmtCacheKey = "TMTJobResults";
    private int _tmtCacheTTL = 24 * 60 * 60; // 24 h

    public JobService(IProxyHttpClientFactory clientFactory, IAPIAuthorizationService tmtApiAuthorizationService, IS3BucketCache tmtApiResultsCacheService, ILogger<JobService> logger)
    {
        _clientFactory = clientFactory;
        _tmtApiAuthorizationService = tmtApiAuthorizationService;
        _tmtApiResultsCacheService = tmtApiResultsCacheService;
        _logger = logger;
    }

    public async Task<(List<Job> jobs, long totalCount)> Find(JobsRequest query)
    {
        // Fetch hakutulos results
        var results = await GetTMTAPIResultsFromCache();

        // Transform the results to a list of jobs
        var jobs = TransformTMTResultsToJobs(results);

        // Filter and paginate the results
        // @TODO: Filtering and pagination needs to be implemented
        var jobsTotalCount = jobs.Count;
        if (query.Query != "")
        {
            jobs = jobs.FindAll(j =>
            {
                // The ! operator does not seem to work at runtime, like it says: https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/null-forgiving
                // Use null-comparison instead
                return (j.BasicInfo.Description != null && j.BasicInfo.Description.Contains(query.Query)) || (j.BasicInfo.Title != null && j.BasicInfo.Title.Contains(query.Query));
            });
        }

        return (jobs, jobsTotalCount);
    }

    /// <summary>
    /// Fetches the results from the cache or from the TMT API if the cache is empty.
    /// </summary>
    private async Task<Hakutulos?> GetTMTAPIResultsFromCache()
    {
        var cachedResults = await _tmtApiResultsCacheService.GetCacheItem<Hakutulos>(_tmtCacheKey);
        if (cachedResults != null)
        {
            _logger.LogInformation("Found TMT API results from cache");
            return cachedResults;
        }

        _logger.LogInformation("Fetching TMT API results from TMT API");
        var results = await GetTMTResultsFromAPI();
        if (results != null)
        {
            _logger.LogInformation("Saving TMT API results to cache");
            await _tmtApiResultsCacheService.SaveCacheItem(_tmtCacheKey, results, _tmtCacheTTL);
        }

        return results;
    }

    /// <summary>
    /// Fetches the results from the TMT API
    /// </summary>
    private async Task<Hakutulos?> GetTMTResultsFromAPI()
    {
        // Get TMT Authorization Details
        APIAuthorizationPackage authorizationPackage = await _tmtApiAuthorizationService.GetAPIAuthorizationPackage(); // Throws HttpRequestException;

        // Build a proxy client
        var httpClient = _clientFactory.GetProxyClient(authorizationPackage);

        // @TODO: fetch all the pages with a loop, merge to a single Hakutulos object
        Hakutulos? results = await GetTMTAPIResultPage(httpClient, authorizationPackage, 100, 0);

        return results;
    }

    /// <summary>
    /// Fetches a single page of unfiltered results from the TMT API.
    /// </summary>
    private async Task<Hakutulos?> GetTMTAPIResultPage(HttpClient httpClient, APIAuthorizationPackage authorizationPackage, int limit, int offset)
    {
        var queryParamsString = "limit=" + limit + "&offset=" + offset;

        // Form the request
        var requestMessage = new HttpRequestMessage
        {
            RequestUri = new Uri($"{_clientFactory.BaseAddress}?{queryParamsString}"),
            Method = HttpMethod.Get,
        };
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authorizationPackage.AccessToken);

        // Send request
        var response = await httpClient.SendAsync(requestMessage);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("TMT API responded with: {StatusCode}", response.StatusCode);
            _logger.LogError("Content: {Content}", await response.Content.ReadAsStringAsync());
            return null;
        }

        try
        {
            return await response.Content.ReadFromJsonAsync<Hakutulos>();
        }
        catch (Exception e)
        {
            if (offset == 0)
            {
                _logger.LogError(e, "Failed to parse TMT API response");
                await File.WriteAllTextAsync("TMTResponseOutputDebug.txt", await response.Content.ReadAsStringAsync());
                _logger.LogError("Wrote a response output debug file to TMTResponseOutputDebug.txt");
            }
        }

        return null;
    }

    /// <summary>
    /// Transforms the TMT API results to a list of jobs.
    /// </summary>
    private List<Job> TransformTMTResultsToJobs(Hakutulos? results)
    {
        var jobs = new List<Job>();

        if (results == null) return jobs;

        jobs.AddRange(results.Ilmoitukset.Select(ilmoitus => new Job
        {
            Employer = ilmoitus.IlmoittajanNimi.FirstOrDefault(x => x.KieliKoodi == "fi")?.Arvo.ToString() ?? string.Empty,
            Location = new Location
            {
                Municipality = ilmoitus.Sijainti.Toimipaikka.Postitoimipaikka,
                Postcode = ilmoitus.Sijainti.Toimipaikka.Postinumero
            },
            BasicInfo = new BasicInfo
            {
                Title = ilmoitus.Perustiedot.TyonOtsikko.FirstOrDefault(x => x.KieliKoodi == "fi")?.Arvo.ToString() ?? string.Empty,
                Description =
                    ilmoitus.Perustiedot.TyonKuvaus.FirstOrDefault(x => x.KieliKoodi == "fi")?.Arvo.ToString() ?? string.Empty,
                WorkTimeType = ilmoitus.Perustiedot.TyoAika
            },
            PublishedAt = ilmoitus.Julkaisupvm,
            ApplicationUrl = ilmoitus.Hakeminen.HakemuksenUrl,
            ApplicationEndDate = ilmoitus.Hakeminen.HakuaikaPaattyy
        }));

        return jobs;
    }
}
