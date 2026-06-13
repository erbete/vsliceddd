using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using WebAPI.Features.Common;

namespace WebAPI.Features.Shelves;

internal static class Endpoints
{
    internal static IServiceCollection AddShelvesFeature(this IServiceCollection services)
    {
        services.AddScoped<GetShelves.Handler>();
        services.AddScoped<GetShelfById.Handler>();
        services.AddScoped<CreateShelf.Handler>();
        services.AddScoped<AddBookToShelf.Handler>();
        services.AddScoped<DeleteShelf.Handler>();
        return services;
    }

    internal static void MapShelvesEndpoints(this RouteGroupBuilder builder)
    {
        var group = builder.MapGroup("/shelves").WithTags("Shelves");

        group.MapGet("", GetShelves.Endpoint);

        group.MapGet("/{id:guid}", GetShelfById.Endpoint).WithName(nameof(GetShelfById));

        group.MapPost("", CreateShelf.Endpoint)
            .AddEndpointFilter<ValidationFilter<CreateShelf.Request>>()
            .ProducesValidationProblem();

        group.MapPost("/{shelfId:guid}/books", AddBookToShelf.Endpoint)
            .AddEndpointFilter<ValidationFilter<AddBookToShelf.Request>>()
            .ProducesValidationProblem();

        group.MapDelete("/{id:guid}", DeleteShelf.Endpoint);
    }
}