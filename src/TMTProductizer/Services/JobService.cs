using System.Net;
using System.Net.Http.Headers;
using CodeGen.Api.TMT.Model;
using TMTProductizer.Models;
using TMTProductizer.Services.TMT;

namespace TMTProductizer.Services;

public class JobService : IJobService
{   
    private readonly HttpClient _client;
    private readonly ITMT_AuthorizationService _tmtAuthorizationService;
    private readonly ILogger<JobService> _logger;

    public JobService(HttpClient client, ITMT_AuthorizationService tmtAuthorizationService, ILogger<JobService> logger)
    {
        _client = client;
        _tmtAuthorizationService = tmtAuthorizationService;
        _logger = logger;
    }

    public async Task<IReadOnlyList<Job>> Find(JobsRequest query)
    {
        var jobs = new List<Job>();
        var pageNumber = GetPageNumberFromOffsetAndLimit(query.Paging.Offset, query.Paging.Limit);

        // Get TMT Authorization Details
        TMTAuthorizationDetails tmtAuthorizationDetails  = await _tmtAuthorizationService.GetTMTAuthorizationDetails(); // Throws HttpRequestException;

        // Form the request
        var requestMessage = new HttpRequestMessage {
            RequestUri = new Uri($"{this._client.BaseAddress}?sivu={pageNumber}&maara={query.Paging.Limit}"),
            Method = HttpMethod.Get,
        };
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", tmtAuthorizationDetails.AccessToken);

        // Build a proxy client
        var httpClient = this.getTMTProxyClient(tmtAuthorizationDetails);

        // Send request
        var response = await httpClient.SendAsync(requestMessage);
        if (!response.IsSuccessStatusCode) {
            _logger.LogError("TMT API responded with: {StatusCode}", response.StatusCode);
            _logger.LogError("Content: {content}", await response.Content.ReadAsStringAsync());
            return jobs;
        };

        // Parse response
        Hakutulos? result;
        try {
            result = await response.Content.ReadFromJsonAsync<Hakutulos>();
        } catch (Exception e) {
            _logger.LogError(e, "Failed to parse TMT API response");
            return jobs;
        }

        if (result == null) return jobs;

        jobs.AddRange(result.Ilmoitukset.Select(ilmoitus => new Job
        {
            Employer = ilmoitus.IlmoittajanNimi.FirstOrDefault(x => x.KieliKoodi == "fi")?.Arvo.ToString(),
            Location = new Location
            {
                City = ilmoitus.Sijainti.Toimipaikka.Postitoimipaikka,
                Postcode = ilmoitus.Sijainti.Toimipaikka.Postinumero
            },
            BasicInfo = new BasicInfo
            {
                Title = ilmoitus.Perustiedot.TyonOtsikko.FirstOrDefault(x => x.KieliKoodi == "fi")?.Arvo.ToString(),
                Description =
                    ilmoitus.Perustiedot.TyonKuvaus.FirstOrDefault(x => x.KieliKoodi == "fi")?.Arvo.ToString(),
                WorkTimeType = ilmoitus.Perustiedot.TyoAika
            },
            PublishedAt = ilmoitus.Julkaisupvm,
            ApplicationUrl = ilmoitus.Hakeminen.HakemuksenUrl,
            ApplicationEndDate = ilmoitus.Hakeminen.HakuaikaPaattyy
        }));

        // TODO: TMT API doesn't support keyword search so we have to do proper in-memory filtering instead. This isn't it.
        if (query.Query != "")
            jobs = jobs.FindAll(j =>
                j.BasicInfo.Description!.Contains(query.Query) ||
                j.BasicInfo.Title!.Contains(query.Query));

        return jobs;
    }

    private int GetPageNumberFromOffsetAndLimit(int pagingOffset, int pagingLimit)
    {
        var (quotient, _) = Math.DivRem(pagingOffset, pagingLimit);

        return quotient;
    }

    private HttpClient getTMTProxyClient(TMTAuthorizationDetails tmtAuthorizationDetails)
    {   
        if (tmtAuthorizationDetails.ProxyAddress == null) {
            return this._client; // Pass for test cases
        }

        var proxy = new WebProxy {
            Address = new Uri(tmtAuthorizationDetails.ProxyAddress),
            BypassProxyOnLocal = false,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(userName: tmtAuthorizationDetails.ProxyUser, password: tmtAuthorizationDetails.ProxyPassword)
        };
        var httpClientHandler = new HttpClientHandler {
            Proxy = proxy,
            UseProxy = true
        };
        var httpClient = new HttpClient(httpClientHandler);

        return httpClient;
    }
}
