using System;
using System.Threading;
using System.Threading.Tasks;
using Database.Seeding;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

#pragma warning disable CA1848

namespace WebAPI.Infrastructure.Setup;

public static class SeedingSetup
{
    public static WebApplicationBuilder AddSeeding(this WebApplicationBuilder builder)
    {
        builder
            .Services.AddOptions<SeedOptions>()
            .Bind(builder.Configuration.GetSection(SeedOptions.SectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        builder.Services.AddScoped<LibrarySeeder>();
        return builder;
    }

    public static Task UseSeedingAsync(this WebApplication app)
    {
        if (!app.Environment.IsDevelopment())
        {
            var argsCheck = Environment.GetCommandLineArgs();
            if (argsCheck.Contains("--seed", StringComparer.OrdinalIgnoreCase))
            {
                var earlyLogger = app.Services.GetRequiredService<ILogger<LibrarySeeder>>();
                earlyLogger.LogWarning("Seeding: blocked — not Development environment");
            }

            return Task.CompletedTask;
        }

        var args = Environment.GetCommandLineArgs();
        var hasSeedFlag = args.Contains("--seed", StringComparer.OrdinalIgnoreCase);
        var hasForceFlag = args.Contains("--force", StringComparer.OrdinalIgnoreCase);

        var seedOptions = app.Services.GetRequiredService<IOptions<SeedOptions>>().Value;
        var shouldSeedOnStartup = seedOptions.SeedOnStartup;

        if (!hasSeedFlag && !shouldSeedOnStartup)
        {
            return Task.CompletedTask;
        }

        return UseSeedingCoreAsync(app, hasSeedFlag, hasForceFlag, shouldSeedOnStartup);
    }

    private static async Task UseSeedingCoreAsync(
        WebApplication app,
        bool hasSeedFlag,
        bool hasForceFlag,
        bool shouldSeedOnStartup
    )
    {
        using var scope = app.Services.CreateScope();
        var seeder = scope.ServiceProvider.GetRequiredService<LibrarySeeder>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<LibrarySeeder>>();

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));

        try
        {
            if (hasSeedFlag && hasForceFlag)
            {
                logger.LogWarning("Seeding: --seed --force requested");
                await seeder.MigrateAndSeedWithForceAsync(cts.Token);
            }
            else if (hasSeedFlag)
            {
                await seeder.MigrateAndSeedAsync(cts.Token);
            }
            else if (shouldSeedOnStartup)
            {
                logger.LogInformation("Seeding: SeedOnStartup enabled");
                await seeder.MigrateAndSeedAsync(cts.Token);
            }
        }
        catch (OperationCanceledException ex)
        {
            logger.LogError(ex, "Seeding: timed out after 30s");
            throw;
        }
    }
}
