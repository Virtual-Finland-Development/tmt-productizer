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

    public async Task<IReadOnlyList<Job>> Find(JobsRequest query)
    {
        var jobs = new List<Job>();

        var pageNumber = GetPageNumberFromOffsetAndLimit(query.Paging.Offset, query.Paging.Limit);
        var response = await _client.GetAsync($"?sivu={pageNumber}&maara={query.Paging.Limit}");

        if (!response.IsSuccessStatusCode) return jobs;

        var result = await response.Content.ReadFromJsonAsync<Hakutulos>();
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
}
