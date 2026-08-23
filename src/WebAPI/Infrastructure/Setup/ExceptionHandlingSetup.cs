using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using WebAPI.Features.Common;

namespace WebAPI.Infrastructure.Setup;

public static class ExceptionHandlingSetup
{
    public static WebApplicationBuilder AddExceptionHandling(this WebApplicationBuilder builder)
    {
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        builder.Services.AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = ctx =>
            {
                var pd = ctx.ProblemDetails;

                pd.Extensions["traceId"] = Problems.TraceId(ctx.HttpContext);

                pd.Extensions.TryAdd("code", pd.Status switch
                {
                    StatusCodes.Status400BadRequest => "Request.Invalid",
                    StatusCodes.Status401Unauthorized => "Request.Unauthorized",
                    StatusCodes.Status403Forbidden => "Request.Forbidden",
                    StatusCodes.Status404NotFound => "Resource.NotFound",
                    StatusCodes.Status405MethodNotAllowed => "Request.MethodNotAllowed",
                    StatusCodes.Status415UnsupportedMediaType => "Request.UnsupportedMediaType",
                    _ => "Error.Unexpected"
                });

                pd.Detail ??= pd.Status switch
                {
                    StatusCodes.Status400BadRequest => "The request could not be processed.",
                    StatusCodes.Status401Unauthorized => "Authentication is required.",
                    StatusCodes.Status403Forbidden => "Access to this resource is denied.",
                    StatusCodes.Status404NotFound => "The requested resource was not found.",
                    StatusCodes.Status405MethodNotAllowed => "The HTTP method is not supported for this resource.",
                    StatusCodes.Status415UnsupportedMediaType => "The request content type is not supported.",
                    _ => Problems.ServerErrorDetail
                };
            };
        });

        return builder;
    }

    public static WebApplication UseExceptionHandling(this WebApplication app)
    {
        app.UseExceptionHandler();
        app.UseStatusCodePages();
        return app;
    }
}