using System;
using Database;
using Database.Configurations;
using Database.Interceptors;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace WebAPI.Infrastructure.Setup;

public static class DatabaseSetup
{
	public static WebApplicationBuilder AddDatabase(this WebApplicationBuilder builder)
	{
		builder.Services.AddDatabase(builder.Configuration);
		return builder;
	}

	private static void AddDatabase(this IServiceCollection services, ConfigurationManager configuration)
	{
		services
			.AddOptions<DatabaseSettings>()
			.Bind(configuration.GetSection(DatabaseSettings.SectionName))
			.ValidateDataAnnotations()
			.ValidateOnStart();

		services.AddSingleton(TimeProvider.System);
		services.AddScoped<EntityAuditInterceptor>();
		services.AddScoped<DomainEventDispatchInterceptor>();

		services.AddDbContext<AppDbContext>((sp, options) =>
		{
			var settings = sp.GetRequiredService<IOptions<DatabaseSettings>>().Value;
			options
				.UseNpgsql(settings.DefaultConnection)
				.AddInterceptors(
					sp.GetRequiredService<EntityAuditInterceptor>(),
					sp.GetRequiredService<DomainEventDispatchInterceptor>());
		});
	}
}