using TMTProductizer.Services;

namespace TMTCacheUpdater;

public class CacheUpdateWorker : BackgroundService
{
    private readonly ITMTJobsFetcher _tmtJobsFetcher;
    private readonly IHostApplicationLifetime _lifeTime;

    public CacheUpdateWorker(ITMTJobsFetcher tmtJobsFetcher, IHostApplicationLifetime lifeTime)
    {
        _tmtJobsFetcher = tmtJobsFetcher;
        _lifeTime = lifeTime;
    }


    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _tmtJobsFetcher.UpdateTMTAPICache();
        _lifeTime.StopApplication();
    }
}