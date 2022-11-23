using System.Net.Http.Headers;
using CodeGen.Api.TMT.Model;
using Microsoft.AspNetCore.Http.Extensions;
using TMTProductizer.Models;
using TMTProductizer.Utils;

namespace TMTProductizer.Services;

public class JobService : IJobService
{
    private readonly IProxyHttpClientFactory _clientFactory;
    private readonly IAPIAuthorizationService _tmtApiAuthorizationService;
    private readonly ILogger<JobService> _logger;

    public JobService(IProxyHttpClientFactory clientFactory, IAPIAuthorizationService tmtApiAuthorizationService, ILogger<JobService> logger)
    {
        _clientFactory = clientFactory;
        _tmtApiAuthorizationService = tmtApiAuthorizationService;
        _logger = logger;
    }

    public async Task<IReadOnlyList<Job>> Find(JobsRequest query)
    {
        var jobs = new List<Job>();
        var queryParamsString = TransformJobRequestToQueryParams(query);

        // Get TMT Authorization Details
        APIAuthorizationPackage authorizationPackage = await _tmtApiAuthorizationService.GetAPIAuthorizationPackage(); // Throws HttpRequestException;

        // Form the request
        var requestMessage = new HttpRequestMessage
        {
            RequestUri = new Uri($"{_clientFactory.BaseAddress}?{queryParamsString}"),
            Method = HttpMethod.Get,
        };
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authorizationPackage.AccessToken);

        // Build a proxy client
        var httpClient = _clientFactory.GetProxyClient(authorizationPackage);

        // Send request
        var response = await httpClient.SendAsync(requestMessage);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("TMT API responded with: {StatusCode}", response.StatusCode);
            _logger.LogError("Content: {content}", await response.Content.ReadAsStringAsync());
            return jobs;
        };

        // Parse response
        Hakutulos? result;
        try
        {
            result = await response.Content.ReadFromJsonAsync<Hakutulos>();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to parse TMT API response");
            await File.WriteAllTextAsync("TMTResponseOutputDebug.txt", await response.Content.ReadAsStringAsync());
            _logger.LogError("Wrote a response output debug file to TMTResponseOutputDebug.txt");
            return jobs;
        }

        if (result == null) return jobs;

        jobs.AddRange(result.Ilmoitukset.Select(ilmoitus => new Job
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

        // TODO: TMT API doesn't support keyword search so we have to do proper in-memory filtering instead. This isn't it.
        if (query.Query != "")
        {
            jobs = jobs.FindAll(j =>
            {
                // The ! operator does not seem to work at runtime, like it says: https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/operators/null-forgiving
                // Use null-comparison instead
                return (j.BasicInfo.Description != null && j.BasicInfo.Description.Contains(query.Query)) || (j.BasicInfo.Title != null && j.BasicInfo.Title.Contains(query.Query));
            });
        }


        return jobs;
    }

    /// <summary>
    /// Transforms a JobsRequest to a query string that can be used in a TMT API request.
    /// </summary>
    private QueryString TransformJobRequestToQueryParams(JobsRequest query)
    {
        var pageNumber = GetPageNumberFromOffsetAndLimit(query.Paging.Offset, query.Paging.Limit);

        var parameters = new Dictionary<string, string> {
            { "sivu", pageNumber.ToString() },
            { "maara", query.Paging.Limit.ToString() }
        };

        var queryBuilder = new QueryBuilder(parameters);

        // note: maybe loopify
        if (query.Location.Countries != null && query.Location.Countries.Any())
        {
            queryBuilder.Add("maa", string.Join(",", query.Location.Countries));
        }
        if (query.Location.Regions != null && query.Location.Regions.Any())
        {
            queryBuilder.Add("maakunta", string.Join(",", query.Location.Regions));
        }
        if (query.Location.Municipalities != null && query.Location.Municipalities.Any())
        {
            queryBuilder.Add("kunta", string.Join(",", query.Location.Municipalities));
        }

        return queryBuilder.ToQueryString();
    }

    private int GetPageNumberFromOffsetAndLimit(int pagingOffset, int pagingLimit)
    {
        var (quotient, _) = Math.DivRem(pagingOffset, pagingLimit);

        return quotient;
    }
}
