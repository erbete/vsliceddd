using Microsoft.AspNetCore.Builder;
using WebAPI.Infrastructure.Setup;
using System;

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
        .AddFeatures();

    var app = builder.Build();

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