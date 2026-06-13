using FluentValidation;
using Microsoft.AspNetCore.Builder;
using WebAPI.Features.Shelves;

namespace WebAPI.Infrastructure.Setup;

public static class FeatureSetup
{
    public static WebApplicationBuilder AddFeatures(this WebApplicationBuilder builder)
    {
        builder.Services.AddValidatorsFromAssemblyContaining<Program>(includeInternalTypes: true);
        builder.Services.AddShelvesFeature();
        return builder;
    }
}