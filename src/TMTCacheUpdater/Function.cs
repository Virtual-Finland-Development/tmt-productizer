using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TMTProductizer.Services;
using TMTProductizer.Services.AWS;
using TMTProductizer.Utils;

namespace TMTCacheUpdater;

public class Function
{
    IHostBuilder _builder;

    public Function()
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        _builder = Host.CreateDefaultBuilder();
        _builder.ConfigureServices(
            services =>
                services.AddScoped<IConfiguration>(i => configuration)
                    .AddScoped<ITMTJobsFetcher, TMTJobsFetcher>()
                    .AddScoped<ITMTAPIResultsCacheService, TMTAPIResultsCacheService>()
                    .AddScoped<IAuthorizationService, AuthGWAuthorizationService>()
                    .AddScoped<ISecretsManager, SecretsManager>()
                    .AddScoped<IAPIAuthorizationService, TMTAPIAuthorizationService>()
                    .AddScoped<IDynamoDBCache, DynamoDBCache>()
                    .AddScoped<IS3BucketCache, S3BucketCache>()
                    .AddScoped<ILocalFileCache, LocalFileCache>()
                    .AddScoped<IProxyHttpClientFactory, ProxyHttpClientFactory>()
                    .AddScoped<HttpClient>(sp => new HttpClient())
                );
    }

    public async Task FunctionHandler()
    {
        using (var host = _builder.Build())
        {
            Console.WriteLine("Running CacheUpdateWorker...");
            Stopwatch watch = Stopwatch.StartNew();
            var tmtJobsFetcher = host.Services.GetRequiredService<ITMTJobsFetcher>();
            await tmtJobsFetcher.UpdateTMTAPICache();
            Console.WriteLine($"Elapsed time {watch.ElapsedMilliseconds} ms.");
        }
    }
}