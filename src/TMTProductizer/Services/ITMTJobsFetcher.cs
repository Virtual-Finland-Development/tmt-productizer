using CodeGen.Api.TMT.Model;

namespace TMTProductizer.Services;

public interface ITMTJobsFetcher
{
    /// <summary>
    /// Fetches the results from the cache or from the TMT API if the cache is empty.
    /// </summary>
    Task<Hakutulos> FetchTMTAPIResults();


    /// <summary>
    /// Updates the cache with the latest results from the TMT API.
    /// </summary>
    Task UpdateTMTAPICache();
}
