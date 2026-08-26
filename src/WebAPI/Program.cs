using System;
using Microsoft.AspNetCore.Builder;
using WebAPI.Infrastructure.Setup;

LoggingSetup.InitializeBootstrap();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.AddLogging()
        .AddDatabase()
        .AddDomainServices()
        .AddExceptionHandling()
        .AddOpenApiDocs()
        .AddTelemetry()
        .AddFeatures()
        .AddSeeding();

    var app = builder.Build();

    await app.UseSeedingAsync();

    app.UseLogging()
        .UseExceptionHandling()
        .UseOpenApiDocs()
        .UseEndpoints();

    await app.RunAsync();
}
catch (Exception ex)
{
    LoggingSetup.LogStartupFailure(ex);
}
finally
{
    await LoggingSetup.CloseAndFlushAsync();
}