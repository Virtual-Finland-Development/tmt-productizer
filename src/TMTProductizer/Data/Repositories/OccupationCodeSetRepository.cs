using System.IO.Compression;
using System.Text.Json;
using TMTProductizer.Models.Repositories;

namespace TMTProductizer.Data.Repositories;

public class OccupationCodeSetRepository : IOccupationCodeSetRepository
{
    private readonly ILogger<IOccupationCodeSetRepository> _logger;
    private readonly HttpClient _httpClient;
    private readonly Uri _occupationsUrl;
    private List<OccupationCodeSet.Occupation>? _occupations = null;

    public OccupationCodeSetRepository(HttpClient httpClient, IConfiguration configuration, ILogger<IOccupationCodeSetRepository> logger)
    {
        _logger = logger;
        _httpClient = httpClient;
        _occupationsUrl = new Uri(configuration.GetSection("OccupationsCodeSetUrl").Value);
    }

    public async Task<List<OccupationCodeSet.Occupation>> GetAllOccupations()
    {
        // Early return if occupations have already been fetched
        if (_occupations != null)
        {
            return _occupations;
        }

        var zipContents = await UnzipUrl(_occupationsUrl.ToString());

        if (!string.IsNullOrEmpty(zipContents))
        {
            var rootOccupationData = JsonSerializer.Deserialize<List<OccupationCodeSet.Occupation>>(zipContents);

            if (rootOccupationData is not null)
            {
                _occupations = rootOccupationData;
                return rootOccupationData;
            }
        }
        return new List<OccupationCodeSet.Occupation>();
    }

    /// <summary>
    /// Downloads a zip file from the given url and returns the content of the first file in the zip archive.
    /// </summary>
    async Task<string> UnzipUrl(string zipUrl)
    {
        var httpResponseMessage = await _httpClient.GetAsync(zipUrl);

        if (httpResponseMessage.IsSuccessStatusCode)
        {
            var stream = await httpResponseMessage.Content.ReadAsStreamAsync();
            using var zipArchive = new ZipArchive(stream, ZipArchiveMode.Read);
            var entry = zipArchive.Entries.FirstOrDefault();

            if (entry is not null)
            {
                using var entryStream = entry.Open();
                using var reader = new StreamReader(entryStream);
                return await reader.ReadToEndAsync();
            }
        }

        return string.Empty;
    }
}
