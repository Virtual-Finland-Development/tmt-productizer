using TMTProductizer.Models.Cache.TMT;

namespace TMTProductizer.Services;
public interface ITMTAPIResultsCacheService
{
    /// <summary>
    /// Search from local cache, then S3
    /// </summary>
    public Task<CachedHakutulos?> GetCachedHakutulos();

    /// <summary>
    /// Save to S3
    /// </summary>
    public Task SaveCachedHakutulos(CachedHakutulos cachedResults);
}