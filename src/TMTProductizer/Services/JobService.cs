using CodeGen.Api.Testbed.Model;
using CodeGen.Api.TMT.Model;
using TMTProductizer.Models;

namespace TMTProductizer.Services;

public class JobService : IJobService
{
    private readonly HttpClient _client;

    public JobService(HttpClient client)
    {
        _client = client;
    }

    public async Task<IReadOnlyList<JobPosting>> Find(JobsRequest query)
    {
        var jobs = new List<JobPosting>();

        var pageNumber = GetPageNumberFromOffsetAndLimit(query.Paging.Offset, query.Paging.Limit);
        var response = await _client.GetAsync($"?sivu={pageNumber}&maara={query.Paging.Limit}");

        if (!response.IsSuccessStatusCode) return jobs;

        var result = await response.Content.ReadFromJsonAsync<Hakutulos>();
        if (result == null) return jobs;

        jobs.AddRange(result.Ilmoitukset.Select(ilmoitus => new JobPosting
        {
            Employer = ilmoitus.IlmoittajanNimi
                .FirstOrDefault(e => e.KieliKoodi == "fi")?.Arvo
                .ToString() ?? string.Empty,
            Location = new JobPostingLocation
            {
                Municipality = "",
                Postcode = ""
            },
            BasicInfo = new JobPostingBasicInfo
            {
                Title = ilmoitus.Perustiedot.TyonOtsikko
                    .FirstOrDefault(x => x.KieliKoodi == "fi")?.Arvo
                    .ToString() ?? string.Empty,
                Description = ilmoitus.Perustiedot.TyonKuvaus
                    .FirstOrDefault(x => x.KieliKoodi == "fi")?.Arvo
                    .ToString() ?? string.Empty,
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
}
