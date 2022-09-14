using Microsoft.AspNetCore.Mvc;
using TMTProductizer.Config;
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

app.MapGet("/jobs", async (int? page, int? pageSize, [FromServices] IJobService service) =>
    {
        var pageNumber = page ?? 0;
        var pagerTake = pageSize ?? 10;

        var jobs = await service.Find(pageNumber, pagerTake);
        return jobs;
    })
    .Produces(200);

app.Run();