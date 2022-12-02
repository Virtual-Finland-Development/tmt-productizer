using TMTProductizer.Models.Cache;

namespace TMTProductizer.Services;

public interface ITMTJobsFetcher
{
    /// <summary>
    /// Fetches the results from the cache or from the TMT API if the cache is empty.
    /// </summary>
    Task<CachedHakutulos> FetchTMTAPIResults();


    /// <summary>
    /// Updates the cache with the latest results from the TMT API.
    /// </summary>
    Task UpdateTMTAPICache();
}
