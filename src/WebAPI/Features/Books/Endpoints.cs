using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using WebAPI.Features.Common;

namespace WebAPI.Features.Books;

internal static class Endpoints
{
    internal static IServiceCollection AddBooksFeature(this IServiceCollection services)
    {
        services.AddScoped<GetBookById.Handler>();
        services.AddScoped<CreateBook.Handler>();
        return services;
    }

    internal static void MapBooksEndpoints(this RouteGroupBuilder builder)
    {
        var group = builder.MapGroup("/books")
            .WithTags("Books")
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapGet("/{id}", GetBookById.Endpoint)
            .WithName(nameof(GetBookById))
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapPost("/", CreateBook.Endpoint)
            .WithName(nameof(CreateBook))
            .AddEndpointFilter<ValidationFilter<CreateBook.Request>>()
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);
    }
}