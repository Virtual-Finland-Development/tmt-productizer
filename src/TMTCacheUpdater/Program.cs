using System.Diagnostics;
using TMTProductizer.Services;
using TMTProductizer.Services.AWS;
using TMTProductizer.Utils;

namespace TMTCacheUpdater;

internal class Program
{
    private static async Task Main(string[] args)
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var builder = Host.CreateDefaultBuilder(args);
        builder.ConfigureServices(
            services =>
                services.AddHostedService<CacheUpdateWorker>()
                    .AddScoped<IConfiguration>(i => configuration)
                    .AddScoped<ITMTJobsFetcher, TMTJobsFetcher>()
                    .AddScoped<IAuthorizationService, AuthGWAuthorizationService>()
                    .AddScoped<ISecretsManager, SecretsManager>()
                    .AddScoped<IAPIAuthorizationService, TMTAPIAuthorizationService>()
                    .AddScoped<IDynamoDBCache, DynamoDBCache>()
                    .AddScoped<IS3BucketCache, S3BucketCache>()
                    .AddScoped<ILocalFileCache, LocalFileCache>()
                    .AddScoped<IProxyHttpClientFactory, ProxyHttpClientFactory>()
                    .AddScoped<HttpClient>(sp => new HttpClient())
                );

        using (var host = builder.Build())
        {
            Console.WriteLine("Running CacheUpdateWorker...");
            Stopwatch watch = Stopwatch.StartNew();
            await host.RunAsync();
            Console.WriteLine($"Elapsed time {watch.ElapsedMilliseconds} ms.");
        }
    }
}