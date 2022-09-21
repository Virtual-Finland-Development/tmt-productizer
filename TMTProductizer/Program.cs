using Microsoft.AspNetCore.Mvc;
using TMTProductizer.Config;
using TMTProductizer.Models;
using TMTProductizer.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<IJobService, JobService>();

builder.Services.Configure<TmtOptions>(builder.Configuration.GetSection("TmtOptions"));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGenNewtonsoftSupport();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapPost("/jobs", async (JobsRequest requestModel, [FromServices] IJobService service) =>
    {
        var jobs = await service.Find(requestModel);

        var response = new
        {
            Results = jobs,
            TotalCount = jobs.Count 
        };
        
        return Results.Ok(response);
    })
    .Produces(200);

app.Run();
