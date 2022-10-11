using System.Net;
using FluentAssertions;
using Moq;
using Moq.Protected;
using TMTProductizer.Models;
using TMTProductizer.Services;

namespace TMTProductizer.UnitTests.Services;

public class JobServiceTests
{
    private const string TmtJson =
        "{\n\t\"ilmoituksienMaara\": 1, \n\t\"Ilmoitukset\": [\n\t\t{\n\t\t\t\"ilmoituksenOhjaus\": true, \n\t\t\t\"ilmoituksenID\": \"f5eb5e7f-8e27-4e6f-96fd-5317f74518ed\", \n\t\t\t\"markkinointikuvaus\": \"string\", \n\t\t\t\"tyollistaja\": \"string\", \n\t\t\t\"osaamisvaatimukset\": {\n\t\t\t\t\"kielitaidot\": [\n\t\t\t\t\t{\n\t\t\t\t\t\t\"kielitaidonLisatieto\": [\n\t\t\t\t\t\t\t{\n\t\t\t\t\t\t\t\t\"kieliKoodi\": \"fi\", \n\t\t\t\t\t\t\t\t\"arvo\": \"string\"\n\t\t\t\t\t\t\t}\n\t\t\t\t\t\t], \n\t\t\t\t\t\t\"kielitaidonTaso\": \"L1\", \n\t\t\t\t\t\t\"kielitaito\": \"http://data.europa.eu/esco/skill/0e3493f4-0cad-4fbc-8e02-025f169b8114\"\n\t\t\t\t\t}\n\t\t\t\t], \n\t\t\t\t\"kortitJaLuvat\": {\n\t\t\t\t\t\"lupaKoodit\": [\n\t\t\t\t\t\t\"string\"\n\t\t\t\t\t], \n\t\t\t\t\t\"kortitJaLuvatLisatieto\": [\n\t\t\t\t\t\t{\n\t\t\t\t\t\t\t\"kieliKoodi\": \"fi\", \n\t\t\t\t\t\t\t\"arvo\": \"string\"\n\t\t\t\t\t\t}\n\t\t\t\t\t]\n\t\t\t\t}, \n\t\t\t\t\"rikosrekisteriote\": true, \n\t\t\t\t\"ajokortti\": {\n\t\t\t\t\t\"ajokortinLisatieto\": [\n\t\t\t\t\t\t{\n\t\t\t\t\t\t\t\"kieliKoodi\": \"fi\", \n\t\t\t\t\t\t\t\"arvo\": \"string\"\n\t\t\t\t\t\t}\n\t\t\t\t\t], \n\t\t\t\t\t\"vaaditutAjokorttiluokat\": [\n\t\t\t\t\t\t\"string\"\n\t\t\t\t\t]\n\t\t\t\t}, \n\t\t\t\t\"ammatit\": [\n\t\t\t\t\t{\n\t\t\t\t\t\t\"luokiteltuArvo\": \"http://data.europa.eu/esco/occupation/8d3e8aaa-791b-4c75-a465-f3f827028f50\", \n\t\t\t\t\t\t\"luokittelunNimi\": \"ESCO\"\n\t\t\t\t\t}\n\t\t\t\t], \n\t\t\t\t\"osaamiset\": [\n\t\t\t\t\t{\n\t\t\t\t\t\t\"luokiteltuArvo\": \"http://data.europa.eu/esco/occupation/8d3e8aaa-791b-4c75-a465-f3f827028f50\", \n\t\t\t\t\t\t\"luokittelunNimi\": \"ESCO\"\n\t\t\t\t\t}\n\t\t\t\t], \n\t\t\t\t\"koulutusaste\": \"31\"\n\t\t\t}, \n\t\t\t\"luontipvm\": \"2022-05-02T12:06:55.818Z\", \n\t\t\t\"ilmoittajanNimi\": [\n\t\t\t\t{\n\t\t\t\t\t\"kieliKoodi\": \"fi\", \n\t\t\t\t\t\"arvo\": \"FapplicationOyj\"\n\t\t\t\t}\n\t\t\t], \n\t\t\t\"hakeminen\": {\n\t\t\t\t\"hakuaikaPaattyy\": \"2022-05-10T12:00:00Z\", \n\t\t\t\t\"ilmoittajanYhteystiedot\": {\n\t\t\t\t\t\"puhelinNro\": \"+358123456789\", \n\t\t\t\t\t\"sposti\": \"some.one@somewhere.com\"\n\t\t\t\t}, \n\t\t\t\t\"hakuohjeet\": [\n\t\t\t\t\t{\n\t\t\t\t\t\t\"kieliKoodi\": \"fi\", \n\t\t\t\t\t\t\"arvo\": \"string\"\n\t\t\t\t\t}\n\t\t\t\t], \n\t\t\t\t\"hakemuksenUrl\": \"www.google.com\"\n\t\t\t}, \n\t\t\t\"kotitaloudenNimi\": \"string\", \n\t\t\t\"ilmoituksenKielet\": [\n\t\t\t\t\"fi\"\n\t\t\t], \n\t\t\t\"perustiedot\": {\n\t\t\t\t\"tyoAika\": \"02\", \n\t\t\t\t\"tyoTunnitMinimi\": 0, \n\t\t\t\t\"tyonOtsikko\": [\n\t\t\t\t\t{\n\t\t\t\t\t\t\"kieliKoodi\": \"fi\", \n\t\t\t\t\t\t\"arvo\": \"Etsit\\u00e4\\u00e4nkapteenia\"\n\t\t\t\t\t}\n\t\t\t\t], \n\t\t\t\t\"tyonTiivistelma\": [\n\t\t\t\t\t{\n\t\t\t\t\t\t\"kieliKoodi\": \"fi\", \n\t\t\t\t\t\t\"arvo\": \"string\"\n\t\t\t\t\t}\n\t\t\t\t], \n\t\t\t\t\"tyoTunnitMaksimi\": 0, \n\t\t\t\t\"palkanLisatieto\": [\n\t\t\t\t\t{\n\t\t\t\t\t\t\"kieliKoodi\": \"fi\", \n\t\t\t\t\t\t\"arvo\": \"string\"\n\t\t\t\t\t}\n\t\t\t\t], \n\t\t\t\t\"palkanPeruste\": \"01\", \n\t\t\t\t\"tyoAlkaaLisatieto\": [\n\t\t\t\t\t{\n\t\t\t\t\t\t\"kieliKoodi\": \"fi\", \n\t\t\t\t\t\t\"arvo\": \"string\"\n\t\t\t\t\t}\n\t\t\t\t], \n\t\t\t\t\"maaraaikaisuudenPaattymisPvm\": \"2022-12-31\", \n\t\t\t\t\"maaraaikaisuudenSyy\": [\n\t\t\t\t\t{\n\t\t\t\t\t\t\"kieliKoodi\": \"fi\", \n\t\t\t\t\t\t\"arvo\": \"string\"\n\t\t\t\t\t}\n\t\t\t\t], \n\t\t\t\t\"kutsutaanTarvittaessa\": true, \n\t\t\t\t\"tyoAlkaaPvm\": \"2022-06-01\", \n\t\t\t\t\"tyonKuvaus\": [\n\t\t\t\t\t{\n\t\t\t\t\t\t\"kieliKoodi\": \"fi\", \n\t\t\t\t\t\t\"arvo\": \"string\"\n\t\t\t\t\t}\n\t\t\t\t], \n\t\t\t\t\"tyoAlkaa\": \"02\", \n\t\t\t\t\"tyoTunnitAjanjakso\": \"0201\", \n\t\t\t\t\"kuuluuMatkustamista\": true, \n\t\t\t\t\"paikkojenMaara\": 1, \n\t\t\t\t\"tyonJatkuvuus\": \"01\", \n\t\t\t\t\"tyoskentely\": {\n\t\t\t\t\t\"vuorotyo\": [\n\t\t\t\t\t\t\"0801,0802\"\n\t\t\t\t\t], \n\t\t\t\t\t\"tyoskentelyAika\": [\n\t\t\t\t\t\t\"01,08\"\n\t\t\t\t\t]\n\t\t\t\t}\n\t\t\t}, \n\t\t\t\"kotisivut\": \"string\", \n\t\t\t\"ilmoittajanYTunnus\": \"string\", \n\t\t\t\"muokattupvm\": \"2022-05-02T12:06:55.818Z\", \n\t\t\t\"sijainti\": {\n\t\t\t\t\"toimipaikka\": {\n\t\t\t\t\t\"postinumero\": \"string\", \n\t\t\t\t\t\"postitoimipaikka\": \"string\", \n\t\t\t\t\t\"toimipaikanNimi\": [\n\t\t\t\t\t\t{\n\t\t\t\t\t\t\t\"kieliKoodi\": \"fi\", \n\t\t\t\t\t\t\t\"arvo\": \"string\"\n\t\t\t\t\t\t}\n\t\t\t\t\t], \n\t\t\t\t\t\"postiosoite\": \"string\"\n\t\t\t\t}, \n\t\t\t\t\"maa\": [\n\t\t\t\t\t\"FI,SE\"\n\t\t\t\t], \n\t\t\t\t\"kunta\": [\n\t\t\t\t\t\"062,179,698,309\"\n\t\t\t\t], \n\t\t\t\t\"maakunta\": [\n\t\t\t\t\t\"06,01,02,19\"\n\t\t\t\t], \n\t\t\t\t\"sijaintiJoustava\": true\n\t\t\t}, \n\t\t\t\"julkaisupvm\": \"2022-05-02T12:06:55.818Z\"\n\t\t}\n\t], \n\t\"sivutus\": {\n\t\t\"sivu\": 0, \n\t\t\"maara\": 0\n\t}\n}";

