using ErrorOr;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace WebAPI.Features.Common;

internal static class Problems
{
    public static ProblemHttpResult From(Error error)
    {
        var (status, title) = error.Type switch
        {
            ErrorType.NotFound => (StatusCodes.Status404NotFound, "Not Found"),
            ErrorType.Conflict => (StatusCodes.Status409Conflict, "Conflict"),
            ErrorType.Validation => (StatusCodes.Status400BadRequest, "Bad Request"),
            _ => (StatusCodes.Status500InternalServerError, "Internal Server Error")
        };

        return TypedResults.Problem(
            statusCode: status,
            title: title,
            detail: status >= StatusCodes.Status500InternalServerError
                ? "An unexpected error occurred."
                : error.Description);
    }
}