using System.Diagnostics;
using ErrorOr;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Features.Common;

internal static class Problems
{
    public const string ServerErrorDetail = "An unexpected error occurred.";

    public static string TraceId(HttpContext? http = null)
        => Activity.Current?.TraceId.ToString() ?? http?.TraceIdentifier ?? "unknown";

    public static ProblemHttpResult From(Error error)
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

        Stamp(problem, isServerError ? "Error.Unexpected" : error.Code);
        return TypedResults.Problem(problem);
    }

    public static void Stamp(ProblemDetails problem, string code, HttpContext? http = null)
    {
        problem.Extensions["traceId"] = TraceId(http);
        problem.Extensions["code"] = code;
    }
}