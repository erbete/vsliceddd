using System;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace WebAPI.Features.Common;

internal sealed class ValidationFilter<T> : IEndpointFilter where T : class
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var validator = context.HttpContext.RequestServices.GetRequiredService<IValidator<T>>();

        var argument = context.Arguments.OfType<T>().FirstOrDefault();
        if (argument is null)
        {
            throw new InvalidOperationException(
                $"ValidationFilter<{typeof(T).Name}> found no argument of type {typeof(T).Name} in the request.");
        }

        var result = await validator.ValidateAsync(argument, context.HttpContext.RequestAborted);
        if (!result.IsValid)
        {
            return TypedResults.Problem(new HttpValidationProblemDetails(result.ToDictionary())
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "One or more validation errors occurred.",
                Detail = "One or more fields in the request body are invalid."
            });
        }

        return await next(context);
    }
}