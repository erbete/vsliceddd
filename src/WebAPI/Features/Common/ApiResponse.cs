using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;

namespace WebAPI.Features.Common;

internal static class ApiResponse
{
    public static Ok<Response<T>> Ok<T>(T data, HttpContext http)
        => TypedResults.Ok(Response<T>.Create(data, http.TraceIdentifier));

    public static Ok<Response<IReadOnlyList<T>>> Ok<T>(PagedResult<T> paged, HttpContext http)
    {
        var links = PaginationLinks.Build(http.Request, paged.Page, paged.PageSize, paged.TotalPages);

        return TypedResults.Ok(Response<IReadOnlyList<T>>.Create(
            paged.Data,
            http.TraceIdentifier,
            new PaginationMetadata(paged.Page, paged.PageSize, paged.TotalCount, paged.TotalPages, links)
        ));
    }

    public static Created<Response<T>> Created<T>(T data, HttpContext http)
        => TypedResults.Created((string?)null, Response<T>.Create(data, http.TraceIdentifier));

    public static CreatedAtRoute<Response<T>> CreatedAtRoute<T>(T data, string routeName, object routeValues, HttpContext http)
        => TypedResults.CreatedAtRoute(Response<T>.Create(data, http.TraceIdentifier), routeName, routeValues);
}
