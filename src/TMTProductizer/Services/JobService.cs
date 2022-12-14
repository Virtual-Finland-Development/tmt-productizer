using TMTProductizer.Models;
using TMTProductizer.Models.Cache.TMT;

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
        string requestedKielikoodi = "fi"; // @TODO: Make this configurable

        // Fetch hakutulos results
        var results = await _jobsFetcher.FetchTMTAPIResults();

        // Filter and paginate the results
        var filteredResults = FilterAndPaginateResults(results, query, requestedKielikoodi);

        // Transform the results to a list of jobs
        var jobs = TransformTMTResultsToJobs(filteredResults, requestedKielikoodi);

        return (jobs, results.IlmoituksienMaara);
    }


    /// <summary>
    /// Transforms the TMT API results to a list of jobs.
    /// </summary>
    private List<Job> TransformTMTResultsToJobs(CachedHakutulos results, string requestedKielikoodi)
    {

        // Creates a tmt link for the job based on the työmarkkinatori url template
        string GenerateApplicationUrl(CachedTyopaikkailmoitus ilmoitus, string kielikoodi)
        {
            return $"https://tyomarkkinatori.fi/henkiloasiakkaat/avoimet-tyopaikat/{ilmoitus.IlmoituksenID}/{kielikoodi}";
        }

        var jobs = new List<Job>();

        jobs.AddRange(results.Ilmoitukset.Select(ilmoitus => new Job
        {
            Employer = ilmoitus.IlmoittajanNimi.FirstOrDefault(x => x.KieliKoodi == requestedKielikoodi)?.Arvo.ToString() ?? string.Empty,
            Location = new Location
            {
                Municipality = ilmoitus.Sijainti.Toimipaikka.Postitoimipaikka,
                Postcode = ilmoitus.Sijainti.Toimipaikka.Postinumero
            },
            BasicInfo = new BasicInfo
            {
                Title = ilmoitus.Perustiedot.TyonOtsikko.FirstOrDefault(x => x.KieliKoodi == requestedKielikoodi)?.Arvo.ToString() ?? string.Empty,
                Description =
                    ilmoitus.Perustiedot.TyonKuvaus.FirstOrDefault(x => x.KieliKoodi == requestedKielikoodi)?.Arvo.ToString() ?? string.Empty,
                WorkTimeType = ilmoitus.Perustiedot.TyoAika
            },
            PublishedAt = ilmoitus.Julkaisupvm,
            ApplicationUrl = GenerateApplicationUrl(ilmoitus, requestedKielikoodi),
            ApplicationEndDate = ilmoitus.Hakeminen.HakuaikaPaattyy
        }));

        return jobs;
    }

    /// <summary>
    /// Filters and paginates the results, by mutation
    /// </summary>
    private CachedHakutulos FilterAndPaginateResults(CachedHakutulos results, JobsRequest query, string requestedKielikoodi)
    {
        // Filter by search phase
        if (query.Query != "")
        {
            var searchPhrases = query.Query.Split(',').ToList();

            results.Ilmoitukset = results.Ilmoitukset.FindAll(ilmoitus =>
            {
                var title = ilmoitus.Perustiedot.TyonOtsikko.FirstOrDefault(x => x.KieliKoodi == requestedKielikoodi)?.Arvo.ToString() ?? string.Empty;
                var description = ilmoitus.Perustiedot.TyonKuvaus.FirstOrDefault(x => x.KieliKoodi == requestedKielikoodi)?.Arvo.ToString() ?? string.Empty;

                var isMatch = false;
                foreach (var searchPhrase in searchPhrases)
                {
                    if ((description != null && description.Contains(searchPhrase)) || (title != null && title.Contains(searchPhrase)))
                    {
                        isMatch = true;
                        break;
                    }
                }
                return isMatch;
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

        // By requirements
        if (query.Requirements != null && query.Requirements.Occupations.Any())
        {
            results.Ilmoitukset = results.Ilmoitukset.FindAll(ilmoitus =>
            {
                var ammattikoodit = ilmoitus.Osaamisvaatimukset.Ammatit.Select(ammatti => ammatti._LuokiteltuArvo).ToArray();
                return query.Requirements.Occupations.Intersect(ammattikoodit).Any();
            });
        }
        if (query.Requirements != null && query.Requirements.Skills.Any())
        {
            results.Ilmoitukset = results.Ilmoitukset.FindAll(ilmoitus =>
            {
                var ammattikoodit = ilmoitus.Osaamisvaatimukset.Osaamiset.Select(osaaminen => osaaminen._LuokiteltuArvo).ToArray();
                return query.Requirements.Skills.Intersect(ammattikoodit).Any();
            });
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
