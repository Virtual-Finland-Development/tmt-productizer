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
        return FilterAndPaginateJobs(jobs, query);
    }

    /// <summary>
    /// Fetches the results from the cache or from the TMT API if the cache is empty.
    /// </summary>
    private async Task<Hakutulos> GetTMTAPIResultsFromCache()
    {
        var cachedResults = await _tmtApiResultsCacheService.GetCacheItem<Hakutulos>(_tmtCacheKey);
        if (cachedResults != null)
        {
            _logger.LogInformation("Found TMT API results from cache");
            return cachedResults;
        }

        _logger.LogInformation("Fetching TMT API results from TMT API");
        var results = await GetTMTResultsFromAPI();
        if (results.IlmoituksienMaara > 0)
        {
            _logger.LogInformation("Saving TMT API results to cache");
            await _tmtApiResultsCacheService.SaveCacheItem(_tmtCacheKey, results, _tmtCacheTTL);
        }

        return results;
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
        catch (Exception e)
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

    /// <summary>
    /// Transforms the TMT API results to a list of jobs.
    /// </summary>
    private List<Job> TransformTMTResultsToJobs(Hakutulos results)
    {
        var jobs = new List<Job>();

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

    /// <summary>
    /// Filters and paginates the jobs.
    /// </summary>
    private (List<Job>, int) FilterAndPaginateJobs(List<Job> jobs, JobsRequest query)
    {
        int totalCount = jobs.Count;

        // Filter by search phase
        if (query.Query != "")
        {
            jobs = jobs.FindAll(j =>
            {
                // The ! operator does not seem to work at runtime, like it says: https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/null-forgiving
                // Use null-comparison instead
                return (j.BasicInfo.Description != null && j.BasicInfo.Description.Contains(query.Query)) || (j.BasicInfo.Title != null && j.BasicInfo.Title.Contains(query.Query));
            });
        }

        // Filter by location
        if (query.Location.Municipalities.Any())
        {
            jobs = jobs.FindAll(j => query.Location.Municipalities.Contains(j.Location.Municipality));
        }
        if (query.Location.Regions.Any())
        {
            jobs = jobs.FindAll(j => query.Location.Regions.Contains(j.Location.Municipality)); // @TODO: implement region filtering, maybe pre-filter the results from the API
        }
        if (query.Location.Countries.Any())
        {
            jobs = jobs.FindAll(j => query.Location.Countries.Contains(j.Location.Municipality));
        }

        // Paginate the jobs
        if (query.Paging.Offset != 0)
        {
            jobs = jobs.Skip(query.Paging.Offset).ToList();
        }
        if (query.Paging.Limit != 0)
        {
            jobs = jobs.Take(query.Paging.Limit).ToList();
        }

        return (jobs, totalCount);
    }
}
