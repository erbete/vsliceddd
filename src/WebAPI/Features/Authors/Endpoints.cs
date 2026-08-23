using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using WebAPI.Features.Common;

namespace WebAPI.Features.Authors;

internal static class Endpoints
{
    internal static IServiceCollection AddAuthorsFeature(this IServiceCollection services)
    {
        services.AddScoped<GetAuthorById.Handler>();
        services.AddScoped<CreateAuthor.Handler>();
        services.AddScoped<DeleteAuthor.Handler>();
        return services;
    }

    internal static void MapAuthorsEndpoints(this RouteGroupBuilder builder)
    {
        var group = builder.MapGroup("/authors")
            .WithTags("Authors")
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapGet("/{id:guid}", GetAuthorById.Endpoint)
            .WithName(nameof(GetAuthorById))
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapPost("/", CreateAuthor.Endpoint)
            .WithName(nameof(CreateAuthor))
            .AddEndpointFilter<ValidationFilter<CreateAuthor.Request>>()
            .ProducesValidationProblem();

        group.MapDelete("/{id:guid}", DeleteAuthor.Endpoint)
            .WithName(nameof(DeleteAuthor));
    }
}