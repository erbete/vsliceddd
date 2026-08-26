using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using WebAPI.Features.Common;

namespace WebAPI.Features.Lending;

internal static class Endpoints
{
    internal static IServiceCollection AddLendingFeature(this IServiceCollection services)
    {
        services.AddScoped<GetLoanById.Handler>();
        services.AddScoped<CreateLoan.Handler>();
        services.AddScoped<ReturnLoan.Handler>();
        return services;
    }

    internal static void MapLendingEndpoints(this RouteGroupBuilder builder)
    {
        var group = builder.MapGroup("/loans")
            .WithTags("Loans")
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapGet("/{id}", GetLoanById.Endpoint)
            .WithName(nameof(GetLoanById))
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapPost("/", CreateLoan.Endpoint)
            .AddEndpointFilter<ValidationFilter<CreateLoan.Request>>()
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapPost("/{id}/return", ReturnLoan.Endpoint)
            .AddEndpointFilter<ValidationFilter<ReturnLoan.Request>>()
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);
    }
}