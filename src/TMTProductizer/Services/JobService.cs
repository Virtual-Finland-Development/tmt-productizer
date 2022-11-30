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

        // Transform the results to a list of jobs
        var jobs = TransformTMTResultsToJobs(results);

        // Filter and paginate the results
        return FilterAndPaginateJobs(jobs, query);
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