    [Test]
    public void TryingToFindJob_WithTmtApiUp_ReturnsListWithData()
    {
        var handler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        handler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(TmtJson)
            });
        var httpClient = new HttpClient(handler.Object) { BaseAddress = new Uri("http://localhost/") };
        var query = new JobsRequest
        {
            Query = "",
            Location = new LocationQuery
            {
                Countries = new List<string>(),
                Regions = new List<string>(),
                Municipalities = new List<string>()
            },
            Paging = new PagingOptions
            {
                Limit = 1,
                Offset = 20
            }
        };
        var sut = new JobService(httpClient);

        var result = sut.Find(query);

        result.Should().NotBeNull();
        result.Result.Should().BeOfType<List<Job>>();
        result.Result.Count.Should().BeGreaterThan(0);
    }

    [Test]
    public void TryingToFindJob_WithTmtApiDown_ReturnsEmptyList()
    {
        var handler = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        handler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.InternalServerError,
                Content = new StringContent("errorMessage")
            });
        var httpClient = new HttpClient(handler.Object) { BaseAddress = new Uri("http://localhost/") };
        var query = new JobsRequest
        {
            Query = "",
            Location = new LocationQuery
            {
                Countries = new List<string>(),
                Regions = new List<string>(),
                Municipalities = new List<string>()
            },
            Paging = new PagingOptions
            {
                Limit = 1,
                Offset = 20
            }
        };
        var sut = new JobService(httpClient);

        var result = sut.Find(query);

        result.Should().NotBeNull();
        result.Result.Should().BeOfType<List<Job>>();
        result.Result.Count.Should().Be(0);
    }
}
