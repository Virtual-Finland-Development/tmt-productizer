using CodeGen.Api.TMT.Model;
using Microsoft.Extensions.Options;
using TMTProductizer.Config;
using TMTProductizer.Models;

namespace TMTProductizer.Services;

internal class JobService : IJobService
{
    private readonly string _endpoint;

    public JobService(IOptions<TmtOptions> configuration)
    {
        _endpoint = configuration.Value.ApiEndpoint;
    }

    public async Task<IReadOnlyList<Job>> Find()
    {
        var jobs = new List<Job>();

        var client = new HttpClient();
        var response = await client.GetAsync(_endpoint + "?sivu=0&maara=10");

        if (!response.IsSuccessStatusCode) return jobs;


        var result = await response.Content.ReadFromJsonAsync<Hakutulos>();
        if (result == null) return jobs;

        jobs.AddRange(result.Ilmoitukset.Select(ilmoitus => new Job
        {
            Employer = ilmoitus.IlmoittajanNimi.FirstOrDefault(x => x.KieliKoodi == "fi")?.Arvo.ToString(),
            Location = new Location
            {
                City = ilmoitus.Sijainti.Toimipaikka.Postitoimipaikka.ToString()
            },
            BasicInfo = new BasicInfo
            {
                Title = ilmoitus.Perustiedot.TyonOtsikko.FirstOrDefault(x => x.KieliKoodi == "fi")?.Arvo.ToString(),
                Description = ilmoitus.Perustiedot.TyonKuvaus.FirstOrDefault(x => x.KieliKoodi == "fi")?.Arvo.ToString()
            },
            PublishedAt = ilmoitus.Julkaisupvm
        }));

        return jobs;
    }
}