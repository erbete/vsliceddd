using Domain.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace WebAPI.Features.Common;

internal static class ApiError
{
    public static ProblemHttpResult ToProblem(ResultError error) =>
        TypedResults.Problem(
            statusCode: error.Code switch
            {
                ErrorCode.NotFound   => StatusCodes.Status404NotFound,
                ErrorCode.Conflict   => StatusCodes.Status409Conflict,
                ErrorCode.Validation => StatusCodes.Status400BadRequest,
                _                    => StatusCodes.Status500InternalServerError
            },
            detail: error.Message);
}