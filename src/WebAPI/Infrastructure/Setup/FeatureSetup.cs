using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using WebAPI.Features.Authors;
using WebAPI.Features.Books;
using WebAPI.Features.Lending;

namespace WebAPI.Infrastructure.Setup;

public static class FeatureSetup
{
    public static WebApplicationBuilder AddFeatures(this WebApplicationBuilder builder)
    {
        builder.Services.AddHealthChecks();
        builder.Services.AddValidatorsFromAssemblyContaining<Program>(includeInternalTypes: true);
        builder.Services.AddAuthorsFeature();
        builder.Services.AddBooksFeature();
        builder.Services.AddLendingFeature();
        return builder;
    }
}