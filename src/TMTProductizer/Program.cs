using Microsoft.AspNetCore.Mvc;
using TMTProductizer.Models;
using TMTProductizer.Services;
using TMTProductizer.Services.AWS;
using TMTProductizer.Services.TMT;
using TMTProductizer.Utils;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<IJobService, JobService>();
builder.Services.AddSingleton<IAuthorizationService, AuthorizationService>();
builder.Services.AddSingleton<ISecretsManager, SecretsManager>();
builder.Services.AddSingleton<ITMTAuthorizationService, TMTAuthorizationService>();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGenNewtonsoftSupport();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllForDevelopment",
        policyBuilder => { policyBuilder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin(); });
});

builder.Services.AddSingleton<IProxyHttpClientFactory>(new ProxyHttpClientFactory(new Uri(builder.Configuration.GetSection("TmtApiEndpoint").Value)));

builder.Services.AddHttpClient<IAuthorizationService, AuthorizationService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration.GetSection("AuthGWEndpoint").Value);
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
