using Database;
using Domain.Common;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace WebAPI.Infrastructure.Setup;

public static class DomainServicesSetup
{
    public static WebApplicationBuilder AddDomainServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IIdGenerator, GuidV7IdGenerator>();
        return builder;
    }
}