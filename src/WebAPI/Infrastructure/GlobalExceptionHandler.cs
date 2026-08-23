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

		int status = problemDetails.Status ?? StatusCodes.Status500InternalServerError;
		if (status >= StatusCodes.Status500InternalServerError)
		{
			HandlerLog.UnexpectedException(logger, exception);
		}
		else
		{
			HandlerLog.RequestFailed(logger, status, exception);
		}

		httpContext.Response.StatusCode = status;

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

internal static partial class HandlerLog
{
	[LoggerMessage(EventId = 5000, Level = LogLevel.Error, Message = "Unexpected exception")]
	public static partial void UnexpectedException(Microsoft.Extensions.Logging.ILogger logger, Exception ex);

	[LoggerMessage(EventId = 4000, Level = LogLevel.Warning, Message = "Request failed with {StatusCode}")]
	public static partial void RequestFailed(Microsoft.Extensions.Logging.ILogger logger, int statusCode, Exception ex);
}