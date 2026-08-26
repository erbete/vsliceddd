using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using WebAPI.Features.Common;

namespace WebAPI.Features.Members;

internal static class Endpoints
{
    internal static IServiceCollection AddMembersFeature(this IServiceCollection services)
    {
        services.AddScoped<CreateMember.Handler>();
        services.AddScoped<GetMemberById.Handler>();
        return services;
    }

    internal static void MapMembersEndpoints(this RouteGroupBuilder builder)
    {
        var group = builder.MapGroup("/members")
            .WithTags("Members")
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        group.MapGet("/{id}", GetMemberById.Endpoint)
            .WithName(nameof(GetMemberById))
            .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapPost("/", CreateMember.Endpoint)
            .AddEndpointFilter<ValidationFilter<CreateMember.Request>>()
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status409Conflict);
    }
}