using CodeGen.Api.TMT.Model;
using TMTProductizer.Models;

namespace TMTProductizer.Services;

internal class JobService : IJobService
{
    private const string JobsEndpoint = "http://localhost:4010/api/v1/tyopaikat?sivu=0&maara=100";

    public async Task<IReadOnlyList<Job>> Find()
    {
        var jobs = new List<Job>();

        var client = new HttpClient();
        var response = await client.GetAsync(JobsEndpoint);

        if (!response.IsSuccessStatusCode) return jobs;


        var result = await response.Content.ReadFromJsonAsync<Hakutulos>();
        if (result == null) return jobs;

        jobs.AddRange(result.Ilmoitukset.Select(ilmoitus => new Job
        {
            ApplicationUrl = ilmoitus.Hakeminen.HakemuksenUrl,
            EmployerName = ilmoitus.IlmoittajanNimi.FirstOrDefault(x => x.KieliKoodi == "fi")?.Arvo.ToString(),
            JobTitle = ilmoitus.Perustiedot.TyonOtsikko.FirstOrDefault(x => x.KieliKoodi == "fi")?.Arvo.ToString()
        }));

        return jobs;
    }
}