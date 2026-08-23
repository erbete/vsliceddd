using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace WebAPI.Features.Common;

internal sealed class ProblemLoggingFilter(ILogger<ProblemLoggingFilter> logger) : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        var result = await next(context);

        if (result is ProblemHttpResult problem &&
            problem.StatusCode is StatusCodes.Status409Conflict
                or StatusCodes.Status401Unauthorized
                or StatusCodes.Status403Forbidden)
        {
            var code = problem.ProblemDetails.Extensions.TryGetValue("code", out var c)
                ? c?.ToString() ?? "unknown"
                : "unknown";

            FilterLog.RequestRejected(logger, problem.StatusCode, code, problem.ProblemDetails.Detail ?? "");
        }

        return result;
    }
}

internal static partial class FilterLog
{
    [LoggerMessage(EventId = 4100, Level = LogLevel.Warning, Message = "Request rejected {StatusCode} {Code}: {Detail}")]
    public static partial void RequestRejected(ILogger logger, int statusCode, string code, string detail);
}