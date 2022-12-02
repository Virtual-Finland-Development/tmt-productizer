using CodeGen.Api.TMT.Model;
using TMTProductizer.Models;

namespace TMTProductizer.Services;

public class JobService : IJobService
{
    private readonly ITMTJobsFetcher _jobsFetcher;
    private readonly ILogger<JobService> _logger;

    public JobService(ITMTJobsFetcher jobsFetcher, ILogger<JobService> logger)
    {
        _jobsFetcher = jobsFetcher;
        _logger = logger;
    }

    public async Task<(List<Job> jobs, long totalCount)> Find(JobsRequest query)
    {
        // Fetch hakutulos results
        var results = await _jobsFetcher.FetchTMTAPIResults();

        // Filter and paginate the results
        var filteredResults = FilterAndPaginateResults(results, query);

        // Transform the results to a list of jobs
        var jobs = TransformTMTResultsToJobs(filteredResults);

        return (jobs, results.IlmoituksienMaara);
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
    /// Filters and paginates the results, by mutation
    /// </summary>
    private Hakutulos FilterAndPaginateResults(Hakutulos results, JobsRequest query)
    {
        // Filter by search phase
        if (query.Query != "")
        {
            results.Ilmoitukset = results.Ilmoitukset.FindAll(ilmoitus =>
            {
                var title = ilmoitus.Perustiedot.TyonOtsikko.FirstOrDefault(x => x.KieliKoodi == "fi")?.Arvo.ToString() ?? string.Empty;
                var description = ilmoitus.Perustiedot.TyonKuvaus.FirstOrDefault(x => x.KieliKoodi == "fi")?.Arvo.ToString() ?? string.Empty;
                return (description != null && description.Contains(query.Query)) || (title != null && title.Contains(query.Query));
            });
        }

        // Filter by location
        if (query.Location.Municipalities.Any())
        {
            results.Ilmoitukset = results.Ilmoitukset.FindAll(ilmoitus => query.Location.Municipalities.Intersect(ilmoitus.Sijainti.Kunta).Any());
        }
        if (query.Location.Regions.Any())
        {
            results.Ilmoitukset = results.Ilmoitukset.FindAll(ilmoitus => query.Location.Regions.Intersect(ilmoitus.Sijainti.Maakunta).Any());
        }
        if (query.Location.Countries.Any())
        {
            results.Ilmoitukset = results.Ilmoitukset.FindAll(ilmoitus => query.Location.Countries.Intersect(ilmoitus.Sijainti.Maa).Any());
        }

        // Paginate the jobs
        if (query.Paging.Offset != 0)
        {
            results.Ilmoitukset = results.Ilmoitukset.Skip(query.Paging.Offset).ToList();
        }
        if (query.Paging.Limit != 0)
        {
            results.Ilmoitukset = results.Ilmoitukset.Take(query.Paging.Limit).ToList();
        }

        return results;
    }
}
