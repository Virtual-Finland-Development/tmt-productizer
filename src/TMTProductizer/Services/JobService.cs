using System.Net.Http.Headers;
using CodeGen.Api.TMT.Model;
using TMTProductizer.Models;
using TMTProductizer.Services.TMT;
using TMTProductizer.Utils;

namespace TMTProductizer.Services;

public class JobService : IJobService
{
    private readonly IProxyHttpClientFactory _clientFactory;
    private readonly ITMTAuthorizationService _tmtAuthorizationService;
    private readonly ILogger<JobService> _logger;

    public JobService(IProxyHttpClientFactory clientFactory, ITMTAuthorizationService tmtAuthorizationService, ILogger<JobService> logger)
    {
        _clientFactory = clientFactory;
        _tmtAuthorizationService = tmtAuthorizationService;
        _logger = logger;
    }

    public async Task<(List<Job> jobs, long totalCount)> Find(JobsRequest query)
    {
        var pageNumber = GetPageNumberFromOffsetAndLimit(query.Paging.Offset, query.Paging.Limit);

        // Get TMT Authorization Details
        TMTAuthorizationDetails tmtAuthorizationDetails = await _tmtAuthorizationService.GetTMTAuthorizationDetails(); // Throws HttpRequestException;

        // Form the request
        var requestMessage = new HttpRequestMessage
        {
            RequestUri = new Uri($"{_clientFactory.BaseAddress}?sivu={pageNumber}&maara={query.Paging.Limit}"),
            Method = HttpMethod.Get,
        };
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tmtAuthorizationDetails.AccessToken);

        // Build a proxy client
        var httpClient = _clientFactory.GetTMTProxyClient(tmtAuthorizationDetails);

        // Send request
        var response = await httpClient.SendAsync(requestMessage);
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("TMT API responded with: {StatusCode}", response.StatusCode);
            _logger.LogError("Content: {Content}", await response.Content.ReadAsStringAsync());
            return (new List<Job>(), 0);
        }

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
            return (new List<Job>(), 0);
        }

        if (result == null) return (new List<Job>(), 0);

        var jobs = new List<Job>();
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

        return (jobs, result.IlmoituksienMaara);
    }

    private int GetPageNumberFromOffsetAndLimit(int pagingOffset, int pagingLimit)
    {
        var (quotient, _) = Math.DivRem(pagingOffset, pagingLimit);

        return quotient;
    }
}
