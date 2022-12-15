using TMTProductizer.Models.Cache.TMT;
using TMTProductizer.Services.AWS;

namespace TMTProductizer.Services;

public class TMTAPIResultsCacheService : ITMTAPIResultsCacheService
{
    private readonly IS3BucketCache _s3BucketCache;
    private readonly ILogger<ITMTAPIResultsCacheService> _logger;
    private readonly ILocalFileCache _localFileCache;

    private readonly string _tmtCacheKey = "TMTJobResults";
    private readonly int _tmtCacheTTL = 24 * 60 * 60; // 24 h

    public TMTAPIResultsCacheService(IConfiguration configuration, ILogger<ITMTAPIResultsCacheService> logger, IS3BucketCache s3BucketCache, ILocalFileCache localFileCache)
    {
        _logger = logger;
        _s3BucketCache = s3BucketCache;
        _s3BucketCache.SetBucketName(configuration.GetSection("S3BucketCacheName").Value);
        _localFileCache = localFileCache;
    }

    public async Task<CachedHakutulos?> GetCachedHakutulos()
    {
        // First try from the faster local cache
        var response = await _localFileCache.GetCacheItem<CachedHakutulos>(_tmtCacheKey);
        if (response != null)
        {
            return response;
        }

        // If not found, try from S3
        var cacheItem = await _s3BucketCache.GetCacheItem<CachedHakutulos>(_tmtCacheKey);
        if (cacheItem != null)
        {
            // Save to local cache for faster access next time
            await _localFileCache.SaveCacheItem<CachedHakutulos>(_tmtCacheKey, cacheItem);
        }

        return cacheItem;
    }

    public async Task SaveCachedHakutulos(CachedHakutulos cachedResults)
    {
        await _s3BucketCache.SaveCacheItem<CachedHakutulos>(_tmtCacheKey, cachedResults, _tmtCacheTTL);
    }
}