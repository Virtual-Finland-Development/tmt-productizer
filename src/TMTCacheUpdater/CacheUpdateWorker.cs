using TMTProductizer.Services;

namespace TMTCacheUpdater;

public class CacheUpdateWorker : BackgroundService
{
    private readonly ITMTJobsFetcher _tmtJobsFetcher;

    public CacheUpdateWorker(ITMTJobsFetcher tmtJobsFetcher) =>
        _tmtJobsFetcher = tmtJobsFetcher;


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _tmtJobsFetcher.UpdateTMTAPICache();
    }
}