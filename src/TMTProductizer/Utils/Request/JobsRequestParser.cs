
using TMTProductizer.Data.Repositories;
using TMTProductizer.Models;
using TMTProductizer.Models.Repositories;

namespace TMTProductizer.Utils.Request;

public class JobsRequestParser : IRequestParser<JobsRequest>
{
    private readonly IOccupationCodeSetRepository _occupationCodeSetRepository;

    public JobsRequestParser(IOccupationCodeSetRepository occupationCodeSetRepository)
    {
        _occupationCodeSetRepository = occupationCodeSetRepository;
    }

    /// <summary>
    /// Parses the request and expands the occupation groups to their sub-occupations
    /// </summary>
    public async Task<JobsRequest> Parse(JobsRequest request)
    {
        JobsRequest parsedRequest = request.Clone();
        if (parsedRequest.Requirements.Occupations.Any())
        {
            var occupations = await _occupationCodeSetRepository.GetAllOccupations();

            var extendedOccupations = new List<String>();
            foreach (var occupationUri in parsedRequest.Requirements.Occupations)
            {
                // If URI an occupation group, add all sub-occupations
                if (!occupationUri.Contains("://data.europa.eu/esco/occupation/"))
                {
                    var subOccupationUris = GetSubOccupationURIs(occupationUri, occupations);
                    extendedOccupations.AddRange(subOccupationUris);
                }
            }

            foreach (var subUri in extendedOccupations.Distinct())
            {
                parsedRequest.Requirements.Occupations.Add(subUri);
            }

        }

        return parsedRequest;
    }

    private List<string> GetSubOccupationURIs(string parentOccupationURI, List<OccupationCodeSet.Occupation> occupations)
    {
        var subOccupations = new List<string>();
        foreach (var occupation in occupations)
        {
            if (occupation.Uri == null || occupation.Broader == null || !occupation.Broader.Any()) continue;
            if (occupation.Broader.Contains(parentOccupationURI))
            {
                subOccupations.Add(occupation.Uri);
                var subSubOccupations = GetSubOccupationURIs(occupation.Uri, occupations);
                subOccupations.AddRange(subSubOccupations);
            }
        }
        return subOccupations;
    }
}