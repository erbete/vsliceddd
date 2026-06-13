using Microsoft.AspNetCore.Builder;
using WebAPI.Infrastructure.Setup;
using WebAPI.Features;
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
        .AddFeatures();

    var app = builder.Build();

    app.UseExceptionHandling()
       .UseLogging()
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