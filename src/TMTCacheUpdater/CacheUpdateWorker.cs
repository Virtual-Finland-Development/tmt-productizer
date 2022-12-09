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
        try
        {
            await _tmtJobsFetcher.UpdateTMTAPICache();
            Environment.ExitCode = 0; // Notify lambda runner that a success occurred
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            Environment.ExitCode = 1;
        }
        _lifeTime.StopApplication();
    }
}