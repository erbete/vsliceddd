using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
namespace WebAPI.Infrastructure.Setup;

public static class LoggingSetup
{
    // Bootstrap logging for startup failure logging. This is called in Program.cs before the builder exists
    public static void InitializeBootstrap()
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console(
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}",
                formatProvider: CultureInfo.InvariantCulture)
            .CreateBootstrapLogger();
    }

    // Main logging setup that replaces the bootstrap logger once the host loads
    public static WebApplicationBuilder AddLogging(this WebApplicationBuilder builder)
    {
        var logLevel = builder.Configuration["Logging:LogLevel:Default"] ?? "Information";
        var aspNetLevel = builder.Configuration["Logging:LogLevel:Microsoft.AspNetCore"] ?? "Warning";
        var systemLevel = builder.Configuration["Logging:LogLevel:System"] ?? "Warning";

        builder.Services.AddSerilog((services, lc) =>
        {
            lc
                .MinimumLevel.Is(Map(logLevel))
                .MinimumLevel.Override("Microsoft.AspNetCore", Map(aspNetLevel))
                .MinimumLevel.Override("System", Map(systemLevel))
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Application", "VSliceDDD")
                .WriteTo.Console(
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}",
                    formatProvider: CultureInfo.InvariantCulture);

            if (builder.Environment.IsDevelopment())
            {
                var logPath = Path.Combine(Path.GetTempPath(), "vsliceddd", "logs", "web-.txt");
                lc.WriteTo.File(
                    logPath,
                    rollingInterval: RollingInterval.Day,
                    formatProvider: CultureInfo.InvariantCulture);
            }
        });

        return builder;
    }

    public static WebApplication UseLogging(this WebApplication app)
    {
        app.UseSerilogRequestLogging(options =>
        {
            options.EnrichDiagnosticContext = (diag, http) => diag.Set("TraceId", http.TraceIdentifier);
            options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} {TraceId} {Elapsed:0.000}ms";
        });
        return app;
    }

    public static void LogStartupFailure(System.Exception ex)
        => Log.Fatal(ex, "Application terminated unexpectedly");

    public static async Task CloseAndFlushAsync()
        => await Log.CloseAndFlushAsync();

    private static LogEventLevel Map(string level) => level.ToUpperInvariant() switch
    {
        "TRACE" => LogEventLevel.Verbose,
        "DEBUG" => LogEventLevel.Debug,
        "INFORMATION" => LogEventLevel.Information,
        "WARNING" => LogEventLevel.Warning,
        "ERROR" => LogEventLevel.Error,
        "CRITICAL" or "FATAL" => LogEventLevel.Fatal,
        _ => LogEventLevel.Information
    };
}