using Microsoft.AspNetCore.Mvc;
using TMTProductizer.Models;
using TMTProductizer.Services;
using TMTProductizer.Services.AWS;
using TMTProductizer.Utils;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<IJobService, JobService>();
builder.Services.AddSingleton<ITMTJobsFetcher, TMTJobsFetcher>();
builder.Services.AddSingleton<IAuthorizationService, AuthGWAuthorizationService>();
builder.Services.AddSingleton<ISecretsManager, SecretsManager>();
builder.Services.AddSingleton<IAPIAuthorizationService>(sp => new TMTAPIAuthorizationService(
    sp.GetRequiredService<IHttpClientFactory>().CreateClient(),
    sp.GetRequiredService<IDynamoDBCache>(),
    sp.GetRequiredService<ISecretsManager>(),
    sp.GetRequiredService<ILogger<TMTAPIAuthorizationService>>(),
    sp.GetRequiredService<IHostEnvironment>(),
    (SecretsName: builder.Configuration.GetSection("TmtSecretsName").Value, SecretsRegion: builder.Configuration.GetSection("TmtSecretsRegion").Value)
));
builder.Services.AddSingleton<IDynamoDBCache>(new DynamoDBCache(builder.Configuration.GetSection("DynamoDBCacheName").Value));
builder.Services.AddSingleton<IS3BucketCache>(sp => new S3BucketCache(
    builder.Configuration.GetSection("S3BucketCacheName").Value, sp.GetRequiredService<ILogger<S3BucketCache>>()));
builder.Services.AddSingleton<IProxyHttpClientFactory>(new ProxyHttpClientFactory(new Uri(builder.Configuration.GetSection("TmtApiEndpoint").Value)));

builder.Services.AddHttpClient<IAuthorizationService, AuthGWAuthorizationService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration.GetSection("AuthGWEndpoint").Value);
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGenNewtonsoftSupport();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllForDevelopment",
        policyBuilder => { policyBuilder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin(); });
});

builder.Services.AddAWSLambdaHosting(LambdaEventSource.HttpApi);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors("AllowAllForDevelopment");
}


app.MapPost("/test/lassipatanen/Job/JobPosting", async (HttpRequest request, JobsRequest requestModel, [FromServices] IJobService service, [FromServices] IAuthorizationService authorizationService) =>
    {
        try
        {
            await authorizationService.Authorize(request);
        }
        catch (HttpRequestException)
        {
            return Results.StatusCode(401);
        }

        IReadOnlyList<Job> jobs;
        long totalCount;
        try
        {
            (jobs, totalCount) = await service.Find(requestModel);
        }
        catch (HttpRequestException e)
        {
            var statusCode = 500;
            if (e.StatusCode != null)
            {
                statusCode = (int)e.StatusCode;
            }
            return Results.StatusCode(statusCode);
        }


        var response = new
        {
            Results = jobs,
            TotalCount = totalCount
        };

        return Results.Ok(response);
    })
    .Produces(200)
    .Produces(401)
    .Produces(500)
    .WithName("FindJobPostings");

app.Run();
