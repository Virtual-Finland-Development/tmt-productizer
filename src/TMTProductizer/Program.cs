using Microsoft.AspNetCore.Mvc;
using TMTProductizer.Models;
using TMTProductizer.Services;
using TMTProductizer.Services.AWS;
using TMTProductizer.Utils;
using TMTProductizer.Data.Repositories;
using TMTProductizer.Utils.Request;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<IJobService, JobService>();
builder.Services.AddSingleton<ITMTJobsFetcher, TMTJobsFetcher>();
builder.Services.AddSingleton<ITMTAPIResultsCacheService, TMTAPIResultsCacheService>();
builder.Services.AddSingleton<IAuthorizationService, AuthGWAuthorizationService>();
builder.Services.AddSingleton<ISecretsManager, SecretsManager>();
builder.Services.AddSingleton<IAPIAuthorizationService, TMTAPIAuthorizationService>();
builder.Services.AddSingleton<IDynamoDBCache, DynamoDBCache>();
builder.Services.AddSingleton<IS3BucketCache, S3BucketCache>();
builder.Services.AddSingleton<ILocalFileCache, LocalFileCache>();
builder.Services.AddSingleton<IProxyHttpClientFactory, ProxyHttpClientFactory>();
builder.Services.AddSingleton<HttpClient>(sp => new HttpClient());
builder.Services.AddSingleton<IOccupationCodeSetRepository, OccupationCodeSetRepository>();
builder.Services.AddSingleton<IRequestParser<JobsRequest>, JobsRequestParser>();

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


app.MapPost("/test/lassipatanen/Job/JobPosting", async (HttpRequest request, JobsRequest jobsRequest, [FromServices] IRequestParser<JobsRequest> JobsRequestParser, [FromServices] IJobService service, [FromServices] IAuthorizationService authorizationService) =>
    {
        // Authorize the request
        try
        {
            await authorizationService.Authorize(request);
        }
        catch (HttpRequestException)
        {
            return Results.Problem("Access denied", statusCode: 401);
        }

        // Parse job request input data
        try
        {
            jobsRequest = await JobsRequestParser.Parse(jobsRequest);
        }
        catch (HttpRequestException)
        {
            return Results.Problem("Validation error", statusCode: 442);
        }

        // Fetch jobs
        try
        {
            IReadOnlyList<Job> jobs;
            long totalCount;
            (jobs, totalCount) = await service.Find(jobsRequest);

            return Results.Ok(new
            {
                Results = jobs,
                TotalCount = totalCount,
            });
        }
        catch (HttpRequestException e)
        {
            var statusCode = 500;
            if (e.StatusCode != null)
            {
                statusCode = (int)e.StatusCode;
            }
            return Results.Problem(e.ToString(), statusCode: statusCode);
        }
    })
    .Produces(200)
    .Produces(401)
    .Produces(500)
    .WithName("FindJobPostings");

app.MapGet("/wake-up", async ([FromServices] IJobService service) =>
{
    await service.WakeUp();
    return Results.Ok();
})
    .Produces(200)
    .WithName("WakeUp");

app.Run();
