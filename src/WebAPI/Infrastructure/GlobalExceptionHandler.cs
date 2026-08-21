using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace WebAPI.Infrastructure;

internal sealed class GlobalExceptionHandler(
	IProblemDetailsService problemDetailsService,
	ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
	public async ValueTask<bool> TryHandleAsync(
		HttpContext httpContext,
		Exception exception,
		CancellationToken cancellationToken)
	{
		var problemDetails = exception switch
		{
			BadHttpRequestException bad => BadRequest(bad),
			DbUpdateConcurrencyException => new ProblemDetails
			{
				Status = StatusCodes.Status409Conflict,
				Title = "Conflict",
				Detail = "The resource was modified by another request. Reload and try again."
			},
			_ => new ProblemDetails
			{
				Status = StatusCodes.Status500InternalServerError,
				Title = "Internal Server Error",
				Detail = "An unexpected error occurred."
			}
		};

#pragma warning disable CA1848
		if (problemDetails.Status >= StatusCodes.Status500InternalServerError)
		{
			logger.LogError(exception, "Unexpected exception");
		}
		else
		{
			logger.LogWarning(exception, "Request failed with {StatusCode}", problemDetails.Status);
		}
#pragma warning restore CA1848

		httpContext.Response.StatusCode = problemDetails.Status!.Value;

		return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
		{
			HttpContext = httpContext,
			Exception = exception,
			ProblemDetails = problemDetails
		});
	}

	private static ProblemDetails BadRequest(BadHttpRequestException ex)
	{
		var problem = new ProblemDetails
		{
			Status = ex.StatusCode,
			Title = "One or more validation errors occurred.",
			Detail = "One or more fields in the request body are invalid."
		};

		if (ex.InnerException is JsonException { Path: not null } json)
		{
			problem.Extensions["errors"] = new Dictionary<string, string[]>
			{
				[json.Path.TrimStart('$', '.')] = ["Invalid value."]
			};
		}

		return problem;
	}
}