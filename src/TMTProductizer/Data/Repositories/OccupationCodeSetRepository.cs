using System.Text.Json;
using TMTProductizer.Models.Repositories;

namespace TMTProductizer.Data.Repositories;

public class OccupationCodeSetRepository : IOccupationCodeSetRepository
{
    private List<OccupationCodeSet.Occupation>? _occupations = null;

    public async Task<List<OccupationCodeSet.Occupation>> GetAllOccupations()
    {
        // Early return if occupations have already been fetched
        if (_occupations != null)
        {
            return _occupations;
        }

        var contents = await GetOccupationContents();

        if (!string.IsNullOrEmpty(contents))
        {
            var rootOccupationData = JsonSerializer.Deserialize<List<OccupationCodeSet.Occupation>>(contents);

            if (rootOccupationData is not null)
            {
                _occupations = rootOccupationData;
                return rootOccupationData;
            }
        }
        return new List<OccupationCodeSet.Occupation>();
    }

    private async Task<string> GetOccupationContents()
    {
        using var reader = new StreamReader("Data/RawData/esco-1.1.0-occupations.json");
        var contents = reader.ReadToEnd();
        return await Task.FromResult(contents);
    }
}
