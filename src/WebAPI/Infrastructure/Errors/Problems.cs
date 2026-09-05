using System.Collections.Generic;
using System.Diagnostics;
using ErrorOr;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Infrastructure.Errors;

internal static class Problems
{
    public const string ServerErrorDetail = "An unexpected error occurred.";

    public static string TraceId(HttpContext? http = null) =>
        Activity.Current?.TraceId.ToString() ?? http?.TraceIdentifier ?? "unknown";

    public static void Normalize(ProblemDetails problem, HttpContext? http = null, string? code = null)
    {
        var status = problem.Status ??= StatusCodes.Status500InternalServerError;
        problem.Extensions["traceId"] = TraceId(http);
        problem.Extensions.TryAdd("code", code ?? DefaultCode(status));
        problem.Detail ??= DefaultDetail(status);
    }

    public static ProblemHttpResult From(Error error, HttpContext? http = null)
    {
        var status = error.Type switch
        {
            ErrorType.NotFound => StatusCodes.Status404NotFound,
            ErrorType.Conflict => StatusCodes.Status409Conflict,
            ErrorType.Validation => StatusCodes.Status400BadRequest,
            ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
            ErrorType.Forbidden => StatusCodes.Status403Forbidden,
            _ => StatusCodes.Status500InternalServerError
        };

        var isServerError = status >= StatusCodes.Status500InternalServerError;

        var problem = new ProblemDetails
        {
            Status = status,
            Detail = isServerError ? ServerErrorDetail : error.Description
        };

        Normalize(problem, http, isServerError ? ProblemCodes.Unexpected : error.Code);
        return TypedResults.Problem(problem);
    }

    private static string DefaultCode(int status) => status switch
    {
        StatusCodes.Status400BadRequest => ProblemCodes.ValidationFailed,
        StatusCodes.Status401Unauthorized => ProblemCodes.Unauthorized,
        StatusCodes.Status403Forbidden => ProblemCodes.Forbidden,
        StatusCodes.Status404NotFound => ProblemCodes.NotFound,
        StatusCodes.Status405MethodNotAllowed => ProblemCodes.MethodNotAllowed,
        StatusCodes.Status415UnsupportedMediaType => ProblemCodes.UnsupportedMedia,
        _ => ProblemCodes.Unexpected
    };

    private static string DefaultDetail(int status) => status switch
    {
        StatusCodes.Status400BadRequest => "One or more fields in the request are invalid.",
        StatusCodes.Status401Unauthorized => "Authentication is required.",
        StatusCodes.Status403Forbidden => "Access to this resource is denied.",
        StatusCodes.Status404NotFound => "The requested resource was not found.",
        StatusCodes.Status405MethodNotAllowed => "The HTTP method is not supported for this resource.",
        StatusCodes.Status415UnsupportedMediaType => "The request content type is not supported.",
        _ => ServerErrorDetail
    };
}